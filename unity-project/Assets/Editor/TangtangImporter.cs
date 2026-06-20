using UnityEngine;
using UnityEditor;

namespace SugarRush.Editor
{
    /// <summary>
    /// One-time setup + runtime-visual helper for the "糖糖 / Tangtang" character model.
    ///
    /// The source is a single ~1.5M-triangle OBJ (AI-generated placeholder) with PBR
    /// textures. This importer (a) flags the normal map and caps texture sizes, (b) builds a
    /// Built-in Standard material wired with albedo + normal (metallic/roughness deferred to
    /// a later pass — see notes), and (c) instantiates the model as a player visual scaled
    /// and oriented for the 2D side-scroller.
    ///
    /// Known debt (intentionally deferred — "后面搞"):
    ///   - 1.5M tris must be decimated (~15k) before shipping; this is the as-is preview.
    ///   - Roughness map is not fed into Standard's smoothness (needs reverse + alpha pack).
    ///   - Model is one mesh, so per-part (pompom) glucose tint isn't possible yet.
    /// </summary>
    public static class TangtangImporter
    {
        private const string Dir = "Assets/Art/Characters/Tangtang";
        private const string ObjPath = Dir + "/Tangtang.obj";
        private const string AlbedoPath = Dir + "/texture_pbr_20250901.png";
        private const string NormalPath = Dir + "/texture_pbr_20250901_normal.png";
        private const string MaterialPath = Dir + "/Tangtang.mat";

        private const float TargetHeight = 1.5f;        // ~ player BoxCollider height (1.6)
        private const float FacingYawDegrees = 35f;     // 3/4 toward camera, skiing to the right

        [MenuItem("SugarRush/Setup/Import Tangtang Model")]
        public static void Setup()
        {
            ConfigureTextures();
            BuildMaterial();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[TangtangImporter] Textures + material configured. Now run Build L1 Scene.");
        }

        private static void ConfigureTextures()
        {
            SetTexture(NormalPath, TextureImporterType.NormalMap, 1024);
            SetTexture(AlbedoPath, TextureImporterType.Default, 2048);
        }

        private static void SetTexture(string path, TextureImporterType type, int maxSize)
        {
            if (AssetImporter.GetAtPath(path) is not TextureImporter importer)
            {
                Debug.LogWarning($"[TangtangImporter] Texture not found: {path}");
                return;
            }

            importer.textureType = type;
            importer.maxTextureSize = maxSize;
            importer.SaveAndReimport();
        }

        private static Material BuildMaterial()
        {
            var mat = new Material(Shader.Find("Standard"));

            var albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(AlbedoPath);
            var normal = AssetDatabase.LoadAssetAtPath<Texture2D>(NormalPath);

            if (albedo != null) mat.SetTexture("_MainTex", albedo);
            if (normal != null)
            {
                mat.SetTexture("_BumpMap", normal);
                mat.EnableKeyword("_NORMALMAP");
            }

            // Cartoon cloth/plastic: near-zero metalness, soft matte highlight. Constants stand
            // in for the metallic/roughness maps until they are reverse-packed in a later pass.
            mat.SetFloat("_Metallic", 0f);
            mat.SetFloat("_Glossiness", 0.3f);

            AssetDatabase.CreateAsset(mat, MaterialPath);
            return mat;
        }

        /// <summary>
        /// Instantiates the Tangtang model under <paramref name="parent"/> as a player visual,
        /// normalized to the player's height and rotated to face 3/4 toward the camera. Returns
        /// the spawned visual root, or null if the model asset is missing.
        /// </summary>
        public static GameObject InstantiateVisual(Transform parent)
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(ObjPath);
            if (model == null)
            {
                Debug.LogWarning($"[TangtangImporter] Model not found at {ObjPath}; player will have no visual. Did you run Import Tangtang Model / pull the asset?");
                return null;
            }

            var visual = (GameObject)PrefabUtility.InstantiatePrefab(model);
            visual.name = "TangtangVisual";
            visual.transform.SetParent(parent, false);

            FitToPlayer(visual);

            var mat = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (mat != null)
            {
                foreach (var r in visual.GetComponentsInChildren<MeshRenderer>())
                {
                    r.sharedMaterial = mat;
                }
            }

            return visual;
        }

        /// <summary>
        /// The raw mesh has an arbitrary origin and unit scale. Use the mesh's local bounds
        /// (available without a frame tick, unlike Renderer.bounds) to scale it to the player's
        /// height and recenter it on the player root, then yaw it to a 3/4 facing.
        /// </summary>
        private static void FitToPlayer(GameObject visual)
        {
            var filter = visual.GetComponentInChildren<MeshFilter>();
            if (filter == null || filter.sharedMesh == null)
            {
                Debug.LogWarning("[TangtangImporter] Model has no MeshFilter; skipping fit.");
                return;
            }

            Bounds local = filter.sharedMesh.bounds;
            float scale = TargetHeight / Mathf.Max(local.size.y, 0.0001f);

            visual.transform.localScale = Vector3.one * scale;
            visual.transform.localPosition = -local.center * scale; // center the mesh on the player root
            visual.transform.localRotation = Quaternion.Euler(0f, FacingYawDegrees, 0f);
        }
    }
}
