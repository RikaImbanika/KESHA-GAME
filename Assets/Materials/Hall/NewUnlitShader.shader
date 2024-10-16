Shader "AlphaSelfIllum" {
    Properties {
        _Color ("Color Tint", Color) = (20,20,20,1)
        _MainTex ("SelfIllum Color (RGB) Alpha (A)", 2D) = "white"
    }
    Category {
       Lighting Off
       ZWrite Off
       Cull Back
       Blend SrcAlpha OneMinusSrcAlpha
       Tags {Queue=Transparent}
       SubShader {
            Material {
               Emission [_Color]
            }
            Pass {
               SetTexture [_MainTex] {
                      Combine Texture * Primary, Texture * Primary
                }
            }
        }
    }
}
