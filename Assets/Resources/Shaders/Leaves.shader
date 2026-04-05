Shader "Custom/Leaves"
{
    Properties
    {
        _MainTex ("Color Texture", 2D) = "white" {}
        _AlphaTex ("Alpha Texture", 2D) = "white" {}
        _UVScale ("Alpha UV Scale", Float) = 1.0
        _AlphaChannel ("Alpha Channel (0=R,1=G,2=B,3=A)", Int) = 3
        _AlphaCutoff ("Alpha Cutoff (для глубины)", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        Pass
        {
            Name "DepthPass"
            ZWrite On
            ColorMask 0
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_depth
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
            sampler2D _AlphaTex;
            float _UVScale;
            int _AlphaChannel;
            float _AlphaCutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag_depth (v2f i) : SV_Target
            {
                float2 alphaUV = i.uv * _UVScale;
                fixed4 alphaTex = tex2D(_AlphaTex, alphaUV);
                float alphaValue;
                if (_AlphaChannel == 0) alphaValue = alphaTex.r;
                else if (_AlphaChannel == 1) alphaValue = alphaTex.g;
                else if (_AlphaChannel == 2) alphaValue = alphaTex.b;
                else alphaValue = alphaTex.a;
                
                // Если прозрачность выше порога – пишем глубину
                clip(alphaValue - _AlphaCutoff);
                return 0;
            }
            ENDCG
        }
        
        // Второй проход: обычное смешивание
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            
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
            sampler2D _AlphaTex;
            float _UVScale;
            int _AlphaChannel;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 alphaUV = i.uv * _UVScale;
                fixed4 alphaTex = tex2D(_AlphaTex, alphaUV);
                float alphaValue;
                if (_AlphaChannel == 0) alphaValue = alphaTex.r;
                else if (_AlphaChannel == 1) alphaValue = alphaTex.g;
                else if (_AlphaChannel == 2) alphaValue = alphaTex.b;
                else alphaValue = alphaTex.a;
                col.a = alphaValue;
                return col;
            }
            ENDCG
        }
    }
    Fallback "Unlit/Transparent"
}