// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

Shader "Custom/LeavesF"
{
    Properties
    {
        _MainTex ("Color Texture", 2D) = "white" {}
        _AlphaTex ("Alpha Texture", 2D) = "white" {}
        _UVScale ("Alpha UV Scale", Float) = 1.0
        _AlphaChannel ("Alpha Channel (0=R,1=G,2=B,3=A)", Int) = 3
        _AlphaCutoff ("Alpha Cutoff (для глубины)", Range(0,1)) = 0.5
        [HDR] _FogColor ("Fog Color", Color) = (0.5, 0.6, 0.7, 1.0)
        _FogDensity ("Fog Density", Range(0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        Pass
        {
            Name "DepthPass"
            ZWrite On
            ColorMask 0
            Cull Off
    
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_depth
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
            };
            sampler2D _AlphaTex;
            float _UVScale;
            int _AlphaChannel;
            float _AlphaCutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag_depth (v2f i) : SV_Target
            {
                float2 alphaUV = i.uv * _UVScale;
                fixed4 alphaTex = tex2D(_AlphaTex, alphaUV);
                float alphaValue;
                if (_AlphaChannel == 0) alphaValue = alphaTex.r;
                else if (_AlphaChannel == 1) alphaValue = alphaTex.g;
                else if (_AlphaChannel == 2) alphaValue = alphaTex.b;
                else alphaValue = alphaTex.a;
                
                clip(alphaValue - _AlphaCutoff);
                return 0;
            }
            ENDCG
        }
        
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
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
                float3 viewVec : TEXCOORD1; // camera to vertex in world space
            };
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AlphaTex;
            float _UVScale;
            int _AlphaChannel;
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
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 alphaUV = i.uv * _UVScale;
                fixed4 alphaTex = tex2D(_AlphaTex, alphaUV);
                float alphaValue;
                if (_AlphaChannel == 0) alphaValue = alphaTex.r;
                else if (_AlphaChannel == 1) alphaValue = alphaTex.g;
                else if (_AlphaChannel == 2) alphaValue = alphaTex.b;
                else alphaValue = alphaTex.a;
                col.a = alphaValue;

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
    Fallback "Unlit/Transparent"
}