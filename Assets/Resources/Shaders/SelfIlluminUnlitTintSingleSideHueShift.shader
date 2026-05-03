Shader "Custom/SelfIlluminUnlitTintSingleSideHueShift"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Speed ("Speed (cycles per second)", Float) = 0.1667   // 1/6
        _HueOffset ("Hue Offset", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Cull Back

        Pass
        {
            Tags { "LightMode"="Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Speed;
            float _HueOffset;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 rgb2hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, i.uv);
                fixed4 baseCol = texCol * _Color;   // Tint Color как в шейдере 1

                float3 hsv = rgb2hsv(baseCol.rgb);

                float timeShift = frac(_Time.y * _Speed + _HueOffset);
                hsv.x = frac(hsv.x + timeShift);    // сдвиг тона

                float3 rgb = hsv2rgb(hsv);
                return fixed4(rgb, baseCol.a);
            }
            ENDCG
        }
    }

    FallBack "Unlit/Texture"
}