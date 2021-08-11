Shader "Unlit/SkyboxShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("NoiseTex", 2D) = "white" {}
        _Scale ("Scale", Vector) = (1, 1, 1, 1)
        _Speed ("Speed", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            float4 _Scale;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 screenUV = i.screenPos.xy / i.screenPos.w;

                // get noise first
                float2 noiseUV = screenUV * _Scale.xy;
                // box out the background
                // [0... 1]
                noiseUV = float2(int2(noiseUV)) / _Scale.xy;
                float noise = frac(tex2D(_NoiseTex, noiseUV));// +_Time.x * _Speed);
                noise *= 8;
                noise = float(int(noise));

                // scale the uvs
                screenUV *= _Scale.xy;
                // tile the background
                screenUV = frac(screenUV); 
                screenUV.x *= noise;
                screenUV.x /= 9;

                
                fixed4 col = tex2D(_MainTex, screenUV);


                return col;
            }
            ENDCG
        }
    }
}
