Shader "Custom/HueShiftUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed (cycles per second)", Float) = 0.1667   // 1/6 ≈ 0.1667 → полный круг за 6 сек
        _HueOffset ("Hue Offset", Range(0,1)) = 0.0            // ручная подстройка начальной фазы
        _Saturation ("Saturation", Range(0,1)) = 1.0           // опционально: регулировка насыщенности
        _Value ("Value", Range(0,1)) = 1.0                     // опционально: регулировка яркости
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
            float _Speed;
            float _HueOffset;
            float _Saturation;
            float _Value;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Преобразование RGB -> HSV
            float3 rgb2hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            // Преобразование HSV -> RGB
            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Основной цвет текстуры
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Перевод в HSV
                float3 hsv = rgb2hsv(col.rgb);
                
                // Вычисляем сдвиг оттенка от времени
                // _Time.y — время в секундах с начала игры
                float timeShift = frac(_Time.y * _Speed + _HueOffset);
                
                // Применяем сдвиг к оттенку (циклически)
                hsv.x = frac(hsv.x + timeShift);
                
                // Применяем пользовательские настройки насыщенности и яркости
                hsv.y *= _Saturation;
                hsv.z *= _Value;
                
                // Обратно в RGB
                float3 rgb = hsv2rgb(hsv);
                
                // Возвращаем цвет с сохранением альфа-канала
                return fixed4(rgb, col.a);
            }
            ENDCG
        }
    }
}