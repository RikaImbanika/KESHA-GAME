Shader "Custom/PortalShader"
{
    Properties
    {
        _MainTex ("2D Texture", 2D) = "white" {}
        _Background ("Background Color", Color) = (0,0,0,1)
        _RectX ("Rectangle X", Range(0,1)) = 0
        _RectY ("Rectangle Y", Range(0,1)) = 0
        _RectWidth ("Rectangle Width", Range(0,1)) = 1
        _RectHeight ("Rectangle Height", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off // Рисуем обе стороны полигонов
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _Background;
            float _RectX;
            float _RectY;
            float _RectWidth;
            float _RectHeight;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Получаем экранные координаты (0-1)
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                
                // Преобразуем экранные координаты в координаты прямоугольника
                float2 rectUV;
                rectUV.x = (screenUV.x - _RectX) / _RectWidth;
                rectUV.y = (screenUV.y - _RectY) / _RectHeight;

                //rectUV.x = screenUV.x * _RectWidth + _RectX;
                //rectUV.y = screenUV.y * _RectHeight + _RectY;
                
                // Проверяем, находится ли точка внутри прямоугольника
                fixed4 col;
                if (rectUV.x >= 0 && rectUV.x <= 1 && rectUV.y >= 0 && rectUV.y <= 1)
                {
                    // Внутри прямоугольника - берем тексель из текстуры
                    col = tex2D(_MainTex, rectUV);
                    
                    // Смешиваем с фоном используя альфу текстуры
                    col.rgb = lerp(_Background.rgb, col.rgb, col.a);
                }
                else
                {
                    // Вне прямоугольника - только цвет фона
                    col.rgb = _Background.rgb;
                }
                
                col.a = 1.0; // Полная непрозрачность
                
                return col;
            }
            ENDCG
        }
    }
}