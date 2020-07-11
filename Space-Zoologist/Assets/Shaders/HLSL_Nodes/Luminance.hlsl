float luminance(float4 color)
{
    return 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
}