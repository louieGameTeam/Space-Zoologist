Shader "Unlit/TileShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GridSelectedTexture("Grid Selected Texture", 2D) = "white" {}
        _GridLiquidLocationTexture("Grid Liquid Location Texture", 2D) = "white" {}
        
        [Toggle]_GridOverlayToggle("Grid Overlay Toggle", float) = 0
        _GridOverlayLineWidth("Grid OverLay Line Width", Range(0, 32)) = 0
        _GridOverlayDesaturation("Grid Overlay Desaturation", Range(0, 1)) = 0
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

            sampler2D _GridSelectedTexture;
            float4 _GridSelectedTexture_ST;
            sampler2D _GridLiquidLocationTexture;
            float4 _GridLiquidLocationTexture_ST;

            float2 _GridTextureDimensions;

            float _GridOverlayToggle;
            int _GridOverlayLineWidth;
            float _GridOverlayDesaturation;

            float4 AddGrid(float4 col, float2 localPixel, int2 tilePos) {
                // add the outlines
                if ((localPixel.x < _GridOverlayLineWidth || localPixel.x >= PIXELS_PER_TILE - _GridOverlayLineWidth)
                    || (localPixel.y < _GridOverlayLineWidth || localPixel.y >= PIXELS_PER_TILE - _GridOverlayLineWidth))
                    col = 1;

                // use texture's alpha channel to figure out if this tile is selected or not
                float4 gridSelected = tex2D(_GridSelectedTexture, float2(tilePos) / _GridTextureDimensions);
                gridSelected.a = 1;

                // if not selected, make saturated
                if (gridSelected.a > 0) {
                    float2 grayScale = 0.33 * (col.r + col.g + col.b);
                    col.rgb = lerp(col.rgb, grayScale.xxx, _GridOverlayDesaturation);
                }

                return col;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv) * i.color;
                int2 tilePos = int2(i.worldPos.xy);
                float2 localUV = frac(i.worldPos.xy);
                int2 localPixel = localUV * PIXELS_PER_TILE;

                // create grid
                if (_GridOverlayToggle > 0)
                    col = AddGrid(col, localPixel, tilePos);

                return col;
            }
            ENDCG
        }
    }
}
