using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Cinemachine;
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
                out var insulin, out var pills, out var snowflake, out var slipperyGround);

            var player = CreatePlayer(glucoseConfig, skiingConfig, slipperyGround);
            var inputGo = CreateInput();
            var input = inputGo.GetComponent<SugarRushInput>();
            CreateCamera(player.transform);
            CreateLighting();
            CreateGround(slipperyGround);
            CreateObstacles();
            CreateItems(insulin, pills, snowflake);
            CreateFinishLine();
            CreateGameFlow(input, player, levelData);
            CreateUI(player);

            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.Refresh();
            Debug.Log($"[L1SceneBuilder] Scene saved to {scenePath}");
        }

        private static void LoadAssets(out GlucoseConfig glucose, out SkiingConfig skiing,
            out LevelData level, out InsulinSprayEffect insulin, out HypoglycemicPillsEffect pills,
            out HighSugarSnowflakeEffect snowflake, out PhysicsMaterial2D slipperyGround)
        {
            glucose = AssetDatabase.LoadAssetAtPath<GlucoseConfig>("Assets/Data/Configs/GlucoseConfig.asset");
            skiing = AssetDatabase.LoadAssetAtPath<SkiingConfig>("Assets/Data/Configs/SkiingConfig.asset");
            level = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/Data/Configs/L1_HospitalChalet.asset");
            insulin = AssetDatabase.LoadAssetAtPath<InsulinSprayEffect>("Assets/Data/Items/InsulinSpray.asset");
            pills = AssetDatabase.LoadAssetAtPath<HypoglycemicPillsEffect>("Assets/Data/Items/HypoglycemicPills.asset");
            snowflake = AssetDatabase.LoadAssetAtPath<HighSugarSnowflakeEffect>("Assets/Data/Items/HighSugarSnowflake.asset");
            slipperyGround = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>("Assets/Data/PhysicsMaterials/SlipperyGround.physicsMaterial2D");
        }

        private static GameObject CreatePlayer(GlucoseConfig glucoseConfig, SkiingConfig skiingConfig, PhysicsMaterial2D slipperyGround)
        {
            var go = new GameObject("Player");
            go.tag = "Player";
            go.transform.position = new Vector3(0f, 0.8f, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Placeholders/WhiteSprite.png");
            sr.color = Color.cyan;
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

            // Wire input to player after creation via GameFlow
            return go;
        }

        private static void CreateCamera(Transform target)
        {
            var camGo = new GameObject("MainCamera");
            var camera = camGo.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.backgroundColor = new Color(0.76f, 0.88f, 0.95f);
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
        }

        private static void CreateLighting()
        {
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void CreateGround(PhysicsMaterial2D slipperyGround)
        {
            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer < 0)
            {
                groundLayer = 8; // Default custom layer index if Ground not defined
            }

            var ground = new GameObject("Ground");
            for (int i = 0; i < 20; i++)
            {
                float x = i * 12f;
                float y = -0.5f + (i % 2) * 0.5f; // slight variation
                var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                platform.name = $"Platform_{i}";
                platform.transform.SetParent(ground.transform);
                platform.transform.position = new Vector3(x, y, 0f);
                platform.transform.localScale = new Vector3(12f, 1f, 1f);
                UnityEngine.Object.DestroyImmediate(platform.GetComponent<Collider>());
                var col = platform.AddComponent<BoxCollider2D>();
                col.size = new Vector2(1f, 1f);
                col.sharedMaterial = slipperyGround;
                platform.layer = groundLayer;
                var renderer = platform.GetComponent<Renderer>();
                renderer.material = new Material(renderer.sharedMaterial);
                renderer.material.color = new Color(0.9f, 0.95f, 1f);
            }
        }

        private static void CreateObstacles()
        {
            var root = new GameObject("Obstacles");

            var rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rock.name = "StumbleRock";
            rock.transform.SetParent(root.transform);
            rock.transform.position = new Vector3(55f, 1f, 0f);
            rock.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            UnityEngine.Object.DestroyImmediate(rock.GetComponent<Collider>());
            rock.AddComponent<BoxCollider2D>();
            var obstacle = rock.AddComponent<Obstacle>();
            SetField(obstacle, "_type", Obstacle.ObstacleType.Stumble);

            var tree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tree.name = "CrashTree";
            tree.transform.SetParent(root.transform);
            tree.transform.position = new Vector3(115f, 1.5f, 0f);
            tree.transform.localScale = new Vector3(0.6f, 1.5f, 0.6f);
            UnityEngine.Object.DestroyImmediate(tree.GetComponent<Collider>());
            tree.AddComponent<BoxCollider2D>();
            var treeObstacle = tree.AddComponent<Obstacle>();
            SetField(treeObstacle, "_type", Obstacle.ObstacleType.Crash);
        }

        private static void CreateItems(InsulinSprayEffect insulin, HypoglycemicPillsEffect pills, HighSugarSnowflakeEffect snowflake)
        {
            var root = new GameObject("Items");

            CreatePickup(root.transform, "InsulinPickup", new Vector3(75f, 2f, 0f), insulin, Color.blue);
            CreatePickup(root.transform, "PillsPickup", new Vector3(135f, 2f, 0f), pills, Color.green);
            CreatePickup(root.transform, "SnowflakePickup", new Vector3(195f, 2f, 0f), snowflake, Color.yellow);
        }

        private static void CreatePickup(Transform parent, string name, Vector3 pos, ItemEffect effect, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
            var trigger = go.AddComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            var pickup = go.AddComponent<PickupItem>();
            SetField(pickup, "_itemEffect", effect);
            var renderer = go.GetComponent<Renderer>();
            renderer.material = new Material(renderer.sharedMaterial);
            renderer.material.color = color;
        }

        private static void CreateFinishLine()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "FinishLine";
            go.transform.position = new Vector3(230f, 3f, 0f);
            go.transform.localScale = new Vector3(1f, 4f, 1f);
            UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
            var trigger = go.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.size = new Vector2(1f, 1f);
            go.AddComponent<FinishLine>();
            var renderer = go.GetComponent<Renderer>();
            renderer.material = new Material(renderer.sharedMaterial);
            renderer.material.color = Color.magenta;
        }

        private static void CreateGameFlow(SugarRushInput input, GameObject player, LevelData levelData)
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
            SetField(input, "OnJumpPressed", null); // events will be wired at runtime via Awake/OnEnable
        }

        private static void CreateUI(GameObject player)
        {
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<UnityEngine.Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            var sliderGo = new GameObject("GlucoseSlider");
            sliderGo.transform.SetParent(canvasGo.transform, false);
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
            SetField(glucoseUI, "_glucoseSystem", player.GetComponent<GlucoseSystem>());
            SetField(glucoseUI, "_slider", slider);
            SetField(glucoseUI, "_fillImage", fillImage);

            var rect = sliderGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(20f, -20f);
            rect.sizeDelta = new Vector2(300f, 30f);
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
