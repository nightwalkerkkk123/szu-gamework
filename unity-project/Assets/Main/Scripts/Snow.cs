using UnityEngine;

namespace SnowShader
{
    public class Snow : MonoBehaviour
    {
        private static readonly int DrawPosition = Shader.PropertyToID("_DrawPosition");
        private static readonly int DrawAngle = Shader.PropertyToID("_DrawAngle");
        private static readonly int DrawBrush = Shader.PropertyToID("_DrawBrush");
        private static readonly int Scale = Shader.PropertyToID("_Scale");

        [SerializeField]
        private CustomRenderTexture snowHeightMap;

        [SerializeField]
        private Material snowHeightMapUpdater;

        private void Awake()
        {
            snowHeightMap.Initialize();
        }

        public void Draw(Vector3 uvCoords, float angle, float scale, Texture2D texture2D)
        {
            snowHeightMapUpdater.SetFloat(Scale, scale);
            snowHeightMapUpdater.SetTexture(DrawBrush, texture2D);
            snowHeightMapUpdater.SetVector(DrawPosition, uvCoords);
            snowHeightMapUpdater.SetFloat(DrawAngle, angle);
        }
    }
}