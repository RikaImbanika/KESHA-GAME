Shader "Custom/UnlitF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
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
                float2 uv      : TEXCOORD0;
                float4 vertex  : SV_POSITION;
                float3 viewVec : TEXCOORD1;  // вектор от камеры к вершине в мировом пространстве (или view space)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _FogColor;
            float _FogDensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Получаем мировую позицию вершины
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // Вектор из камеры в вершину (в мировом пространстве)
                o.viewVec = worldPos - _WorldSpaceCameraPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // Расстояние от камеры до точки (длина viewVec) – плавная, без привязки к геометрии,
                // потому что интерполируется линейно в экранном пространстве.
                // Это имитирует туман, основанный на глубине, и даёт ровный эффект на плоских поверхностях.
                float dist = length(i.viewVec);

                float fogFactor = 1.0 - exp(-_FogDensity * dist);
                fogFactor = saturate(fogFactor);

                col.rgb = lerp(col.rgb, _FogColor.rgb, fogFactor * _FogColor.a);
                return col;
            }
            ENDCG
        }
    }
}