// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mobile/VertexLitColor" {
    Properties {
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 80

        Pass {
            Lighting On
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float3 Shade1VertexLights (float4 vertex, float3 normal)
            {
                float3 viewpos = mul (UNITY_MATRIX_MV, vertex).xyz;
                float3 viewN = mul ((float3x3)UNITY_MATRIX_IT_MV, normal);
                float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;

                float3 toLight = unity_LightPosition[0].xyz - viewpos.xyz * unity_LightPosition[0].w;
                float lengthSq = dot(toLight, toLight);
                float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[0].z);
                float diff = max (0, dot (viewN, normalize(toLight)));
                lightColor += unity_LightColor[0].rgb * (diff * atten);

                return lightColor;
            }

			struct vertexData
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
			};


            struct v2f
            {
                fixed4 color : COLOR0;
                float4 pos : SV_POSITION;
            };

            v2f vert (vertexData v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = float4 (Shade1VertexLights(v.vertex, v.normal)*2, 1.0)*v.color;
                return o; 
            }

            fixed4 frag (v2f i) : COLOR {
                return i.color;
            }
            ENDCG
        }
    }
}
