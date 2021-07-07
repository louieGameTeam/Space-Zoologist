Shader "Unlit/TileShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData]_GridInformationTexture ("Grid Information Texture", 2D) = "white" {}
        _NoiseTexture("Noise Texture", 2D) = "white" {}
        
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
            #define PIXELS_PER_TILE 32
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

            v2f vert (appdata v)
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
                if (int(tileInformation.a * 256) % HIGHLIGHT_FLAG == 0 && tileInformation.a != 0)
                    col.rgb *= tileInformation.rgb;
                else if (int(tileInformation.a) % HIGHLIGHT_FLAG != 0 || tileInformation.a == 0) {
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

                col = lerp(liquid, col, col.a);

                return col;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                int2 tilePos = int2(i.worldPos.xy);
                float2 localUV = frac(i.worldPos.xy);
                int2 localPixel = localUV * PIXELS_PER_TILE;

                // rgb color, alpha mask
                float4 tileInformation = tex2D(_GridInformationTexture, float2(tilePos) / _GridTextureDimensions);

                // add liquid and other animated tiles first
                if (int(tileInformation.a * 256) % LIQUID_FLAG == 0 && tileInformation.a != 0)
                    col = AddLiquid(col, localPixel, i.worldPos.xy);
                
                // then add color modifier
                col *= i.color;

                // create grid
                if (_GridOverlayToggle > 0)
                    col = AddGrid(col, localPixel, tilePos, tileInformation);

                return col;
            }
            ENDCG
        }
    }
}
