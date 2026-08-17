Shader "Custom/TMPUnlitF"
{
    Properties
    {
        _MainTex ("Font Atlas", 2D) = "white" {}
        _FaceColor ("Text Color", Color) = (1,1,1,1)

        // Параметры тумана
        [HDR] _FogColor ("Fog Color", Color) = (0.5,0.6,0.7,1)
        _FogDensity ("Fog Density", Range(0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Cull Back
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float3 viewVec : TEXCOORD1; // world-space vector from camera to vertex
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _FaceColor;
            float4 _FogColor;
            float _FogDensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _FaceColor;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewVec = worldPos - _WorldSpaceCameraPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Выборка SDF-атласа: альфа хранит дистанцию до края глифа
                fixed4 col = tex2D(_MainTex, i.uv);

                // Сглаживание на основе дистанции (стандартный приём TMP)
                float distance = col.a;
                float width = fwidth(distance);
                float alpha = smoothstep(0.5 - width, 0.5 + width, distance);

                // Применяем цвет вершины и цвет текста
                col.rgb = i.color.rgb;
                col.a = alpha * i.color.a;

                // Туман (та же формула, что и в Custom/UnlitF)
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