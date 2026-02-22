Shader "Custom/S2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondCameraTexture ("Second Camera Texture", 2D) = "white" {}
        _SecondObjectWorldToLocal ("Second Object World To Local", 2D) = "white" {} // Не используется напрямую, передаем через матрицу
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
                float4 screenPos : TEXCOORD1; // Позиция в экранных координатах второй камеры
                float3 worldPos : TEXCOORD2;   // Мировая позиция
            };

            // Основная текстура (для обычного рендера)
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            // Текстура со второй камеры
            sampler2D _SecondCameraTexture;
            
            // Матрица для преобразования из мировых координат в локальные координаты второго объекта
            float4x4 _SecondObjectWorldToLocal;
            
            // Матрица вида-проекции второй камеры
            float4x4 _SecondCameraVP;

            v2f vert (appdata v)
            {
                v2f o;
                
                // Стандартное преобразование вершин
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // Получаем мировую позицию текущего объекта
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldPos = worldPos;
                
                // 1) Преобразуем координаты текущего объекта в локальные координаты второго объекта
                float4 localPosInSecondObject = mul(_SecondObjectWorldToLocal, float4(worldPos, 1.0));
                
                // 2) Преобразуем локальные координаты второго объекта в экранные координаты второй камеры
                // Для этого умножаем на матрицу вида-проекции второй камеры
                float4 clipPos = mul(_SecondCameraVP, float4(worldPos, 1.0));
                
                // Переводим из Clip Space в Screen Space (от 0 до 1)
                float4 screenPos = ComputeScreenPos(clipPos);
                o.screenPos = screenPos;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 2) Проверяем видимость прямоугольника во второй камере
                // Нормализуем экранные координаты
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                
                // Проверяем, находится ли точка внутри экранного пространства (0-1)
                // и не отсечена ли по Z (между Near и Far плоскостями)
                bool isValidScreenUV = screenUV.x >= 0 && screenUV.x <= 1 && 
                                       screenUV.y >= 0 && screenUV.y <= 1;
                
                // 3) Получаем цвет со второй камеры
                fixed4 secondCameraColor = fixed4(0,0,0,1); // Черный по умолчанию (невидимо)
                
                if (isValidScreenUV)
                {
                    // Сэмплируем текстуру второй камеры
                    secondCameraColor = tex2D(_SecondCameraTexture, screenUV);
                }
                
                // Возвращаем цвет со второй камеры
                // Если нужно смешивать с основной текстурой, можно сделать:
                // fixed4 mainColor = tex2D(_MainTex, i.uv);
                // return mainColor * secondCameraColor; или lerp(mainColor, secondCameraColor, 0.5);
                
                return secondCameraColor;
            }
            ENDCG
        }
    }
}