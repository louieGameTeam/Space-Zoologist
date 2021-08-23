Shader "Unlit/AnimalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _LightColor("Light Color", COLOR) = (1, 1, 1, 1)
        _LightDirection("Light Direction", Vector) = (0, 1, 0, 0)
        _ShadowArea("Shadow Area", float) = 1
        _ShadowScaleMin("Shadow Scale Minimum", float) = 1
        _ShadowScaleMax("Shadow Scale Maximum", float) = 1
        _ShadowOffset("Shadow Offset", Vector) = (0, 0, 0, 0)
        _Pixelation("Pixelation", int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 centerUV : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _Max_UV;
            float2 _Min_UV;

            float4 _LightColor;
            float4 _LightDirection;
            float _ShadowArea;
            float _ShadowScaleMin;
            float _ShadowScaleMax;
            float4 _ShadowOffset;
            int _Pixelation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex * _ShadowArea);
                o.uv = v.uv;
                o.centerUV = (_Max_UV + _Min_UV) / 2;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                i.uv = (i.uv - i.centerUV) * _ShadowArea + i.centerUV;

                fixed4 col = tex2D(_MainTex, i.uv) * _LightColor * i.color;

                // transform shadow uv
                float2 lightDirection = normalize(_LightDirection.xy);

                // pixelate the shadow first
                float2 pixelUV = float2(int2(i.uv * _Pixelation)) / _Pixelation;
                float2 shadowUV = pixelUV;
                shadowUV -= _Min_UV;
                
                float2x2 xShearMat = {
                    1, 0,
                    lightDirection.x, 1
                };

                float yScale = lerp(_ShadowScaleMax, _ShadowScaleMin, 1 - abs(lightDirection.x));
                float2x2 yScaleMat = {
                    1, 0,
                    0, yScale
                };

                // apply shear
                shadowUV = mul(shadowUV, xShearMat);
                // apply scale
                shadowUV = mul(shadowUV, yScaleMat);

                shadowUV += _Min_UV + _ShadowOffset.xy;

                fixed4 shadow = tex2D(_MainTex, shadowUV);
                shadow.rgb = 0;

                // remove rest of the sprite atlas
                if (abs(i.uv.x - i.centerUV.x) > _Max_UV.x - i.centerUV.x ||
                    abs(i.uv.y - i.centerUV.y) > _Max_UV.y - i.centerUV.y) {
                    col.a = 0;
                }
                
                // do the same for the transformed shadow
                
                if (shadowUV.y < _Min_UV.y ||
                    shadowUV.y > _Max_UV.y ||
                    shadowUV.x < _Min_UV.x ||
                    shadowUV.x > _Max_UV.x){
                    shadow.a = 0;
                }

                // add the shadow through lerp
                col = lerp(shadow, col, col.a);

                return col;
            }
            ENDCG
        }
    }
}
