using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace SugarRush.EditorTools
{
    /// <summary>
    /// Idempotent one-click snow atmosphere for any open scene:
    /// gradient skybox + cool fog + winter directional light + camera-following snow.
    /// </summary>
    public static class SnowAtmosphereSetup
    {
        private const string SkyboxPath = "Assets/Materials/SnowSkybox.mat";
        private const string SnowRootName = "SnowAtmosphere";

        // Cold winter sky/fog tone. Geometry fades into this exact color via fog,
        // giving a seamless "snowfield whiteout" without skybox sun artifacts.
        private static readonly Color SkyColor = new Color(0.62f, 0.76f, 0.88f);

        [MenuItem("SugarRush/Setup/Apply Snow Atmosphere")]
        public static void Apply()
        {
            // 1. Flat cold ambient (procedural skybox produces warm horizon near the
            //    sun, wrong for snow — use a solid cold sky via camera clear color below).
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.85f, 0.92f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.78f, 0.85f, 0.95f);
            RenderSettings.ambientGroundColor = new Color(0.7f, 0.75f, 0.82f);

            // 2. Fog matched to sky color (hides chunk pop-in + depth)
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = SkyColor;
            RenderSettings.fogStartDistance = 20f;
            RenderSettings.fogEndDistance = 110f;

            // 3. Winter directional light
            var light = Object.FindObjectOfType<Light>();
            if (light == null)
            {
                var go = new GameObject("Directional Light");
                light = go.AddComponent<Light>();
                light.type = LightType.Directional;
            }
            light.color = new Color(0.95f, 0.97f, 1f);
            light.intensity = 1.1f;
            light.transform.rotation = Quaternion.Euler(35f, -40f, 0f);
            light.shadows = LightShadows.Soft;

            // 4. Camera: solid cold-blue clear so the world reads as a snowfield
            var cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = SkyColor;
            }

            // 5. Camera-following snow particle system (reconfigure if already present)
            if (cam != null)
            {
                var existing = cam.transform.Find(SnowRootName);
                GameObject snow;
                if (existing == null)
                {
                    snow = new GameObject(SnowRootName);
                    snow.transform.SetParent(cam.transform, false);
                    snow.transform.localPosition = new Vector3(0f, 8f, 12f);
                }
                else
                {
                    snow = existing.gameObject;
                }
                var ps = snow.GetComponent<ParticleSystem>();
                if (ps == null) ps = snow.AddComponent<ParticleSystem>();
                ConfigureSnow(ps);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[SnowAtmosphere] Applied to scene: " + EditorSceneManager.GetActiveScene().name);
        }

        private static void ConfigureSnow(ParticleSystem ps)
        {
            var main = ps.main;
            main.startSize = 0.12f;
            main.startSpeed = 2.5f;
            main.startLifetime = 5f;
            main.maxParticles = 600;
            main.startColor = new Color(1f, 1f, 1f, 0.85f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 80f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(40f, 1f, 30f);

            var vel = ps.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.World;
            // All three axes must share the same MinMaxCurve mode (random-between-two-constants).
            vel.x = new ParticleSystem.MinMaxCurve(-0.4f, 0.4f);
            vel.y = new ParticleSystem.MinMaxCurve(-2.0f, -3.0f);
            vel.z = new ParticleSystem.MinMaxCurve(-0.4f, 0.4f);

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        }
    }
}
