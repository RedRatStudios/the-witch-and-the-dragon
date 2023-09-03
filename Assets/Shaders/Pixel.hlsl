#ifndef PIXEL_HLSL
#define PIXEL_HLSL
void Pixel_float(in float4 uv, in float res, in float pixelw, in float pixelh, out float4 Out)
{
    // here is where we pixelate the image, we just do pixel divided by resolution
    float dx = pixelw / res;
    float dy = pixelh / res;
    // math shit, ask me in DMs if you wanna know how it works
    float2 coord = float2(dx * floor(uv.x / dx), dy * floor(uv.y / dy));
    Out = float4(coord, 0, 1);
    // this probably has some issues with different aspect ratios and or resolutions, but i can fix those as they come up
}
#endif