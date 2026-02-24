Shader "Hidden/PortalProjector"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
        Pass
        {
            ZWrite On
            ZTest LEqual  
            Blend Off
            Cull Off

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
                float4 pos : SV_POSITION;
                float4 uvProj : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4x4 _PortalVP;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uvProj = mul(_PortalVP, worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uvProj.xy / i.uvProj.w;
                uv = uv * 0.5 + 0.5;

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}