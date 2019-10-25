//https://github.com/przemyslawzaworski
//Assign displacement map (R) to properties.
 
Shader "Vertex To Structured Buffer"
{
Properties
    {
    }
Subshader
    {
Pass
        {
			ZTest Always
			Cull Off
			ZWrite Off
			Fog { Mode off }
		CGPROGRAM
			#include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
			#pragma geometry geom
            #pragma target 5.0
			 
            uniform AppendStructuredBuffer<float4> WATriVertexPositionBuffer : register(u1);

			struct APPDATA
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			    uint id : SV_VertexID;    
				float4 col : COLOR;
            };

			struct v2g {
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD0;
				fixed4 col : COLOR;
			};

			struct g2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 col : COLOR;
			};
 
			v2g vert (APPDATA IN)
            {
				v2g vs;
				vs.worldPos = float4(mul(unity_ObjectToWorld, IN.vertex).xyz, IN.id);

				vs.pos = UnityObjectToClipPos(IN.vertex);
				vs.uv = IN.uv;
				vs.col = IN.col;

				WATriVertexPositionBuffer.Append(vs.worldPos);

                return vs;
            }

			[maxvertexcount(3)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> tristream) {
				//g2f o;
				//o.pos = input[0].pos;	o.uv = input[0].uv;		o.col = input[0].col;
				//tristream.Append(o);
				//o.pos = input[1].pos;	o.uv = input[1].uv;		o.col = input[1].col;
				//tristream.Append(o);
				//o.pos = input[2].pos;	o.uv = input[2].uv;		o.col = input[2].col;
				//tristream.Append(o);


				//WATriVertexPositionBuffer.Append(input[0].worldPos);
				//WATriVertexPositionBuffer.Append(input[1].worldPos);
				//WATriVertexPositionBuffer.Append(input[2].worldPos);
				// 
			}
 
			float4 frag (g2f ps) : SV_TARGET
            {
                return float4(1,1,1,1);
            }
 
ENDCG
        }
    }
}