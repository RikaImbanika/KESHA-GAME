Shader "Custom/PortalShaderF"
{
    Properties
    {
        _MainTex ("2D Texture", 2D) = "white" {}
        _Background ("Background Color", Color) = (0,0,0,1)
        _RectX ("Rectangle X", Range(0,1)) = 0
        _RectY ("Rectangle Y", Range(0,1)) = 0
        _RectWidth ("Rectangle Width", Range(0,1)) = 1
        _RectHeight ("Rectangle Height", Range(0,1)) = 1
        [HDR] _FogColor ("Fog Color", Color) = (0.5, 0.6, 0.7, 1.0)
        _FogDensity ("Fog Density", Range(0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
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
                float3 viewVec : TEXCOORD1;   // camera to surface in world space
            };
            
            sampler2D _MainTex;
            float4 _Background;
            float _RectX;
            float _RectY;
            float _RectWidth;
            float _RectHeight;
            float4 _FogColor;
            float _FogDensity;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewVec = worldPos - _WorldSpaceCameraPos;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                
                float2 rectUV;
                rectUV.x = (screenUV.x - _RectX) / _RectWidth;
                rectUV.y = (screenUV.y - _RectY) / _RectHeight;
                
                fixed4 col;
                if (rectUV.x >= 0 && rectUV.x <= 1 && rectUV.y >= 0 && rectUV.y <= 1)
                {
                    col = tex2D(_MainTex, rectUV);
                    col.rgb = lerp(_Background.rgb, col.rgb, col.a);
                }
                else
                {
                    col.rgb = _Background.rgb;
                }
                
                col.a = 1.0;

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
}