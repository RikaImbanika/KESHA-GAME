Shader "Custom/Tex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _ScrollX ("Scroll Speed X", Float) = 0.1
        _ScrollY ("Scroll Speed Y", Float) = 0.0
        _Center ("Mask Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _RadialMaskPower ("Radial Mask Power", Float) = 1.0   // чем выше, тем резче переход у краёв
        _AlphaMultiplier ("Alpha Multiplier", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _ScrollX;
            float _ScrollY;
            float2 _Center;
            float _RadialMaskPower;
            float _AlphaMultiplier;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Линейное смещение UV во времени с зацикливанием
                float2 scrollingUV = i.uv + float2(_ScrollX, _ScrollY) * _Time.y;
                scrollingUV = frac(scrollingUV); // зацикливание

                fixed4 texColor = tex2D(_MainTex, scrollingUV);

                // Расстояние от текущей точки до центра маски (в UV-пространстве)
                float2 offset = i.uv - _Center;
                float radius = length(offset);

                // Максимальное возможное расстояние от центра до угла прямоугольника
                float2 maxOffset = max(abs(_Center), abs(1.0 - _Center));
                float maxDist = length(maxOffset);
                radius = saturate(radius / maxDist); // теперь радиус в [0,1]

                // Маска прозрачности: 1 в центре, 0 на краях
                float alphaMask = pow(1.0 - radius, _RadialMaskPower);

                // Итоговый цвет
                fixed4 finalColor = texColor * _Color;
                finalColor.a *= alphaMask * _AlphaMultiplier;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}