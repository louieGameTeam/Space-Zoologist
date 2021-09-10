Shader "Unlit/ConstructionShader"
{
    Properties
    {
        _1TileAnimationTex("One Day Tile Animation Texture", 2D) = "white" {}
        _1TileTex("One Day Tile Texture", 2D) = "white" {}

        _1PlantAnimationTex("One Day Plant Animation Texture", 2D) = "white" {}
        _2PlantAnimationTex("Two Day Plant Animation Texture", 2D) = "white" {}
        _3PlantAnimationTex("Three Day Plant Animation Texture", 2D) = "white" {}
        
        _2PlantGradientTex("Two Day Plant Gradient Texture", 2D) = "white" {}
        _3PlantGradientTex("Three Day Plant Gradient Texture", 2D) = "white" {}

        _AnimationSpeed("Animation Speed", float) = 20
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "PreviewType" = "Plane"}

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FLAG_MULTIPLIER 256
            #define BUFFER_TYPE_TREE 0
            #define BUFFER_TYPE_ONEFOOD 1
            #define BUFFER_TYPE_TILE 2
            #define TILE_ANIMATION_FRAMES 16
            #define PLANT_ANIMATION_FRAMES 82

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _CenterTex;
			float4 _CenterTex_ST;
            
            sampler2D _1TileAnimationTex;
            float4 _1TileAnimationTex_ST;
            sampler2D _1TileTex;
            float4 _1TileTex_ST;

            sampler2D _1PlantAnimationTex;
            float4 _1PlantAnimationTex_ST;
            sampler2D _2PlantAnimationTex;
            float4 _2PlantAnimationTex_ST;
            sampler2D _3PlantAnimationTex;
            float4 _3PlantAnimationTex_ST;

            sampler2D _2PlantGradientTex;
            float4 _2PlantGradientTex_ST;
            sampler2D _3PlantGradientTex;
            float4 _3PlantGradientTex_ST;

            float _AnimationSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                int2 tilePos = int2(i.worldPos.xy);
                float2 localUV = frac(i.worldPos.xy);

                float4 info = tex2D(_MainTex, i.uv);
                int bufferType = info.r * FLAG_MULTIPLIER;
                int progress = info.b * FLAG_MULTIPLIER;
                int total = info.a * FLAG_MULTIPLIER;

                float4 center = tex2D(_CenterTex, i.uv);

                float4 col = 0;

                // frame for animation
                int frame = _Time.y * _AnimationSpeed;
                // add the tile buffer if it is one
                if (bufferType == BUFFER_TYPE_TILE) {
                    if (total == 1) {
                        col = tex2D(_1TileTex, localUV);
                    }
                }

                float4 gradient = 0;
                if (bufferType == BUFFER_TYPE_TREE || bufferType == BUFFER_TYPE_ONEFOOD) {
                    float2 animationUV = float2(float(1) / PLANT_ANIMATION_FRAMES * (localUV.x + frame), localUV.y);
                    if (total == 1) {
                        col = tex2D(_1PlantAnimationTex, animationUV);
                    }
                    if (total == 2) {
                        col = tex2D(_2PlantAnimationTex, animationUV);
                        gradient = tex2D(_2PlantGradientTex, localUV);
                        if (total * gradient.r < progress && gradient.a > 0)
                            col += 1;
                    }
                    if (total == 3) {
                        col = tex2D(_3PlantAnimationTex, animationUV);
                        gradient = tex2D(_3PlantGradientTex, localUV);
                        if (total * gradient.r < progress && gradient.a > 0)
                            col += 1;
                    }
                }

                // add the center (tiles only for now)
                if (center.a == 1 && bufferType == BUFFER_TYPE_TILE) {
                    float2 animationUV = float2(float(1) / TILE_ANIMATION_FRAMES * (localUV.x + frame), localUV.y);

					col = tex2D(_1TileAnimationTex, animationUV);
                }

                return col;
            }
            ENDCG
        }
    }
}
