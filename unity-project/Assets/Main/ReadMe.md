# ENG
# SnowShader
Snow shader for Unity HDRP/Shader Graph.

## Features
- Snow deformation based on Render Texture.
- Compatible with High Definition Render Pipeline (HDRP).
- Configurable parameters:
<br/>
MainTexture - the main texture of snow. <br/>
Metallic - metallic <br/>
BaseMapTiling - tiling MainTexture <br/>
Bottom Color - the color of the lower part of the snow during deformation. <br/>
Normal Texture - the texture of the snow normality. <br/>
NormalMapTiling - NormalMap tiling. <br/>
NormalStength - the strength of the normal map. <br/>
HeightMap is a height map (texture) for the deformation (RenderTexture). <br/>
HeightMultiplier - the height of the snow. <br/>
BottomHeight is a variable for determining the height of a color during deformation. <br/>
EdgeFadeStart is in progress. <br/>
EdgeFadeEnd is in progress. <br/>
Splat Dissolve is a variable for blurring borders during deformation. Mostly in the range of [0.1] <br/>
Noise Texture - noise texture. <br/>
Noise Tiling - tiling for Noise Texture. <br/>
Noise Sparkling is in progress. <br/>
Tesselation Factor - the power of tessellation. <br/>

# Warnings
For the shader to work correctly, the brush texture must use a Wrap Mode other than Repeat, and it must not touch its own edges. The background of the brush should be white.