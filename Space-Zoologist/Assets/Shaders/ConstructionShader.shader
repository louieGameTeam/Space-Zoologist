Shader "Unlit/ConstructionShader"
{
    Properties
    {
        _1TileAnimationTex("One Day Tile Animation Texture", 2D) = "white" {}
        _1TileTex("One Day Tile Texture", 2D) = "white" {}

        _PlantAnimationTex("Plant Animation Texture", 2D) = "white" {}
        
        _2PlantGradientTex("Two Day Plant Gradient Texture", 2D) = "white" {}
        _3PlantGradientTex("Three Day Plant Gradient Texture", 2D) = "white" {}

        _PlantBorderTex("Plant Border Texture", 2D) = "white" {}

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

            sampler2D _PlantAnimationTex;
            float4 _PlantAnimationTex_ST;
            sampler2D _PlantBorderTex;
            float4 _PlantBorderTex_ST;

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
                // if it is a food source
                if (bufferType == BUFFER_TYPE_TREE || bufferType == BUFFER_TYPE_ONEFOOD) {
                    // send it to the right place in the texture

                    // first find the tile position based on the height and width
                    float4 foodInfo = tex2D(_CenterTex, i.uv);
                    int tileNumber = foodInfo.r * FLAG_MULTIPLIER - 1;
                    int2 plantDim = int2(foodInfo.g * FLAG_MULTIPLIER, foodInfo.b * FLAG_MULTIPLIER);

                    // bottom left corner of the animation
                    int2 plantLocalTile = int2(tileNumber % plantDim.x, plantDim.y == 1 ? 0 : tileNumber / plantDim.y);
                    float2 plantLocalUV = float2(0, 0);
                    float2 plantTexPosUV = float2(0, 0);
                    // position the animation properly at the center
                    // if square
                    if (plantDim.x == plantDim.y) {
                        plantLocalUV = ((float2) plantLocalTile + localUV) / (float2) plantDim;
                    }
                    // if width is greater than height
                    if (plantDim.x > plantDim.y) {
                        plantLocalUV = ((float2) plantLocalTile + localUV) / (float2) plantDim;
                        plantLocalUV.x = (plantLocalUV.x - 0.5) * plantDim.x / plantDim.y + 0.5;
                        if (plantLocalUV.x < 0 || plantLocalUV.x > 1)
                            plantLocalUV.x = 0;
                    }
                    // if height is greater than width
                    if (plantDim.y > plantDim.x) {
                        plantLocalUV = ((float2) plantLocalTile + localUV) / (float2) plantDim;
                        plantLocalUV.y = (plantLocalUV.y - 0.5) * plantDim.y / plantDim.x + 0.5;
                        if (plantLocalUV.y < 0 || plantLocalUV.y > 1)
                            plantLocalUV.y = 0;
                    }

                    // add it if there is a construction buffer there
                    if (total >= 1) {
                        plantTexPosUV = float2(float(1) / PLANT_ANIMATION_FRAMES * (plantLocalUV.x + frame), (plantLocalUV.y + total - 1) / 3);
                        col = tex2D(_PlantAnimationTex, plantTexPosUV);

                    }
                    // find the right gradient for the food source amount of days
                    if (total == 2) {
                        gradient = tex2D(_2PlantGradientTex, plantLocalUV);
                        if (total * gradient.r < progress && gradient.a > 0)
                            col += 1;
                    }
                    if (total == 3) {
                        gradient = tex2D(_3PlantGradientTex, plantLocalUV);
                        if (total * gradient.r < progress && gradient.a > 0)
                            col += 1;
                    }

                    // add the corners at the end
                    // bottom left corner
                    if (tileNumber == 0) {
                        if (localUV.x < 0.5 && localUV.y < 0.5) {
                            float4 borderColor = tex2D(_PlantBorderTex, localUV);
                            col = borderColor.a > 0 ? borderColor : col;
                        }
                    }
                    // bottom right corner
                    if (tileNumber == plantDim.x - 1 && plantDim.x >= 1) {
                        if (localUV.x > 0.5 && localUV.y < 0.5) {
                            float4 borderColor = tex2D(_PlantBorderTex, localUV);
                            col = borderColor.a > 0 ? borderColor : col;
                        }
                    }
                    // upper left corner
                    if (tileNumber == plantDim.x * plantDim.y - plantDim.x) {
                        if (localUV.x < 0.5 && localUV.y > 0.5) {
                            float4 borderColor = tex2D(_PlantBorderTex, localUV);
                            col = borderColor.a > 0 ? borderColor : col;
                        }
                    }
                    // upper right corner
                    if (tileNumber == plantDim.x * plantDim.y - 1 && plantDim.x >= 1) {
                        if (localUV.x > 0.5 && localUV.y > 0.5) {
                            float4 borderColor = tex2D(_PlantBorderTex, localUV);
                            col = borderColor.a > 0 ? borderColor : col;
                        }
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
