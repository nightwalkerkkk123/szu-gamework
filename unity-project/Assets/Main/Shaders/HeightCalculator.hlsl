#ifndef SNOW_UTILS_INCLUDED
#define SNOW_UTILS_INCLUDED

void CalculateHeight_float(
    Texture2D splatMap,
    SamplerState samplerState,
    float2 uv,
    float blurSize,
    out float result)
{    
    float mean_height = 0;
    const int iterations = 9;

    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            float x = uv.x + i * blurSize;
            float y = uv.y + j * blurSize;
            float2 blur = float2(x, y);

            mean_height += SAMPLE_TEXTURE2D_LOD(splatMap, samplerState, blur, 1).r;
        }
    }

    result = saturate(mean_height / iterations);
    
}

#endif
