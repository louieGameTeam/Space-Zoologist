Shader "Unlit/TileShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData]_GridInformationTexture ("Grid Information Texture", 2D) = "white" {}
        _NoiseTexture("Noise Texture", 2D) = "white" {}
        _TileAtlas("Tile Atlas", 2D) = "white" {}

        _TileNoiseDistribution("Tile Noise Distribution", float) = 1
        
        [Toggle]_GridOverlayToggle("Grid Overlay Toggle", float) = 0
        _GridOverlayLineWidth("Grid OverLay Line Width", Range(0, 32)) = 0
        _GridOverlayDesaturation("Grid Overlay Desaturation", Range(0, 1)) = 0

        _LiquidColor("Liquid Color", COLOR) = (1, 1, 1, 1)
        _LiquidSubColor("Liquid Sub Color", COLOR) = (1, 1, 1, 1)
        _LiquidTextureScaling("Liquid Texture Scaling", Vector) = (1, 1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define PIXELS_PER_TILE 64
            #define TILE_TYPE_LIQUID 6
            // flags as determined in GridSystem
            #define LIQUID_FLAG 0x2
            #define HIGHLIGHT_FLAG 0x3

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.color = v.color;
                return o;
            }

            sampler2D _GridInformationTexture;
            float4 _GridInformationTexture_ST;
            sampler2D _NoiseTexture;
            float4 _NoiseTexture_ST;
            sampler2D _TileAtlas;
            float4 _TileAtlas_ST;

            float2 _GridTextureDimensions;

            float _GridOverlayToggle;
            int _GridOverlayLineWidth;
            float _GridOverlayDesaturation;

            float4 AddGrid(float4 col, float2 localPixel, int2 tilePos, float4 tileInformation) {
                // add the outlines
                if ((localPixel.x < _GridOverlayLineWidth || localPixel.x >= PIXELS_PER_TILE - _GridOverlayLineWidth)
                    || (localPixel.y < _GridOverlayLineWidth || localPixel.y >= PIXELS_PER_TILE - _GridOverlayLineWidth))
                    col = 1;

                // use texture's alpha channel to figure out if this tile is selected or not

                // if not selected, make saturated
                if (int(tileInformation.a * 256) % HIGHLIGHT_FLAG != 0 || tileInformation.a == 0) {
                    float2 grayScale = 0.33 * (col.r + col.g + col.b);
                    col.rgb = lerp(col.rgb, grayScale.xxx, _GridOverlayDesaturation);
                }

                return col;
            }

            float4 _LiquidColor;
            float4 _LiquidSubColor;
            float4 _LiquidTextureScaling;

            float4 AddLiquid(float4 col, float2 localPixel, float2 worldPos) {
                float4 liquid = 1;
                worldPos.xy /= _LiquidTextureScaling.xy;
                worldPos.y += _Time.x / 2;
                float2 noiseUV = float2(int2(frac(worldPos) * PIXELS_PER_TILE)) / PIXELS_PER_TILE;
                float noise = tex2D(_NoiseTexture, noiseUV);

                liquid = lerp(_LiquidColor, _LiquidSubColor, noise);

                col = liquid;

                return col;
            }

            float _TileNoiseDistribution;

            float4 frag(v2f i) : SV_Target
            {
                float4 col = 1;
                int2 tilePos = int2(i.worldPos.xy);
                float2 localUV = frac(i.worldPos.xy);
                int2 localPixel = localUV * PIXELS_PER_TILE;


                tilePos += int2(1, 1);
                float tileNoise = tex2D(_NoiseTexture, float2(tilePos) / _TileNoiseDistribution);
                // r: tile type, a: flag information
                float4 tileInformation = tex2D(_GridInformationTexture, float2(tilePos) / _GridTextureDimensions);

                // create local matrix for edge detections
                int3x3 tileTypeMatrix =
                {
                    0, 0, 0,
                    0, 0, 0,
                    0, 0, 0
                };

                // set matrix values
                tileTypeMatrix[0][0] = tex2D(_GridInformationTexture, float2(tilePos + int2(-1, -1)) / _GridTextureDimensions).r * 256;
                tileTypeMatrix[0][1] = tex2D(_GridInformationTexture, float2(tilePos + int2(0, -1)) / _GridTextureDimensions).r * 256;
                tileTypeMatrix[0][2] = tex2D(_GridInformationTexture, float2(tilePos + int2(1, -1)) / _GridTextureDimensions).r * 256;

                tileTypeMatrix[1][0] = tex2D(_GridInformationTexture, float2(tilePos + int2(-1, 0)) / _GridTextureDimensions).r * 256;
                tileTypeMatrix[1][1] = tex2D(_GridInformationTexture, float2(tilePos + int2(0, 0)) / _GridTextureDimensions).r * 256;
                tileTypeMatrix[1][2] = tex2D(_GridInformationTexture, float2(tilePos + int2(1, 0)) / _GridTextureDimensions).r * 256;

                tileTypeMatrix[2][0] = tex2D(_GridInformationTexture, float2(tilePos + int2(-1, 1)) / _GridTextureDimensions).r * 256;
                tileTypeMatrix[2][1] = tex2D(_GridInformationTexture, float2(tilePos + int2(0, 1)) / _GridTextureDimensions).r * 256;
                tileTypeMatrix[2][2] = tex2D(_GridInformationTexture, float2(tilePos + int2(1, 1)) / _GridTextureDimensions).r * 256;

                // get the correct tileset
                // 8 tiles in x, 6 tiles in y
                float xuvDim = float(1) / 8;
                float yuvDim = float(1) / 6;

                float yOffset = 5 - tileTypeMatrix[1][1] + localUV.y;
                float2 firstTilePosition = float2(xuvDim * localUV.x, yuvDim * yOffset);
                // get random between 4 base tiles
                int xOffset = int(tileNoise * 4);
                float2 tileAtlasPosition = firstTilePosition + float2(xuvDim * xOffset, 0);

                float4 tile = tex2D(_TileAtlas, tileAtlasPosition);

                col = tile;

                // add borders
                // edges first
                if (tilePos.x == 1 || tileTypeMatrix[1][0] == TILE_TYPE_LIQUID) {
                    float4 leftBar = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 5, 0));

                    if (localUV.x < 0.5)
                        col = lerp(col, leftBar, leftBar.a);
                }
                if (tilePos.x == _GridTextureDimensions.x || tileTypeMatrix[1][2] == TILE_TYPE_LIQUID) {
                    float4 rightBar = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 5, 0));

                    if (localUV.x > 0.5)
                        col = lerp(col, rightBar, rightBar.a);
                }
                if (tilePos.y == 1 || tileTypeMatrix[0][1] == TILE_TYPE_LIQUID) {
                    float4 bottomBar = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 4, 0));

                    if (localUV.y < 0.5)
                        col = lerp(col, bottomBar, bottomBar.a);
                }
                if (tilePos.y == _GridTextureDimensions.y || tileTypeMatrix[2][1] == TILE_TYPE_LIQUID) {
                    float4 topBar = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 4, 0));

                    if (localUV.y > 0.5)
                        col = lerp(col, topBar, topBar.a);
                }

                // corners after
                // outer corners
                if ((tilePos.x == 1 && tilePos.y == 1) ||
                    (tileTypeMatrix[1][0] == TILE_TYPE_LIQUID && tileTypeMatrix[0][1] == TILE_TYPE_LIQUID)) {
                    float4 blCorner = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 6, 0));

                    if (localUV.x < 0.5 && localUV.y < 0.5)
                        col = lerp(col, blCorner, blCorner.a);
                }
                if ((tilePos.x == _GridTextureDimensions.x && tilePos.y == 1) ||
                    (tileTypeMatrix[1][2] == TILE_TYPE_LIQUID && tileTypeMatrix[0][1] == TILE_TYPE_LIQUID)) {
                    float4 brCorner = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 6, 0));

                    if (localUV.x > 0.5 && localUV.y < 0.5)
                        col = lerp(col, brCorner, brCorner.a);
                }
                if ((tilePos.x == 1 && tilePos.y == _GridTextureDimensions.y) ||
                    (tileTypeMatrix[1][0] == TILE_TYPE_LIQUID && tileTypeMatrix[2][1] == TILE_TYPE_LIQUID)) {
                    float4 tlCorner = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 6, 0));

                    if (localUV.x < 0.5 && localUV.y > 0.5)
                        col = lerp(col, tlCorner, tlCorner.a);
                }
                if ((tilePos.x == _GridTextureDimensions.x && tilePos.y == _GridTextureDimensions.y) ||
                    (tileTypeMatrix[1][2] == TILE_TYPE_LIQUID && tileTypeMatrix[2][1] == TILE_TYPE_LIQUID)) {
                    float4 trCorner = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 6, 0));

                    if (localUV.x > 0.5 && localUV.y > 0.5)
                        col = lerp(col, trCorner, trCorner.a);
                }
                
                // inner corners
                if (tileTypeMatrix[0][0] == TILE_TYPE_LIQUID && tileTypeMatrix[0][1] != TILE_TYPE_LIQUID && tileTypeMatrix[1][0] != TILE_TYPE_LIQUID) {
                    float4 blInnerCorner = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 7, 0));

                    if (localUV.x < 0.5 && localUV.y < 0.5)
                        col = lerp(col, blInnerCorner, blInnerCorner.a);
                }
                if (tileTypeMatrix[0][2] == TILE_TYPE_LIQUID && tileTypeMatrix[0][1] != TILE_TYPE_LIQUID && tileTypeMatrix[1][2] != TILE_TYPE_LIQUID) {
                    float4 brInnerCorner = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 7, 0));

                    if (localUV.x > 0.5 && localUV.y < 0.5)
                        col = lerp(col, brInnerCorner, brInnerCorner.a);
                }
                if (tileTypeMatrix[2][0] == TILE_TYPE_LIQUID && tileTypeMatrix[1][0] != TILE_TYPE_LIQUID && tileTypeMatrix[2][1] != TILE_TYPE_LIQUID) {
                    float4 tlInnerCorner = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 7, 0));

                    if (localUV.x < 0.5 && localUV.y > 0.5)
                        col = lerp(col, tlInnerCorner, tlInnerCorner.a);
                }
                if (tileTypeMatrix[2][2] == TILE_TYPE_LIQUID && tileTypeMatrix[2][1] != TILE_TYPE_LIQUID && tileTypeMatrix[1][2] != TILE_TYPE_LIQUID) {
                    float4 trInnerCorner = tex2D(_TileAtlas, firstTilePosition + float2(xuvDim * 7, 0));

                    if (localUV.x > 0.5 && localUV.y > 0.5)
                        col = lerp(col, trInnerCorner, trInnerCorner.a);
                }
                

                // add liquid and other animated tiles first
                if (tileTypeMatrix[1][1] == 6)
                    col = AddLiquid(col, localPixel, i.worldPos.xy);
                
                // then add color modifier
                col *= i.color;

                // create grid
                if (_GridOverlayToggle > 0)
                    col = AddGrid(col, localPixel, tilePos, tileInformation);

                // add highlights if needed
                //if (int(tileInformation.a * 256) % HIGHLIGHT_FLAG == 0 && tileInformation.a != 0)
                    //col.rgb *= tileInformation.rgb;

                return col;
            }
            ENDCG
        }
    }
}
