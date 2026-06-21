#ifndef SNOW_UTILS_H
#define SNOW_UTILS_H

void CalculateEdgeFade_float(
    float2 uv,
    float fadeStart,
    float fadeEnd,
    out float result)
{
    // Расстояние до ближайшего края по X и Y
    // Ex. min(0.2, 1 - 0.2) = 0.2
    float distX = min(uv.x, 1.0 - uv.x);
    float distY = min(uv.y, 1.0 - uv.y);

    // Минимальное расстояние до любого края
    float distanceToEdge = min(distX, distY);
    if (distanceToEdge > fadeStart)
    {
        result = 1;
        return;
    }

    result = smoothstep(fadeEnd, fadeStart, distanceToEdge);

    result = saturate(result);
}

#endif
