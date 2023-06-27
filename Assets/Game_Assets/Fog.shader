Shader "Custom/HeightFog" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _FogHeight ("Fog Height", Range(0, 1)) = 0.5
        _FogDensity ("Fog Density", Range(0, 1)) = 0.5
    }

    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vp
            #pragma fragment fp

            #include "UnityCG.cginc"

            struct VertexData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vp(VertexData v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex, _CameraDepthTexture;
            float4 _FogColor;
            float _FogDensity, _FogOffset, _FogHeight;

            float4 fp(v2f i) : SV_Target {
                int x, y;
                float4 col = tex2D(_MainTex, i.uv);
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                depth = Linear01Depth(depth);

                float viewDistance = depth * _ProjectionParams.z * 50;

                float heightFactor = (i.vertex.y - _FogHeight) * 1.0 / (_FogHeight * _FogHeight);
                float heightFog = pow(saturate(heightFactor), 3.0);

                float distFactor = smoothstep(0.0, 210.0, ((viewDistance - (_FogOffset))) / _FogHeight);

                float fogFactor = ((_FogDensity) / sqrt(log(2))) * distFactor;
                fogFactor = exp2(-fogFactor * fogFactor);

                float4 fogOutput = lerp(_FogColor, col, saturate(fogFactor + heightFog));

                return fogOutput;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
