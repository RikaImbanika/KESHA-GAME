Shader "Custom/ScrollTexF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _ScrollX ("Scroll Speed X", Float) = 0.1
        _ScrollY ("Scroll Speed Y", Float) = 0.0
        _Center ("Mask Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _RadialMaskPower ("Radial Mask Power", Float) = 1.0
        _AlphaMultiplier ("Alpha Multiplier", Float) = 1.0
        [HDR] _FogColor ("Fog Color", Color) = (0.5, 0.6, 0.7, 1.0)
        _FogDensity ("Fog Density", Range(0, 0.1)) = 0.02
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
                float3 viewVec : TEXCOORD1; // camera to vertex in world space
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _ScrollX;
            float _ScrollY;
            float2 _Center;
            float _RadialMaskPower;
            float _AlphaMultiplier;
            float4 _FogColor;
            float _FogDensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewVec = worldPos - _WorldSpaceCameraPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 scrollingUV = i.uv + float2(_ScrollX, _ScrollY) * _Time.y;
                scrollingUV = frac(scrollingUV);

                fixed4 texColor = tex2D(_MainTex, scrollingUV);

                float2 offset = i.uv - _Center;
                float radius = length(offset);

                float2 maxOffset = max(abs(_Center), abs(1.0 - _Center));
                float maxDist = length(maxOffset);
                radius = saturate(radius / maxDist);

                float alphaMask = pow(1.0 - radius, _RadialMaskPower);

                fixed4 finalColor = texColor * _Color;
                finalColor.a *= alphaMask * _AlphaMultiplier;

                // Exponential fog
                float dist = length(i.viewVec);
                float fogFactor = 1.0 - exp(-_FogDensity * dist);
                fogFactor = saturate(fogFactor);
                finalColor.rgb = lerp(finalColor.rgb, _FogColor.rgb, fogFactor * _FogColor.a);

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}