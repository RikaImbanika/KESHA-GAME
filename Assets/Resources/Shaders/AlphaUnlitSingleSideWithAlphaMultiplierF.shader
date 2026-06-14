// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

Shader "Custom/AlphaUnlitSingleSideWithAlphaMultiplierF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Alpha ("Alpha (from code)", Range(0,1)) = 1
        [HDR] _FogColor ("Fog Color", Color) = (0.5, 0.6, 0.7, 1.0)
        _FogDensity ("Fog Density", Range(0, 0.1)) = 0.02
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float3 viewVec : TEXCOORD1; // world-space vector from camera to vertex
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Alpha;
            float4 _FogColor;
            float _FogDensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewVec = worldPos - _WorldSpaceCameraPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
                fixed4 col = tex;
                col.a *= _Alpha;

                // Exponential fog
                float dist = length(i.viewVec);
                float fogFactor = 1.0 - exp(-_FogDensity * dist);
                fogFactor = saturate(fogFactor);
                col.rgb = lerp(col.rgb, _FogColor.rgb, fogFactor * _FogColor.a);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}