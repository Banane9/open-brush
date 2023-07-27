Shader "Unlit/SlicedCubeMap" {
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

                static const float pi = 3.141592653589793238462;

                samplerCUBE _CubeMap;
                fixed _Alpha;
                uniform sampler1D slices;
                uniform int count;

                struct v2f {
                    float4 pos : SV_Position;
                    half3 uv : TEXCOORD0;
                    float2 dir : TEXTCOORD1;
                };

                v2f vert(appdata_tan v) {
                    v2f o;
                    o.dir = float2(v.vertex.x, v.vertex.z);
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.vertex.xyz; // don't mirror so cubemap projects as expected
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target{
                    float2 dir = normalize(i.dir);
                    float angle = acos(dot(dir, float2(0, 1)));

                    if (dir.x < 0)
                        angle = (2 * pi) - angle;

                    float alpha = _Alpha * tex1D(slices, ((angle + (pi / count)) / (2 * pi)));
                    return texCUBE(_CubeMap, i.uv) * fixed4(1, 1, 1, alpha);
                }
                ENDCG

                ZWrite On
                Cull Front
                Blend SrcAlpha OneMinusSrcAlpha
            }
    }
}
