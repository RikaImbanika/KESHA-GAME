// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

Shader "Custom/Skybox/SkyHemisphereMirror" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Rotation ("Rotation", Range(0, 360)) = 0
        _VMin ("V Min (горизонт)", Range(0, 1)) = 0
        _VMax ("V Max (зенит)", Range(0, 1)) = 1.0
        [Toggle] _MirrorHorizontal ("Mirror Horizontally", Float) = 1
    }
    SubShader {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _MIRRORHORIZONTAL_ON
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 vertex : SV_POSITION;
                float3 worldDir : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Rotation;
            float _VMin;
            float _VMax;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldDir = normalize(v.vertex.xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 dir = i.worldDir;
                
                float rad = _Rotation * (UNITY_PI / 180.0);
                float cosA = cos(rad);
                float sinA = sin(rad);
                float3 rotatedDir;
                rotatedDir.x = dir.x * cosA - dir.z * sinA;
                rotatedDir.z = dir.x * sinA + dir.z * cosA;
                rotatedDir.y = dir.y;

                float lon = atan2(rotatedDir.x, rotatedDir.z);
                float lat = asin(rotatedDir.y);
                
                float u = (lon / (2.0 * UNITY_PI)) + 0.5;
                
                float absLat = abs(lat);
                
                float v = _VMin + (absLat / (UNITY_PI / 2.0)) * (_VMax - _VMin);
                
                #ifdef _MIRRORHORIZONTAL_ON
                
                if (lat < 0) {
                    u = 1.0 - u;
                }
                #endif

                fixed4 col = tex2D(_MainTex, float2(u, v));
                return col;
            }
            ENDCG
        }
    }
}