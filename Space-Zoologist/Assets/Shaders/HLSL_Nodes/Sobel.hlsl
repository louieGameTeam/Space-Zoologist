float Sobel(v2f i)
{
    // Sobel Convolution Core
    const int Gx[9] =
    {
        -1, -2, -1,
                0, 0, 0,
                1, 2, 1
    };
    const int Gy[9] =
    {
        -1, 0, 1,
                -2, 0, 2,
                -1, 0, 1
    };
    half texColor;
    half edgeX = 0;
    half edgeY = 0;
    for (int it = 0; it < 9; it++)
    {
        texColor = luminance(tex2D(_MainTex, i.uv[it]));
        edgeX += texColor * Gx[it];
        edgeY += texColor * Gy[it];
    }
    half edge = 1 - abs(edgeX) - abs(edgeY);
    return edge;
}