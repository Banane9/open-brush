// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/CubeMap" {
    Properties{
        _CubeMap("Cube Map", Cube) = "white" {}
        _Alpha("Alpha", Range(0, 1)) = 1
    }
        SubShader{
            Pass {
                Tags { "DisableBatching" = "True" "Queue" = "Transparent" "RenderType" = "Transparent" }
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                samplerCUBE _CubeMap;
                fixed _Alpha;

                struct v2f {
                    float4 pos : SV_Position;
                    half3 uv : TEXCOORD0;
                };

                v2f vert(appdata_tan v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.vertex.xyz * half3(-1,1,1); // mirror so cubemap projects as expected
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    return texCUBE(_CubeMap, i.uv) * fixed4(1, 1, 1, _Alpha);
                }
                ENDCG

                ZWrite Off
                Cull Front
                Blend SrcAlpha OneMinusSrcAlpha
            }
    }
}
