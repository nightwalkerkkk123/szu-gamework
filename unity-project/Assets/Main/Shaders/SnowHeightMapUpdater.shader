Shader "CustomRenderTexture/SnowHeightMapUpdater"
{
    Properties
    {
        _DrawPosition("DrawPosition", Vector) = (0.5,0.5,0,0)

        _DrawBrush("DrawBrush", 2D) = "white"

        _Scale("Scale", float) = 1

        _DrawAngle("Angle", float) = 0
    }

    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass
        {
            Name "SnowHeightMapUpdater"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4 _DrawPosition;
            sampler2D _DrawBrush;
            float _Scale;
            float _DrawAngle;
            
            float4 frag(v2f_customrendertexture IN) : SV_Target
            {
                float2 uv = IN.localTexcoord.xy;
                float4 previousPixelColor = tex2D(_SelfTexture2D, uv);

                // Расстояние от текущего пикселя до _DrawPosition (в UV-пространстве).
                float2 drawPos = uv.xy - _DrawPosition;

                float2x2 rotMatrix = float2x2(cos(_DrawAngle), -sin(_DrawAngle), sin(_DrawAngle), cos(_DrawAngle));

                drawPos = mul(rotMatrix, drawPos);
                drawPos /= _Scale;
                drawPos += float2(0.5, 0.5);
                
                float4 drawTexture = tex2D(_DrawBrush, drawPos);

                float4 a = float4(previousPixelColor.xy, 0, 1);
                float4 b = float4(drawTexture.xy, 0, 1);

                return  min(a, b); //min(float4(1,1,1,1), drawTexture);
            }
            ENDCG
        }
    }
}