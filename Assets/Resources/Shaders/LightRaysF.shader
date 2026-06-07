Shader "Custom/LightRaysF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TextureScaleX ("Texture Scale X", Float) = 1.0
        _TextureScaleY ("Texture Scale Y", Float) = 1.0
        _ScrollSpeed ("Scroll Speed", Float) = 0.1
        _ScrollAngle ("Scroll Angle (degrees)", Range(0, 360)) = 0
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
                float2 scrollDir : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 viewVec : TEXCOORD2; // camera to vertex in world space
            };

            sampler2D _MainTex;
            float _TextureScaleX;
            float _TextureScaleY;
            float _ScrollSpeed;
            float _ScrollAngle;
            float _RadialMaskPower;
            float _AlphaMultiplier;
            float4 _FogColor;
            float _FogDensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float angleRad = _ScrollAngle * 0.0174533;
                o.scrollDir = float2(cos(angleRad), sin(angleRad));

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewVec = worldPos - _WorldSpaceCameraPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texUV = i.uv * float2(_TextureScaleX, _TextureScaleY) + i.scrollDir * (_ScrollSpeed * _Time.y);
                fixed4 c = tex2D(_MainTex, frac(texUV));

                float r = saturate(length(i.uv - 0.5) * 1.41421356);
                c.a *= pow(1.0 - r, _RadialMaskPower) * _AlphaMultiplier;

                // Exponential fog
                float dist = length(i.viewVec);
                float fogFactor = 1.0 - exp(-_FogDensity * dist);
                fogFactor = saturate(fogFactor);
                c.rgb = lerp(c.rgb, _FogColor.rgb, fogFactor * _FogColor.a);

                return c;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}