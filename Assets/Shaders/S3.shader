Shader "Unlit/S3"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RectMin ("Rect Min", Vector) = (-1, -1, 0, 0)
        _RectMax ("Rect Max", Vector) = (1, 1, 0, 0)
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _RectMin;
            float2 _RectMax;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // Преобразуем UV из [0,1] в диапазон прямоугольника
                float2 rectSize = _RectMax - _RectMin;
                float2 correctedUV = _RectMin + v.uv * rectSize;
                
                // Применяем перспективную коррекцию
                // Здесь мы делаем обратное преобразование: точки, которые были на границах прямоугольника,
                // теперь становятся границами текстуры
                
                o.uv = correctedUV;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Сэмплируем текстуру с скорректированными UV
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}