using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;
using SugarRush.Core;
using SugarRush.Foundation;
using SugarRush.Gameplay;
using SugarRush.Gameplay.Items;
using SugarRush.GameFlow;

namespace SugarRush.Editor
{
    public static class L1SceneBuilder
    {
        [MenuItem("SugarRush/Setup/Build L1 Scene")]
        public static void Build()
        {
            string scenePath = "Assets/Scenes/L1_HospitalChalet.unity";

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SceneManager.SetActiveScene(scene);

            LoadAssets(out var glucoseConfig, out var skiingConfig, out var levelData,
                out var insulin, out var pills, out var snowflake, out var candy, out var speedBoost,
                out var slipperyGround);

            GameConfig.Initialize(glucoseConfig, skiingConfig, levelData);

            var segments = EnsureSegments(levelData, insulin, pills, snowflake, candy, speedBoost);

            var player = CreatePlayer(glucoseConfig, skiingConfig, slipperyGround);
            var inputGo = CreateInput();
            var input = inputGo.GetComponent<SugarRushInput>();
            CreateCamera(player.transform);
            CreateLighting();
            CreateLevelFromSegments(segments, slipperyGround, player.GetComponent<GlucoseSystem>());
            CreateBackground();
            CreateSnowParticles();
            CreateGapDeathZone();
            var flow = CreateGameFlow(input, player, levelData);
            CreateUI(player, flow.GetComponent<LevelManager>());

            EditorSceneManager.SaveScene(scene, scenePath);
            EnsureSceneInBuildSettings(scenePath);
            AssetDatabase.Refresh();
            Debug.Log($"[L1SceneBuilder] Scene saved to {scenePath}");
        }

        private static void EnsureSceneInBuildSettings(string scenePath)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            bool alreadyIncluded = scenes.Exists(s => s.path == scenePath);
            if (alreadyIncluded) return;

            scenes.Add(new EditorBuildSettingsScene(scenePath, enabled: true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"[L1SceneBuilder] Added {scenePath} to Build Settings.");
        }

        private static void LoadAssets(out GlucoseConfig glucose, out SkiingConfig skiing,
            out LevelData level, out InsulinSprayEffect insulin, out HypoglycemicPillsEffect pills,
            out HighSugarSnowflakeEffect snowflake, out CandyEffect candy, out SpeedBoostEffect speedBoost,
            out PhysicsMaterial2D slipperyGround)
        {
            glucose = AssetDatabase.LoadAssetAtPath<GlucoseConfig>("Assets/Data/Configs/GlucoseConfig.asset");
            skiing = AssetDatabase.LoadAssetAtPath<SkiingConfig>("Assets/Data/Configs/SkiingConfig.asset");
            level = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/Data/Configs/L1_HospitalChalet.asset");
            insulin = AssetDatabase.LoadAssetAtPath<InsulinSprayEffect>("Assets/Data/Items/InsulinSpray.asset");
            pills = AssetDatabase.LoadAssetAtPath<HypoglycemicPillsEffect>("Assets/Data/Items/HypoglycemicPills.asset");
            snowflake = AssetDatabase.LoadAssetAtPath<HighSugarSnowflakeEffect>("Assets/Data/Items/HighSugarSnowflake.asset");
            candy = AssetDatabase.LoadAssetAtPath<CandyEffect>("Assets/Data/Items/Candy.asset");
            speedBoost = AssetDatabase.LoadAssetAtPath<SpeedBoostEffect>("Assets/Data/Items/SpeedBoost.asset");
            slipperyGround = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>("Assets/Data/PhysicsMaterials/SlipperyGround.physicsMaterial2D");
        }

        private static List<LevelSegmentData> EnsureSegments(LevelData level,
            InsulinSprayEffect insulin, HypoglycemicPillsEffect pills, HighSugarSnowflakeEffect snowflake,
            CandyEffect candy, SpeedBoostEffect speedBoost)
        {
            string folder = "Assets/Data/Segments";
            EnsureFolder(folder);

            // Always recreate L1 segments so iteration is consistent.
            DeleteSegmentAssets(folder);

            // Per-obstacle-type colors (placeholder sprites; helps the player read intent at a glance).
            // Rollable was previously a blue-gray (0.4, 0.5, 0.6) which blended
            // into the sky background (0.76, 0.88, 0.95) — now a dark wood tone.
            Color stumbleColor = new Color(0.55f, 0.4f, 0.35f);
            Color jumpableColor = new Color(0.4f, 0.65f, 0.3f);
            Color rollableColor = new Color(0.3f, 0.2f, 0.1f);
            Color avoidableColor = new Color(0.8f, 0.2f, 0.15f);

            // 1. Intro (250m, gentle 8° slope) — single low Stumble, first Insulin pickup.
            var segment1 = CreateSegmentAsset($"{folder}/L1_Segment_Intro.asset", "L1_Intro", 250f, 8f,
                obstacles: new SegmentObstacle[]
                {
                    new SegmentObstacle { DistanceAlongSegment = 180f, HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Stumble, Size = new Vector2(0.8f, 0.8f), Color = stumbleColor }
                },
                pickups: new SegmentPickup[]
                {
                    new SegmentPickup { DistanceAlongSegment = 120f, HeightOffset = 1.5f, ItemEffect = insulin, Color = Color.blue }
                });

            // 2. FirstJump (200m, 12°) — two low Jumpable to teach jumping, with speed boost.
            var segment2 = CreateSegmentAsset($"{folder}/L1_Segment_FirstJump.asset", "L1_FirstJump", 200f, 12f,
                obstacles: new SegmentObstacle[]
                {
                    new SegmentObstacle { DistanceAlongSegment = 60f,  HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Jumpable, HeightMeters = 0f, Size = new Vector2(0.9f, 0.4f), Color = jumpableColor },
                    new SegmentObstacle { DistanceAlongSegment = 140f, HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Jumpable, HeightMeters = 0f, Size = new Vector2(0.9f, 0.4f), Color = jumpableColor }
                },
                pickups: new SegmentPickup[]
                {
                    new SegmentPickup { DistanceAlongSegment = 100f, HeightOffset = 1.5f, ItemEffect = speedBoost, Color = Color.cyan }
                });

            // 3. FirstRoll (250m, 12°) — three Rollable to teach rolling (invuln).
            //    Candy health pack placed between obstacles as a safety net.
            var segment3 = CreateSegmentAsset($"{folder}/L1_Segment_FirstRoll.asset", "L1_FirstRoll", 250f, 12f,
                obstacles: new SegmentObstacle[]
                {
                    new SegmentObstacle { DistanceAlongSegment = 50f,  HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Rollable, Size = new Vector2(1.5f, 0.4f), Color = rollableColor },
                    new SegmentObstacle { DistanceAlongSegment = 130f, HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Rollable, Size = new Vector2(1.5f, 0.4f), Color = rollableColor },
                    new SegmentObstacle { DistanceAlongSegment = 200f, HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Rollable, Size = new Vector2(1.5f, 0.4f), Color = rollableColor }
                },
                pickups: new SegmentPickup[]
                {
                    new SegmentPickup { DistanceAlongSegment = 160f, HeightOffset = 1.2f, ItemEffect = candy, Color = Color.green }
                });

            // 4. ItemTutorial (250m, 12°) — pickup-only, teaches auto-pickup and item use.
            var segment4 = CreateSegmentAsset($"{folder}/L1_Segment_ItemTutorial.asset", "L1_ItemTutorial", 250f, 12f,
                pickups: new SegmentPickup[]
                {
                    new SegmentPickup { DistanceAlongSegment = 80f,  HeightOffset = 1.5f, ItemEffect = insulin,   Color = Color.blue },
                    new SegmentPickup { DistanceAlongSegment = 180f, HeightOffset = 1.5f, ItemEffect = snowflake, Color = Color.yellow }
                });

            // 5. Mixed (400m, 10° — breathing slope) — combines Jumpable-tall, Rollable, Stumble, ColdWind, Pills.
            var segment5 = CreateSegmentAsset($"{folder}/L1_Segment_Mixed.asset", "L1_Mixed", 400f, 10f,
                obstacles: new SegmentObstacle[]
                {
                    new SegmentObstacle { DistanceAlongSegment = 100f, HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Jumpable, HeightMeters = 1.4f, Size = new Vector2(0.9f, 0.4f), Color = jumpableColor },
                    new SegmentObstacle { DistanceAlongSegment = 200f, HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Rollable, Size = new Vector2(1.5f, 0.4f), Color = rollableColor },
                    new SegmentObstacle { DistanceAlongSegment = 320f, HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Stumble,  Size = new Vector2(0.8f, 0.8f), Color = stumbleColor }
                },
                pickups: new SegmentPickup[]
                {
                    new SegmentPickup { DistanceAlongSegment = 150f, HeightOffset = 1.2f, ItemEffect = candy, Color = Color.green },
                    new SegmentPickup { DistanceAlongSegment = 250f, HeightOffset = 1.5f, ItemEffect = pills, Color = Color.green }
                },
                hazards: new SegmentHazard[]
                {
                    // ColdWind band, ~80m wide, 4m tall, centered at 320m.
                    new SegmentHazard { DistanceAlongSegment = 320f, CenterHeightOffset = 4f, Size = new Vector2(80f, 4f), DeltaPerSecond = -3f }
                });

            // 6. FinalSprint (350m, 16° — peak slope) — Jumpable, Rollable, Avoidable, Snowflake, ColdWind.
            var segment6 = CreateSegmentAsset($"{folder}/L1_Segment_FinalSprint.asset", "L1_FinalSprint", 350f, 16f,
                obstacles: new SegmentObstacle[]
                {
                    new SegmentObstacle { DistanceAlongSegment = 80f,  HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Jumpable,  HeightMeters = 0f, Size = new Vector2(0.9f, 0.4f), Color = jumpableColor },
                    new SegmentObstacle { DistanceAlongSegment = 160f, HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Rollable,  Size = new Vector2(1.5f, 0.4f), Color = rollableColor },
                    new SegmentObstacle { DistanceAlongSegment = 280f, HeightOffset = 0.4f, Type = Obstacle.ObstacleType.Avoidable,  Size = new Vector2(1.2f, 1.2f), Color = avoidableColor }
                },
                pickups: new SegmentPickup[]
                {
                    new SegmentPickup { DistanceAlongSegment = 40f,  HeightOffset = 1.2f, ItemEffect = candy, Color = Color.green },
                    new SegmentPickup { DistanceAlongSegment = 140f, HeightOffset = 1.5f, ItemEffect = speedBoost, Color = Color.cyan },
                    new SegmentPickup { DistanceAlongSegment = 220f, HeightOffset = 1.5f, ItemEffect = snowflake, Color = Color.yellow }
                },
                hazards: new SegmentHazard[]
                {
                    // ColdWind band, 80m wide, centered at 140m.
                    new SegmentHazard { DistanceAlongSegment = 140f, CenterHeightOffset = 4f, Size = new Vector2(80f, 4f), DeltaPerSecond = -2f }
                });

            // 7. HospitalStation (300m, 8° — release) — clean approach to the FinishLine.
            var segment7 = CreateSegmentAsset($"{folder}/L1_Segment_HospitalStation.asset", "L1_HospitalStation", 300f, 8f);

            var segments = new List<LevelSegmentData> { segment1, segment2, segment3, segment4, segment5, segment6, segment7 };
            level.SetSegments(segments);
            EditorUtility.SetDirty(level);
            AssetDatabase.SaveAssets();

            return segments;
        }

        private static void DeleteSegmentAssets(string folder)
        {
            var guids = AssetDatabase.FindAssets("t:LevelSegmentData", new[] { folder });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.DeleteAsset(path);
            }
        }

        private static LevelSegmentData CreateSegmentAsset(string path, string id, float length, float slopeAngle,
            SegmentObstacle[] obstacles = null, SegmentPickup[] pickups = null, SegmentHazard[] hazards = null,
            bool hasGap = false, float gapStart = 0f, float gapEnd = 0f)
        {
            var segment = ScriptableObject.CreateInstance<LevelSegmentData>();
            var so = new SerializedObject(segment);
            so.FindProperty("<SegmentId>k__BackingField").stringValue = id;
            so.FindProperty("<Length>k__BackingField").floatValue = length;
            so.FindProperty("<SlopeAngle>k__BackingField").floatValue = slopeAngle;
            so.FindProperty("<GroundPieceLength>k__BackingField").floatValue = 12f;
            so.FindProperty("<HasGap>k__BackingField").boolValue = hasGap;
            so.FindProperty("<GapStartRatio>k__BackingField").floatValue = gapStart;
            so.FindProperty("<GapEndRatio>k__BackingField").floatValue = gapEnd;
            so.FindProperty("<GapDepth>k__BackingField").floatValue = 8f;
            so.ApplyModifiedProperties();

            if (obstacles != null) segment.Obstacles.AddRange(obstacles);
            if (pickups != null) segment.Pickups.AddRange(pickups);
            if (hazards != null) segment.Hazards.AddRange(hazards);

            AssetDatabase.CreateAsset(segment, path);
            return segment;
        }

        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
                var folderName = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        private static GameObject CreatePlayer(GlucoseConfig glucoseConfig, SkiingConfig skiingConfig, PhysicsMaterial2D slipperyGround)
        {
            var go = new GameObject("Player");
            go.tag = "Player";
            go.transform.position = new Vector3(0f, 1.5f, 0f);

            var whiteSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Placeholders/WhiteSprite.png");

            // Body — white ski jacket
            var bodyGO = new GameObject("Body");
            bodyGO.transform.SetParent(go.transform);
            bodyGO.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            var bodySR = bodyGO.AddComponent<SpriteRenderer>();
            bodySR.sprite = whiteSprite;
            bodySR.color = Color.white;
            bodySR.drawMode = SpriteDrawMode.Sliced;
            bodySR.size = new Vector2(0.65f, 1.1f);
            bodySR.sortingOrder = 1;

            // Head
            var headGO = new GameObject("Head");
            headGO.transform.SetParent(go.transform);
            headGO.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            var headSR = headGO.AddComponent<SpriteRenderer>();
            headSR.sprite = whiteSprite;
            headSR.color = new Color(1f, 0.82f, 0.7f, 1f);
            headSR.drawMode = SpriteDrawMode.Sliced;
            headSR.size = new Vector2(0.5f, 0.5f);
            headSR.sortingOrder = 2;

            // Red hat
            var hatGO = new GameObject("Hat");
            hatGO.transform.SetParent(headGO.transform);
            hatGO.transform.localPosition = new Vector3(0f, 0.22f, 0f);
            var hatSR = hatGO.AddComponent<SpriteRenderer>();
            hatSR.sprite = whiteSprite;
            hatSR.color = new Color(0.85f, 0.15f, 0.15f, 1f);
            hatSR.drawMode = SpriteDrawMode.Sliced;
            hatSR.size = new Vector2(0.55f, 0.3f);
            hatSR.sortingOrder = 3;

            // Yellow scarf
            var scarfGO = new GameObject("Scarf");
            scarfGO.transform.SetParent(go.transform);
            scarfGO.transform.localPosition = new Vector3(0.15f, 0.45f, -0.02f);
            var scarfSR = scarfGO.AddComponent<SpriteRenderer>();
            scarfSR.sprite = whiteSprite;
            scarfSR.color = new Color(1f, 0.85f, 0.2f, 1f);
            scarfSR.drawMode = SpriteDrawMode.Sliced;
            scarfSR.size = new Vector2(0.4f, 0.15f);
            scarfSR.sortingOrder = 2;

            // Main SpriteRenderer for PlayerVisuals reference
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = whiteSprite;
            sr.color = new Color(1f, 1f, 1f, 0f); // invisible — children provide the visuals
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(0.8f, 1.6f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2.5f;
            rb.freezeRotation = true;

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 1.6f);
            col.sharedMaterial = slipperyGround;

            var glucose = go.AddComponent<GlucoseSystem>();
            SetField(glucose, "_config", glucoseConfig);

            var skiing = go.AddComponent<SkiingController>();
            SetField(skiing, "_config", skiingConfig);
            SetField(skiing, "_glucoseSystem", glucose);
            SetField(skiing, "_hitbox", col);

            CreateSkiTrail(go.transform, skiing, glucose);

            var visuals = go.AddComponent<PlayerVisuals>();
            SetField(visuals, "_bodyRenderer", bodySR);
            SetField(visuals, "_skiingController", skiing);
            SetField(visuals, "_glucoseSystem", glucose);

            var particles = go.AddComponent<SimpleParticleSpawner>();
            SetField(particles, "_particleSprite", whiteSprite);

            var inventory = go.AddComponent<PlayerInventory>();
            SetField(inventory, "_glucoseSystem", glucose);
            SetField(inventory, "_skiingController", skiing);

            return go;
        }

        private static GameObject CreateInput()
        {
            var go = new GameObject("Input");
            var input = go.AddComponent<SugarRushInput>();
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>("Assets/Data/SugarRush.inputactions");
            SetField(input, "_inputAsset", asset);

            return go;
        }

        private static void CreateCamera(Transform target)
        {
            var camGo = new GameObject("MainCamera");
            var camera = camGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.backgroundColor = new Color(0.45f, 0.7f, 0.95f);
            camGo.transform.position = new Vector3(0f, 2f, -10f);
            camGo.tag = "MainCamera";

            var brain = camGo.AddComponent<CinemachineBrain>();
            brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);

            var vcamGo = new GameObject("CM vcam1");
            var vcam = vcamGo.AddComponent<CinemachineVirtualCamera>();
            vcam.Follow = target;
            vcam.m_Lens.Orthographic = true;
            vcam.m_Lens.OrthographicSize = 6f;
            vcamGo.AddComponent<CameraFollow>();
            SetField(vcamGo.GetComponent<CameraFollow>(), "_target", target);
            SetField(vcamGo.GetComponent<CameraFollow>(), "_virtualCamera", vcam);

            var transposer = vcam.AddCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_ScreenX = 0.35f;
            transposer.m_DeadZoneWidth = 0.1f;
            transposer.m_DeadZoneHeight = 0.15f;

            var camEffects = vcamGo.AddComponent<CameraEffects>();
            SetField(camEffects, "_skiingController", target.GetComponent<SkiingController>());
            SetField(camEffects, "_glucoseSystem", target.GetComponent<GlucoseSystem>());
        }

        private static void CreateLighting()
        {
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void CreateBackground()
        {
            var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Textures/Mountain_BG.png");
            var camGo = GameObject.Find("MainCamera");
            if (camGo == null) { Debug.LogWarning("[L1SceneBuilder] MainCamera not found for background."); return; }

            // 3D sprite background attached to camera with fixed large scale
            // (avoids edge bleeding from varying game-view aspect ratios)
            var bgRoot = new GameObject("Background");
            bgRoot.transform.SetParent(camGo.transform);
            bgRoot.transform.localPosition = new Vector3(0f, 0f, 20f);

            var mountain = new GameObject("MountainBG");
            mountain.transform.SetParent(bgRoot.transform);
            mountain.transform.localPosition = Vector3.zero;
            mountain.transform.localScale = new Vector3(20f, 20f, 1f);

            var sr = mountain.AddComponent<SpriteRenderer>();
            sr.sprite = bgSprite;
            sr.color = Color.white;
            sr.drawMode = SpriteDrawMode.Simple;
            sr.sortingOrder = -100;
        }

        private static void CreateSnowParticles()
        {
            var go = new GameObject("SnowParticles");
            go.transform.position = new Vector3(90f, 8f, 0f);

            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 60f;
            main.loop = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(4f, 8f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 1.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
            main.startColor = new Color(1f, 1f, 1f, 0.6f);
            main.maxParticles = 120;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = 0.02f;

            var emission = ps.emission;
            emission.rateOverTime = 25f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(200f, 0.1f, 1f);
            shape.position = new Vector3(0f, 5f, 0f);

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        private static void CreateContinuousGround(GameObject groundRoot, int groundLayer, PhysicsMaterial2D physicsMat,
            float startX, float startY, float length, float slopeAngle, int id)
        {
            float rad = slopeAngle * Mathf.Deg2Rad;
            float halfLen = length * 0.5f;
            float centerX = startX + halfLen * Mathf.Cos(rad);
            float centerY = startY - halfLen * Mathf.Sin(rad);

            var go = new GameObject($"GroundCollider_{id}");
            go.transform.SetParent(groundRoot.transform);
            go.transform.position = new Vector3(centerX, centerY, 0f);
            go.transform.rotation = Quaternion.Euler(0f, 0f, -slopeAngle);
            go.layer = groundLayer;

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(length + 1.5f, 3f);
            col.offset = new Vector2(0f, -0.3f);
            col.sharedMaterial = physicsMat;
        }

        private static void CreateLevelFromSegments(List<LevelSegmentData> segments, PhysicsMaterial2D slipperyGround, GlucoseSystem glucoseSystem)
        {
            if (segments == null || segments.Count == 0)
            {
                Debug.LogError("[L1SceneBuilder] No segments provided.");
                return;
            }

            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer < 0)
            {
                Debug.LogWarning("[L1SceneBuilder] Ground layer not defined. Falling back to layer 8.");
                groundLayer = 8;
            }

            var groundRoot = new GameObject("Ground");
            var obstaclesRoot = new GameObject("Obstacles");
            var itemsRoot = new GameObject("Items");
            var hazardsRoot = new GameObject("Hazards");

            // Align the first ground piece so its center is at (0,0). Player spawns above it.
            float firstRad = segments[0].SlopeAngle * Mathf.Deg2Rad;
            float firstHalfLength = segments[0].GroundPieceLength * 0.5f;
            float currentX = -firstHalfLength * Mathf.Cos(firstRad);
            float currentY = firstHalfLength * Mathf.Sin(firstRad);
            int platformIndex = 0;

            foreach (var segment in segments)
            {
                if (segment == null) continue;

                float rad = segment.SlopeAngle * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                float segmentWorldDX = segment.Length * cos;
                float segmentWorldDY = segment.Length * sin;

                // Build ground pieces
                int pieces = Mathf.Max(1, Mathf.CeilToInt(segment.Length / segment.GroundPieceLength));
                float pieceLength = segment.Length / pieces;

                // First pass: create one continuous collider per continuous block
                // This eliminates gaps between individual platforms
                float segStartX = currentX;
                float segStartY = currentY;
                float segEndX = currentX + segmentWorldDX;
                float segEndY = currentY - segmentWorldDY;

                if (segment.HasGap)
                {
                    // Gap segment: create two continuous colliders (before gap, after gap)
                    float gapStartDist = segment.GapStartRatio * segment.Length;
                    float gapEndDist = segment.GapEndRatio * segment.Length;

                    // Pre-gap block
                    CreateContinuousGround(groundRoot, groundLayer, slipperyGround,
                        currentX, currentY, gapStartDist, segment.SlopeAngle, platformIndex);

                    // Post-gap block
                    float postGapStartX = currentX + gapEndDist * cos;
                    float postGapStartY = currentY - gapEndDist * sin;
                    float postGapLength = segment.Length - gapEndDist;
                    CreateContinuousGround(groundRoot, groundLayer, slipperyGround,
                        postGapStartX, postGapStartY, postGapLength, segment.SlopeAngle, platformIndex + 100);
                }
                else
                {
                    // No gap: one single continuous collider for the whole segment
                    CreateContinuousGround(groundRoot, groundLayer, slipperyGround,
                        currentX, currentY, segment.Length, segment.SlopeAngle, platformIndex);
                }

                // Second pass: create visual sprites (decorative, no colliders needed)
                for (int i = 0; i < pieces; i++)
                {
                    float tCenter = (i + 0.5f) / pieces;

                    // Skip visual pieces inside the gap.
                    if (segment.HasGap && tCenter >= segment.GapStartRatio && tCenter <= segment.GapEndRatio)
                        continue;

                    float centerDistance = tCenter * segment.Length;
                    float x = currentX + centerDistance * cos;
                    float y = currentY - centerDistance * sin;

                    // Visual snow sprite with per-platform collider (avoids Box2D huge-collider bugs)
                    var platform = new GameObject($"Platform_{platformIndex++}");
                    platform.transform.SetParent(groundRoot.transform);
                    platform.transform.position = new Vector3(x, y, 0f);
                    platform.transform.rotation = Quaternion.Euler(0f, 0f, -segment.SlopeAngle);
                    platform.layer = groundLayer;

                    var platCol = platform.AddComponent<BoxCollider2D>();
                    platCol.size = new Vector2(pieceLength + 0.3f, 1.5f);
                    platCol.offset = new Vector2(0f, -0.25f);
                    platCol.sharedMaterial = slipperyGround;

                    var sr = platform.AddComponent<SpriteRenderer>();
                    var whiteSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Placeholders/WhiteSprite.png");
                    sr.sprite = whiteSprite;
                    sr.color = new Color(0.92f, 0.96f, 1f, 1f);
                    sr.drawMode = SpriteDrawMode.Tiled;
                    sr.size = new Vector2(pieceLength, 1.2f);
                    sr.sortingOrder = 0;

                    // Snow overlay
                    var overlay = new GameObject("SnowTop");
                    overlay.transform.SetParent(platform.transform);
                    overlay.transform.localPosition = new Vector3(0f, 0.55f, -0.01f);
                    var overlaySR = overlay.AddComponent<SpriteRenderer>();
                    overlaySR.sprite = whiteSprite;
                    overlaySR.color = new Color(1f, 1f, 1f, 0.4f);
                    overlaySR.drawMode = SpriteDrawMode.Tiled;
                    overlaySR.size = new Vector2(pieceLength, 0.3f);
                    overlaySR.sortingOrder = 1;
                }

                // Visual dark pit for gap.
                if (segment.HasGap)
                {
                    float gapStartDist = segment.GapStartRatio * segment.Length;
                    float gapEndDist = segment.GapEndRatio * segment.Length;
                    float gapMidDist = (gapStartDist + gapEndDist) * 0.5f;
                    float gapX = currentX + gapMidDist * cos;
                    float gapY = currentY - gapMidDist * sin;
                    float gapWorldWidth = (gapEndDist - gapStartDist) * cos;

                    var pit = CreateSpriteObject(groundRoot.transform, "GapPit",
                        new Vector3(gapX, gapY - segment.GapDepth * 0.5f - 1f, 0.1f),
                        new Vector2(gapWorldWidth + 2f, segment.GapDepth),
                        new Color(0.1f, 0.12f, 0.15f, 1f));
                    pit.transform.rotation = Quaternion.Euler(0f, 0f, -segment.SlopeAngle);
                }

                // Place obstacles
                foreach (var obstacle in segment.Obstacles)
                {
                    Vector3 pos = EvaluateSegmentPosition(currentX, currentY, segment.Length, segment.SlopeAngle, obstacle.DistanceAlongSegment, obstacle.HeightOffset);
                    var go = CreateSpriteObject(obstaclesRoot.transform, obstacle.Type.ToString(), pos, obstacle.Size, obstacle.Color);
                    go.AddComponent<BoxCollider2D>();
                    var obs = go.AddComponent<Obstacle>();
                    SetField(obs, "_type", obstacle.Type);
                }

                // Place pickups
                foreach (var pickup in segment.Pickups)
                {
                    if (pickup.ItemEffect == null) continue;
                    Vector3 pos = EvaluateSegmentPosition(currentX, currentY, segment.Length, segment.SlopeAngle, pickup.DistanceAlongSegment, pickup.HeightOffset);
                    CreatePickup(itemsRoot.transform, pickup.ItemEffect.DisplayName, pos, pickup.ItemEffect, pickup.Color);
                }

                // Place hazards
                foreach (var hazard in segment.Hazards)
                {
                    Vector3 pos = EvaluateSegmentPosition(currentX, currentY, segment.Length, segment.SlopeAngle, hazard.DistanceAlongSegment, hazard.CenterHeightOffset);
                    CreateHazard(hazardsRoot.transform, pos, hazard.Size, hazard.DeltaPerSecond, glucoseSystem);
                }

                currentX += segmentWorldDX;
                currentY -= segmentWorldDY;
            }

            // Finish line at end of last segment
            Vector3 finishPos = new Vector3(currentX, currentY + 2f, 0f);
            var finish = CreateSpriteObject(null, "FinishLine", finishPos, new Vector2(1f, 4f), Color.magenta);
            var finishTrigger = finish.AddComponent<BoxCollider2D>();
            finishTrigger.isTrigger = true;
            finish.AddComponent<FinishLine>();
        }

        private static Vector3 EvaluateSegmentPosition(float segmentStartX, float segmentStartY, float segmentLength, float slopeAngle, float distanceAlongSegment, float heightOffset)
        {
            float ratio = Mathf.Clamp01(distanceAlongSegment / segmentLength);
            float rad = slopeAngle * Mathf.Deg2Rad;
            float x = segmentStartX + ratio * segmentLength * Mathf.Cos(rad);
            float y = segmentStartY - ratio * segmentLength * Mathf.Sin(rad) + heightOffset;
            return new Vector3(x, y, 0f);
        }

        private static void CreatePickup(Transform parent, string name, Vector3 pos, ItemEffect effect, Color color)
        {
            var go = CreateSpriteObject(parent, name, pos, new Vector2(1.5f, 1.5f), color);
            // Use the ItemEffect's Icon sprite if available, otherwise keep white placeholder
            if (effect != null && effect.Icon != null)
            {
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = effect.Icon;
                    sr.color = Color.white; // Show true sprite colors, not tinted
                }
            }
            var trigger = go.AddComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = 1.0f;
            var pickup = go.AddComponent<PickupItem>();
            SetField(pickup, "_itemEffect", effect);
        }

        private static void CreateHazard(Transform parent, Vector3 position, Vector2 size, float deltaPerSecond, GlucoseSystem glucoseSystem)
        {
            var go = CreateSpriteObject(parent, "HazardZone", position, size, new Color(0.4f, 0.7f, 1f, 0.3f));

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = size;

            var hazard = go.AddComponent<HazardZone>();
            SetField(hazard, "_glucoseSystem", glucoseSystem);
            SetField(hazard, "_deltaPerSecond", deltaPerSecond);
        }

        private static void CreateGapDeathZone()
        {
            var go = new GameObject("GapDeathZone");
            // Place well below the entire downhill track (which reaches ~-350 at 2000m).
            go.transform.position = new Vector3(0f, -500f, 0f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(5000f, 20f);

            go.AddComponent<GapDeathZone>();
        }

        private static GameObject CreateGameFlow(SugarRushInput input, GameObject player, LevelData levelData)
        {
            var go = new GameObject("GameFlow");
            go.transform.position = Vector3.zero;

            var flow = go.AddComponent<GameFlowManager>();
            SetField(flow, "_input", input);
            SetField(flow, "_playerController", player.GetComponent<SkiingController>());
            SetField(flow, "_glucoseSystem", player.GetComponent<GlucoseSystem>());
            SetField(flow, "_introDuration", 0.1f);

            var levelManager = go.AddComponent<LevelManager>();
            SetField(levelManager, "_levelData", levelData);
            SetField(levelManager, "_player", player.transform);

            // Wire input to player components
            SetField(player.GetComponent<SkiingController>(), "_input", input);
            SetField(player.GetComponent<PlayerInventory>(), "_input", input);

            return go;
        }

        private static void CreateUI(GameObject player, LevelManager levelManager)
        {
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<UnityEngine.Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

            CreateGlucoseVisionOverlay(canvasGo.transform, player.GetComponent<GlucoseSystem>());
            CreateGlucoseBar(canvasGo.transform, player.GetComponent<GlucoseSystem>());
            CreateHUD(canvasGo.transform, player, levelManager);
            CreateResultPanel(canvasGo.transform);
        }

        private static void CreateGlucoseVisionOverlay(Transform canvas, GlucoseSystem glucoseSystem)
        {
            var go = new GameObject("GlucoseVisionOverlay");
            go.transform.SetParent(canvas, false);

            var image = go.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            image.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            var frostGo = new GameObject("FrostOverlay");
            frostGo.transform.SetParent(go.transform, false);
            var frostImage = frostGo.AddComponent<UnityEngine.UI.Image>();
            frostImage.color = new Color(1f, 1f, 1f, 0f);
            frostImage.raycastTarget = false;
            var frostRect = frostGo.GetComponent<RectTransform>();
            frostRect.anchorMin = Vector2.zero;
            frostRect.anchorMax = Vector2.one;
            frostRect.sizeDelta = Vector2.zero;

            var effect = go.AddComponent<GlucoseVisionEffect>();
            SetField(effect, "_glucoseSystem", glucoseSystem);
            SetField(effect, "_frostImage", frostImage);
        }

        private static void CreateGlucoseBar(Transform canvas, GlucoseSystem glucoseSystem)
        {
            var sliderGo = new GameObject("GlucoseSlider");
            sliderGo.transform.SetParent(canvas, false);
            var slider = sliderGo.AddComponent<UnityEngine.UI.Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.6f;
            slider.wholeNumbers = false;

            var backgroundGo = new GameObject("Background");
            backgroundGo.transform.SetParent(sliderGo.transform, false);
            var bgImage = backgroundGo.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = Color.gray;
            var bgRect = backgroundGo.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderGo.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;

            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(fillArea.transform, false);
            var fillImage = fillGo.AddComponent<UnityEngine.UI.Image>();
            fillImage.color = Color.green;
            var fillRect = fillGo.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            slider.fillRect = fillRect;
            slider.targetGraphic = fillImage;

            var glucoseUI = sliderGo.AddComponent<GlucoseUI>();
            SetField(glucoseUI, "_glucoseSystem", glucoseSystem);
            SetField(glucoseUI, "_slider", slider);
            SetField(glucoseUI, "_fillImage", fillImage);

            var rect = sliderGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(20f, -20f);
            rect.sizeDelta = new Vector2(300f, 30f);
        }

        private static void CreateHUD(Transform canvas, GameObject player, LevelManager levelManager)
        {
            var hudGo = new GameObject("HUD");
            hudGo.transform.SetParent(canvas, false);

            // Semi-transparent background panel for readability
            var bgGo = new GameObject("HUDBackground");
            bgGo.transform.SetParent(hudGo.transform, false);
            var bgImage = bgGo.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0f, 0f, 0f, 0.4f);
            var bgRect = bgGo.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 1f);
            bgRect.anchorMax = new Vector2(0f, 1f);
            bgRect.pivot = new Vector2(0f, 1f);
            bgRect.anchoredPosition = new Vector2(8f, -8f);
            bgRect.sizeDelta = new Vector2(440f, 165f);

            var hud = hudGo.AddComponent<HUD>();
            SetField(hud, "_levelManager", levelManager);
            SetField(hud, "_skiingController", player.GetComponent<SkiingController>());
            SetField(hud, "_glucoseSystem", player.GetComponent<GlucoseSystem>());

            var distanceText = CreateText(hudGo.transform, "DistanceText", new Vector2(20f, -60f), TextAnchor.UpperLeft);
            var speedText = CreateText(hudGo.transform, "SpeedText", new Vector2(20f, -95f), TextAnchor.UpperLeft);
            var timeText = CreateText(hudGo.transform, "TimeText", new Vector2(20f, -130f), TextAnchor.UpperLeft);
            var glucoseText = CreateText(hudGo.transform, "GlucoseValueText", new Vector2(20f, -25f), TextAnchor.UpperLeft);

            SetField(hud, "_distanceText", distanceText);
            SetField(hud, "_speedText", speedText);
            SetField(hud, "_timeText", timeText);
            SetField(hud, "_glucoseText", glucoseText);
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition, TextAnchor anchor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<UnityEngine.UI.Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 28;
            text.color = Color.white;
            text.alignment = anchor;
            text.text = name;

            var outline = go.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var shadow = go.AddComponent<UnityEngine.UI.Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
            shadow.effectDistance = new Vector2(2f, -2f);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(420f, 34f);

            return text;
        }

        private static void CreateResultPanel(Transform canvas)
        {
            var panelGo = new GameObject("ResultPanel");
            panelGo.transform.SetParent(canvas, false);
            var panel = panelGo.AddComponent<ResultPanel>();

            var root = new GameObject("PanelRoot");
            root.transform.SetParent(panelGo.transform, false);
            var rootImage = root.AddComponent<UnityEngine.UI.Image>();
            rootImage.color = new Color(0f, 0f, 0f, 0.75f);
            var rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;
            SetField(panel, "_panel", root);

            var title = CreateText(root.transform, "TitleText", new Vector2(0f, 60f), TextAnchor.MiddleCenter);
            title.fontSize = 48;
            title.color = Color.white;
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 60f);
            titleRect.sizeDelta = new Vector2(500f, 80f);
            SetField(panel, "_titleText", title);

            var message = CreateText(root.transform, "MessageText", new Vector2(0f, -20f), TextAnchor.MiddleCenter);
            message.fontSize = 28;
            message.color = Color.white;
            var messageRect = message.GetComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0.5f, 0.5f);
            messageRect.anchorMax = new Vector2(0.5f, 0.5f);
            messageRect.pivot = new Vector2(0.5f, 0.5f);
            messageRect.anchoredPosition = new Vector2(0f, -20f);
            messageRect.sizeDelta = new Vector2(600f, 60f);
            SetField(panel, "_messageText", message);

            var buttonGo = new GameObject("RestartButton");
            buttonGo.transform.SetParent(root.transform, false);
            var buttonImage = buttonGo.AddComponent<UnityEngine.UI.Image>();
            buttonImage.color = new Color(0.2f, 0.5f, 0.9f);
            var buttonRect = buttonGo.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = new Vector2(0f, -100f);
            buttonRect.sizeDelta = new Vector2(200f, 60f);
            var button = buttonGo.AddComponent<UnityEngine.UI.Button>();
            SetField(panel, "_restartButton", button);

            var buttonText = CreateText(buttonGo.transform, "RestartText", Vector2.zero, TextAnchor.MiddleCenter);
            buttonText.fontSize = 28;
            buttonText.color = Color.white;
            buttonText.text = "再来一局";
            var buttonTextRect = buttonText.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.sizeDelta = Vector2.zero;
        }

        private static void CreateSki(Transform parent, Vector2 localPosition)
        {
            var go = new GameObject("Ski");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
            go.transform.localRotation = Quaternion.Euler(0f, 0f, -12f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Placeholders/WhiteSprite.png");
            sr.color = new Color(0.2f, 0.2f, 0.2f);
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(0.12f, 1.2f);
        }

        private static void CreateSkiTrail(Transform parent, SkiingController skiingController, GlucoseSystem glucoseSystem)
        {
            var go = new GameObject("SkiTrail");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0f, -0.7f, 0.1f);

            var trail = go.AddComponent<TrailRenderer>();
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.time = 0.4f;
            trail.widthMultiplier = 0.25f;
            trail.minVertexDistance = 0.05f;
            trail.startColor = new Color(0.85f, 0.95f, 1f, 0.7f);
            trail.endColor = new Color(0.85f, 0.95f, 1f, 0f);
            trail.sortingLayerID = parent.GetComponentInChildren<SpriteRenderer>()?.sortingLayerID ?? 0;
            trail.sortingOrder = -1;

            var skiTrail = go.AddComponent<SkiTrail>();
            SetField(skiTrail, "_skiingController", skiingController);
            SetField(skiTrail, "_glucoseSystem", glucoseSystem);
        }

        private static GameObject CreateSpriteObject(Transform parent, string name, Vector3 position, Vector2 size, Color color)
        {
            var go = new GameObject(name);
            if (parent != null)
            {
                go.transform.SetParent(parent);
            }
            go.transform.position = position;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Placeholders/WhiteSprite.png");
            sr.color = color;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = size;
            return go;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
                return;
            }

            var prop = target.GetType().GetProperty(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(target, value);
                return;
            }

            Debug.LogWarning($"[L1SceneBuilder] Could not set {fieldName} on {target.GetType().Name}");
        }
    }
}
