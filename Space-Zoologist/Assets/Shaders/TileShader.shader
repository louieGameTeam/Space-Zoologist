Shader "Unlit/TileShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData]_GridInfoTex ("Grid Information Texture", 2D) = "white" {}
        _NoiseTexture("Noise Texture", 2D) = "white" {}
        _TileAtlas("Tile Atlas", 2D) = "white" {}
        _PropAtlas("Prop Atlas", 2D) = "white" {}

        _TileNoiseDistribution("Tile Noise Distribution", float) = 1
        _TilePropSparsity("Tile Prop Sparsity", Range(0, 1)) = 1
        
        [Toggle]_GridOverlayToggle("Grid Overlay Toggle", float) = 0
        _GridOverlayLineWidth("Grid Overlay Line Width", Range(0, 32)) = 0
        _GridOverlayDesaturation("Grid Overlay Desaturation", Range(0, 1)) = 0
        _GridOverlayRulerTiles("Grid Overlay Ruler Tiles", Range(0, 32)) = 0
        _GridOverlayRulerColor("Grid Overlay Ruler Color", COLOR) = (1, 1, 1, 1)
        _GridOverlayRulerLineWidth("Grid Overlay Ruler Line Width", Range(0, 32)) = 0
        
        _CameraDefaultOrthoHeight("Camera Default Zoom Ortho Height(For grid scaling reference)", float) = 9.4

        _LiquidColor("Liquid Color", COLOR) = (1, 1, 1, 1)
        _LiquidSubColor("Liquid Sub Color", COLOR) = (1, 1, 1, 1)
        _LiquidTextureScaling("Liquid Texture Scaling", Vector) = (1, 1, 0, 0)

        _BlendBorderWidth("Blend Border Width", float) = 0
        _BlendUVScale("Blend UV Scale", float) = 1
        _RoundingDist("Rounding Distance", Range(0, 1)) = 0

    }
    SubShader
    {
        Tags {"Queue" = "Background" "PreviewType" = "Plane"}

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define PIXELS_PER_TILE 64
            #define TILE_TYPE_EMPTY 0 
            #define TILE_TYPE_WALL 6
            #define TILE_TYPE_LIQUID 7
            #define TILES_X 5
            #define TILES_Y 6
            #define PROPS_X 6
            #define H_LIQUID_BORDER 1
            #define V_LIQUID_BORDER 2
            #define OUTER_CORNER 3
            #define INNER_CORNER 4
            // flags as determined in GridSystem
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

            sampler2D _GridInfoTex;
            float4 _GridInfoTex_ST;
            sampler2D _NoiseTexture;
            float4 _NoiseTexture_ST;
            sampler2D _TileAtlas;
            float4 _TileAtlas_ST;
            sampler2D _PropAtlas;
            float4 _PropAtlas_ST;

            float2 _GridTexDim;

            float _TilePropSparsity;

            float _GridOverlayToggle;
            int _GridOverlayLineWidth;
            float _GridOverlayDesaturation;
            int _GridOverlayRulerTiles;
            float4 _GridOverlayRulerColor;
            int _GridOverlayRulerLineWidth;

            float _CameraDefaultOrthoHeight;

            float4 AddGrid(float4 col, float2 localPixel, int2 tilePos, float4 tileInformation) {
                //Line scaling to avoid problems when zoomed out - should probably do scale calculations outside of shader?
                //Clamped above one to avoid thin lines when zoomed in - should probably find a less static solution for scale
                float line_scale = max(1,unity_OrthoParams.y / _CameraDefaultOrthoHeight);

                float _GridOverlayLineWidth_final = _GridOverlayLineWidth * line_scale;
                // add the outlines
                if ((localPixel.x < _GridOverlayLineWidth_final || localPixel.x >= PIXELS_PER_TILE - _GridOverlayLineWidth_final )
                    || (localPixel.y < _GridOverlayLineWidth_final || localPixel.y >= PIXELS_PER_TILE - _GridOverlayLineWidth_final ))
                    col = 1;

                // if not selected, make saturated
                if (int(tileInformation.a * 256) % HIGHLIGHT_FLAG != 0 || tileInformation.a == 0) {
                    float2 grayScale = 0.33 * (col.r + col.g + col.b);
                    col.rgb = lerp(col.rgb, grayScale.xxx, _GridOverlayDesaturation);
                }

                float _GridOverlayRulerLineWidth_final = _GridOverlayRulerLineWidth * line_scale;
                // different outlines for measurement
                if ((tilePos.x % _GridOverlayRulerTiles == 0 && localPixel.x < _GridOverlayRulerLineWidth_final) ||
                    ((tilePos.x + 1) % _GridOverlayRulerTiles == 0 && localPixel.x >= PIXELS_PER_TILE - _GridOverlayRulerLineWidth_final) ||
                    (tilePos.y % _GridOverlayRulerTiles == 0 && localPixel.y < _GridOverlayRulerLineWidth_final) ||
                    ((tilePos.y + 1) % _GridOverlayRulerTiles == 0 && localPixel.y >= PIXELS_PER_TILE - _GridOverlayRulerLineWidth_final))
                    col = _GridOverlayRulerColor;

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

            float2 GetFirstTileUV(int tileType, float2 localUV) {
                // get the correct tileset
                float xuvDim = float(1) / TILES_X;
                float yuvDim = float(1) / TILES_Y;

                float yOffset = TILES_Y - 1 - (tileType - 1) + localUV.y;
                return float2(xuvDim * localUV.x, yuvDim * yOffset);
            }

            float _TileNoiseDistribution;

            float _BlendBorderWidth;
            float _BlendUVScale;
            float _BlendBorderNoiseThreshold;
            float _RoundingDist;

            float4 frag(v2f i) : SV_Target
            {
                float4 col = 1;
                int2 tilePos = int2(i.worldPos.xy);
                float2 localUV = frac(i.worldPos.xy);
                int2 localPixel = localUV * PIXELS_PER_TILE;

                // r: tile type, a: flag information
                float4 tileInformation = tex2D(_GridInfoTex, float2(tilePos) / _GridTexDim);

                // create local matrix for edge detections
                // Tile Type Matrix: contains tile type of surrounding tiles
                int3x3 ttm =
                {
                    0, 0, 0,
                    0, 0, 0,
                    0, 0, 0
                };

                // set matrix values
                ttm[0][0] = tex2D(_GridInfoTex, float2(tilePos + int2(-1, -1)) / _GridTexDim).r * 256;
                ttm[0][1] = tex2D(_GridInfoTex, float2(tilePos + int2(0, -1)) / _GridTexDim).r * 256;
                ttm[0][2] = tex2D(_GridInfoTex, float2(tilePos + int2(1, -1)) / _GridTexDim).r * 256;

                ttm[1][0] = tex2D(_GridInfoTex, float2(tilePos + int2(-1, 0)) / _GridTexDim).r * 256;
                ttm[1][1] = tex2D(_GridInfoTex, float2(tilePos + int2(0, 0)) / _GridTexDim).r * 256;
                ttm[1][2] = tex2D(_GridInfoTex, float2(tilePos + int2(1, 0)) / _GridTexDim).r * 256;

                ttm[2][0] = tex2D(_GridInfoTex, float2(tilePos + int2(-1, 1)) / _GridTexDim).r * 256;
                ttm[2][1] = tex2D(_GridInfoTex, float2(tilePos + int2(0, 1)) / _GridTexDim).r * 256;
                ttm[2][2] = tex2D(_GridInfoTex, float2(tilePos + int2(1, 1)) / _GridTexDim).r * 256;

                float2 tileUVUnit = float2(float(1) / TILES_X, float(1) / TILES_Y);
                float2 firstTileUV = GetFirstTileUV(ttm[1][1], localUV);
                float4 tile = tex2D(_TileAtlas, firstTileUV);

                col = tile;
                
                // blend tiles
                // get noise uv based on world position and pixelate it
                float2 noiseUV = float2(int2(frac(i.worldPos.xy) * PIXELS_PER_TILE)) / PIXELS_PER_TILE * _BlendUVScale;
                // used to remove artifacts from areas on tile that are considered "rounded"
                float borderToNearestVertex = PIXELS_PER_TILE - sqrt(pow(_RoundingDist * PIXELS_PER_TILE, 2) - pow(PIXELS_PER_TILE / 2, 2));
                // left tile
                if (localPixel.x < _BlendBorderWidth && ttm[1][1] != TILE_TYPE_WALL && ttm[1][0] != TILE_TYPE_WALL && ttm[1][0] != TILE_TYPE_EMPTY && ttm[1][0] != TILE_TYPE_LIQUID) {
                    float4 leftTile = tex2D(_TileAtlas, GetFirstTileUV(ttm[1][0], localUV));
                    float dist = 1 - (float(_BlendBorderWidth) - localPixel.x) / _BlendBorderWidth;
                    col = tex2D(_NoiseTexture, noiseUV) > 0.5 - dist * 0.5 ? col : 
                        (((ttm[0][0] == ttm[1][1] && localPixel.y < borderToNearestVertex)
                            || (ttm[2][0] == ttm[1][1] && localPixel.y > PIXELS_PER_TILE - borderToNearestVertex))
                            && distance(localPixel, int2(32, 32)) > _RoundingDist * PIXELS_PER_TILE ? col : leftTile);
                }
                // right tile
                if (localPixel.x > PIXELS_PER_TILE - _BlendBorderWidth && ttm[1][1] != TILE_TYPE_WALL && ttm[1][2] != TILE_TYPE_WALL && ttm[1][2] != TILE_TYPE_EMPTY && ttm[1][2] != TILE_TYPE_LIQUID) {
                    float4 rightTile = tex2D(_TileAtlas, GetFirstTileUV(ttm[1][2], localUV));
                    float dist = (PIXELS_PER_TILE - localPixel.x) / _BlendBorderWidth;
                    col = tex2D(_NoiseTexture, noiseUV) > 0.5 - dist * 0.5 ? col :
                        (((ttm[0][2] == ttm[1][1] && localPixel.y < borderToNearestVertex)
                            || (ttm[2][2] == ttm[1][1] && localPixel.y > PIXELS_PER_TILE - borderToNearestVertex))
                            && distance(localPixel, int2(32, 32)) > _RoundingDist * PIXELS_PER_TILE ? col : rightTile);
                }
                // bottom tile
                if (localPixel.y < _BlendBorderWidth && ttm[1][1] != TILE_TYPE_WALL && ttm[0][1] != TILE_TYPE_WALL && ttm[0][1] != TILE_TYPE_EMPTY && ttm[0][1] != TILE_TYPE_LIQUID) {
                    float4 bottomTile = tex2D(_TileAtlas, GetFirstTileUV(ttm[0][1], localUV));
                    float dist = 1 - (float(_BlendBorderWidth) - localPixel.y) / _BlendBorderWidth;
                    col = tex2D(_NoiseTexture, noiseUV) > 0.5 - dist * 0.5 ? col :
                        (((ttm[0][0] == ttm[1][1] && localPixel.x < borderToNearestVertex)
                            || (ttm[0][2] == ttm[1][1] && localPixel.x > PIXELS_PER_TILE - borderToNearestVertex))
                            && distance(localPixel, int2(32, 32)) > _RoundingDist * PIXELS_PER_TILE ? col : bottomTile);
                }
                // top tile
                if (localPixel.y > PIXELS_PER_TILE - _BlendBorderWidth && ttm[1][1] != TILE_TYPE_WALL && ttm[2][1] != TILE_TYPE_WALL && ttm[2][1] != TILE_TYPE_EMPTY && ttm[2][1] != TILE_TYPE_LIQUID) {
                    float4 topTile = tex2D(_TileAtlas, GetFirstTileUV(ttm[2][1], localUV));
                    float dist = (PIXELS_PER_TILE - localPixel.y) / _BlendBorderWidth;
                    col = tex2D(_NoiseTexture, noiseUV) > 0.5 - dist * 0.5 ? col :
                        (((ttm[2][0] == ttm[1][1] && localPixel.x < borderToNearestVertex)
                            || (ttm[2][2] == ttm[1][1] && localPixel.x > PIXELS_PER_TILE - borderToNearestVertex))
                            && distance(localPixel, int2(32, 32)) > _RoundingDist * PIXELS_PER_TILE ? col : topTile);
                }
                // corner rounding
                // bottom left
                if (ttm[1][0] == ttm[0][1] && ttm[1][0] != ttm[1][1] &&
                    ttm[1][1] != TILE_TYPE_WALL && ttm[0][2] != TILE_TYPE_WALL &&
                    ttm[1][0] != TILE_TYPE_LIQUID) {
                    float4 bottomLeftCorner = tex2D(_TileAtlas, GetFirstTileUV(ttm[1][0], localUV));
                    col = distance(localPixel, int2(32, 32)) > _RoundingDist * PIXELS_PER_TILE && localUV.x < 0.5 && localUV.y < 0.5 ? bottomLeftCorner : col;
                }
                // bottom right
                if (ttm[1][2] == ttm[0][1] && ttm[1][2] != ttm[1][1] &&
                    ttm[1][1] != TILE_TYPE_WALL && ttm[1][2] != TILE_TYPE_WALL &&
                    ttm[1][2] != TILE_TYPE_LIQUID) {
                    float4 bottomRightCorner = tex2D(_TileAtlas, GetFirstTileUV(ttm[1][2], localUV));
                    col = distance(localPixel, int2(32, 32)) > _RoundingDist * PIXELS_PER_TILE && localUV.x > 0.5 && localUV.y < 0.5 ? bottomRightCorner : col;
                }
                // top left
                if (ttm[1][0] == ttm[2][1] && ttm[1][0] != ttm[1][1] &&
                    ttm[1][1] != TILE_TYPE_WALL && ttm[1][0] != TILE_TYPE_WALL &&
                    ttm[1][0] != TILE_TYPE_LIQUID) {
                    float4 topLeftCorner = tex2D(_TileAtlas, GetFirstTileUV(ttm[2][1], localUV));
                    col = distance(localPixel, int2(32, 32)) > _RoundingDist * PIXELS_PER_TILE && localUV.x < 0.5 && localUV.y > 0.5 ? topLeftCorner : col;
                }
                // top right
                if (ttm[1][2] == ttm[2][1] && ttm[1][2] != ttm[1][1] &&
                    ttm[1][1] != TILE_TYPE_WALL && ttm[1][2] != TILE_TYPE_WALL &&
                    ttm[1][2] != TILE_TYPE_LIQUID) {
                    float4 topRightCorner = tex2D(_TileAtlas, GetFirstTileUV(ttm[1][2], localUV));
                    col = distance(localPixel, int2(32, 32)) > _RoundingDist * PIXELS_PER_TILE && localUV.x > 0.5 && localUV.y > 0.5 ? topRightCorner : col;
                }
                
                // use tilenoise to add props
                float tileNoise = tex2D(_NoiseTexture, float2(tilePos) / _TileNoiseDistribution);
                // yay magic numbers
                // would generate my own perlin noise but using quick texture for now
                tileNoise = clamp((tileNoise - 0.75) * 2 + 1, 0, 1);
                float2 propUV = float2((float(int((tileNoise - _TilePropSparsity) / (1 - _TilePropSparsity) * PROPS_X)) + localUV.x) / PROPS_X, firstTileUV.y);
                float4 prop = tex2D(_PropAtlas, propUV);

                col = tileNoise < _TilePropSparsity ? col : lerp(col, prop, prop.a);
                
                // add borders
                // edges first
                if (tilePos.x == 0 || ttm[1][0] == TILE_TYPE_LIQUID || ttm[1][0] == TILE_TYPE_EMPTY || 
                    (ttm[1][1] == TILE_TYPE_WALL && ttm[1][0] != TILE_TYPE_WALL)) {
                    float4 leftBar = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * V_LIQUID_BORDER, 0));

                    if (localUV.x < 0.5)
                        col = lerp(col, leftBar, leftBar.a);
                }
                if (tilePos.x == _GridTexDim.x - 1 || ttm[1][2] == TILE_TYPE_LIQUID || ttm[1][2] == TILE_TYPE_EMPTY ||
                    (ttm[1][1] == TILE_TYPE_WALL && ttm[1][2] != TILE_TYPE_WALL)) {
                    float4 rightBar = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * V_LIQUID_BORDER, 0));

                    if (localUV.x > 0.5)
                        col = lerp(col, rightBar, rightBar.a);
                }
                if (tilePos.y == 0 || ttm[0][1] == TILE_TYPE_LIQUID || ttm[0][1] == TILE_TYPE_EMPTY ||
                    (ttm[1][1] == TILE_TYPE_WALL && ttm[0][1] != TILE_TYPE_WALL)) {
                    float4 bottomBar = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * H_LIQUID_BORDER, 0));

                    if (localUV.y < 0.5)
                        col = lerp(col, bottomBar, bottomBar.a);
                }
                if (tilePos.y == _GridTexDim.y - 1 || ttm[2][1] == TILE_TYPE_LIQUID || ttm[2][1] == TILE_TYPE_EMPTY ||
                    (ttm[1][1] == TILE_TYPE_WALL && ttm[2][1] != TILE_TYPE_WALL)) {
                    float4 topBar = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * H_LIQUID_BORDER, 0));

                    if (localUV.y > 0.5)
                        col = lerp(col, topBar, topBar.a);
                }

                
                // corners after
                // outer corners
                if ((tilePos.x == 0 && tilePos.y == 0) ||
                    (ttm[1][0] == TILE_TYPE_LIQUID && ttm[0][1] == TILE_TYPE_LIQUID) ||
                    (ttm[1][0] == TILE_TYPE_EMPTY && ttm[0][1] == TILE_TYPE_EMPTY)) {
                    float4 blCorner = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * OUTER_CORNER, 0));

                    if (localUV.x < 0.5 && localUV.y < 0.5)
                        col = lerp(col, blCorner, blCorner.a);
                }
                if ((tilePos.x == _GridTexDim.x && tilePos.y == 0) ||
                    (ttm[1][2] == TILE_TYPE_LIQUID && ttm[0][1] == TILE_TYPE_LIQUID) ||
                    (ttm[1][2] == TILE_TYPE_EMPTY && ttm[0][1] == TILE_TYPE_EMPTY)) {
                    float4 brCorner = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * OUTER_CORNER, 0));

                    if (localUV.x > 0.5 && localUV.y < 0.5)
                        col = lerp(col, brCorner, brCorner.a);
                }
                if ((tilePos.x == 0 && tilePos.y == _GridTexDim.y) ||
                    (ttm[1][0] == TILE_TYPE_LIQUID && ttm[2][1] == TILE_TYPE_LIQUID) ||
                    (ttm[1][0] == TILE_TYPE_EMPTY && ttm[2][1] == TILE_TYPE_EMPTY)) {
                    float4 tlCorner = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * OUTER_CORNER, 0));

                    if (localUV.x < 0.5 && localUV.y > 0.5)
                        col = lerp(col, tlCorner, tlCorner.a);
                }
                if ((tilePos.x == _GridTexDim.x && tilePos.y == _GridTexDim.y) ||
                    (ttm[1][2] == TILE_TYPE_LIQUID && ttm[2][1] == TILE_TYPE_LIQUID) ||
                    (ttm[1][2] == TILE_TYPE_EMPTY && ttm[2][1] == TILE_TYPE_EMPTY)) {
                    float4 trCorner = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * OUTER_CORNER, 0));

                    if (localUV.x > 0.5 && localUV.y > 0.5)
                        col = lerp(col, trCorner, trCorner.a);
                }
                
                // inner corners
                if ((ttm[0][0] == TILE_TYPE_LIQUID && ttm[0][1] != TILE_TYPE_LIQUID && ttm[1][0] != TILE_TYPE_LIQUID && ttm[1][1] != TILE_TYPE_WALL) ||
                    (ttm[0][0] == TILE_TYPE_EMPTY && ttm[0][1] != TILE_TYPE_EMPTY && ttm[1][0] != TILE_TYPE_EMPTY)) {
                    float4 blInnerCorner = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * INNER_CORNER, 0));

                    if (localUV.x < 0.5 && localUV.y < 0.5)
                        col = lerp(col, blInnerCorner, blInnerCorner.a);
                }
                if ((ttm[0][2] == TILE_TYPE_LIQUID && ttm[0][1] != TILE_TYPE_LIQUID && ttm[1][2] != TILE_TYPE_LIQUID && ttm[1][1] != TILE_TYPE_WALL) ||
                    (ttm[0][2] == TILE_TYPE_EMPTY && ttm[0][1] != TILE_TYPE_EMPTY && ttm[1][2] != TILE_TYPE_EMPTY)) {
                    float4 brInnerCorner = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * INNER_CORNER, 0));

                    if (localUV.x > 0.5 && localUV.y < 0.5)
                        col = lerp(col, brInnerCorner, brInnerCorner.a);
                }
                if ((ttm[2][0] == TILE_TYPE_LIQUID && ttm[1][0] != TILE_TYPE_LIQUID && ttm[2][1] != TILE_TYPE_LIQUID && ttm[1][1] != TILE_TYPE_WALL) ||
                    (ttm[2][0] == TILE_TYPE_EMPTY && ttm[1][0] != TILE_TYPE_EMPTY && ttm[2][1] != TILE_TYPE_EMPTY)) {
                    float4 tlInnerCorner = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * INNER_CORNER, 0));

                    if (localUV.x < 0.5 && localUV.y > 0.5)
                        col = lerp(col, tlInnerCorner, tlInnerCorner.a);
                }
                if ((ttm[2][2] == TILE_TYPE_LIQUID && ttm[2][1] != TILE_TYPE_LIQUID && ttm[1][2] != TILE_TYPE_LIQUID && ttm[1][1] != TILE_TYPE_WALL) ||
                    (ttm[2][2] == TILE_TYPE_EMPTY && ttm[2][1] != TILE_TYPE_EMPTY && ttm[1][2] != TILE_TYPE_EMPTY)) {
                    float4 trInnerCorner = tex2D(_TileAtlas, firstTileUV + float2(tileUVUnit.x * INNER_CORNER, 0));

                    if (localUV.x > 0.5 && localUV.y > 0.5)
                        col = lerp(col, trInnerCorner, trInnerCorner.a);
                }

                // add liquid and other animated tiles first
                if (ttm[1][1] == 7)
                    col = AddLiquid(col, localPixel, i.worldPos.xy);
                
                // then add color modifier
                if (i.color.r + i.color.g + i.color.b != 3)
                    col.rgb = col.rgb * (col.a / (col.a + i.color.a)) + i.color.rgb * (i.color.a / (col.a + i.color.a));

                // create grid
                if (_GridOverlayToggle > 0)
                    col = AddGrid(col, localUV * PIXELS_PER_TILE, tilePos, tileInformation);

                return col;
            }
            ENDCG
        }
    }
}
