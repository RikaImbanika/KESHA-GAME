Shader "Custom/SelfIlluminUnlitTintSingleSide"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Cull Back

        Pass
        {
            Tags { "LightMode"="Always" }

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
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, i.uv);
                return texCol * _Color;
            }
            ENDCG
        }
    }

    FallBack "Unlit/Texture"
}