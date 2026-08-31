// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

Shader "ClothesF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ClothMap ("Cloth Map", 2D) = "white" {}
        _HueShift1 ("Hue Shift 1", Float) = 0
        _HueShift2 ("Hue Shift 2", Float) = 0
        _HueShift3 ("Hue Shift 3", Float) = 0
        [HDR] _FogColor ("Fog Color", Color) = (0.5,0.6,0.7,1)
        _FogDensity ("Fog Density", Range(0, 0.1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
                float2 uv       : TEXCOORD0;
                float2 clothUV  : TEXCOORD1;
                float4 vertex   : SV_POSITION;
                float3 viewVec  : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _ClothMap;
            float4 _ClothMap_ST;
            float4 _Color;

            float _HueShift1;
            float _HueShift2;
            float _HueShift3;

            float4 _FogColor;
            float _FogDensity;

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
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float3 applyHueShift(float3 color, float shift)
            {
                if (abs(shift) < 0.00001) return color;
                float3 hsv = rgb2hsv(color);
                hsv.x = frac(hsv.x + shift);
                return hsv2rgb(hsv);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.clothUV = TRANSFORM_TEX(v.uv, _ClothMap);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewVec = worldPos - _WorldSpaceCameraPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                fixed4 clothMask = tex2D(_ClothMap, i.clothUV);

                float totalHueShift = dot(clothMask.rgb, float3(_HueShift1, _HueShift2, _HueShift3));

                col.rgb = applyHueShift(col.rgb, totalHueShift);

                float dist = max(0.0, length(i.viewVec) - _ProjectionParams.y);
                float fogFactor = 1.0 - exp(-_FogDensity * dist);
                fogFactor = saturate(fogFactor);
                col.rgb = lerp(col.rgb, _FogColor.rgb, fogFactor * _FogColor.a);

                return col;
            }
            ENDCG
        }
    }
}