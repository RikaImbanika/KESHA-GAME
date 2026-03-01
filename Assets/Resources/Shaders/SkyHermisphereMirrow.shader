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
//
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
                
                // Поворот вокруг Y
                float rad = _Rotation * (UNITY_PI / 180.0);
                float cosA = cos(rad);
                float sinA = sin(rad);
                float3 rotatedDir;
                rotatedDir.x = dir.x * cosA - dir.z * sinA;
                rotatedDir.z = dir.x * sinA + dir.z * cosA;
                rotatedDir.y = dir.y;

                // Вычисляем широту и долготу
                float lon = atan2(rotatedDir.x, rotatedDir.z);
                float lat = asin(rotatedDir.y);
                
                // Нормируем долготу в [0,1]
                float u = (lon / (2.0 * UNITY_PI)) + 0.5;
                
                // Для верхней полусферы (y>=0) используем lat напрямую
                // Для нижней (y<0) берём абсолютную широту (отражение)
                float absLat = abs(lat);
                
                // Преобразуем широту в v-координату в диапазоне [_VMin, _VMax]
                // При absLat = 0 (горизонт) -> v = _VMin
                // При absLat = PI/2 (зенит/надир) -> v = _VMax
                float v = _VMin + (absLat / (UNITY_PI / 2.0)) * (_VMax - _VMin);
                
                #ifdef _MIRRORHORIZONTAL_ON
                // Для нижней полусферы инвертируем u (горизонтальное зеркало)
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