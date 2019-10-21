// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AppendExample/BufferShader"
{
	Properties
	{
		size("SIZE", Float) = 1
	}

    SubShader

    {

        Pass

        {
            ZTest Always Cull Off ZWrite Off

            Fog { Mode off }



            CGPROGRAM

            #include "UnityCG.cginc"
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
			#pragma geometry geom

			uniform StructuredBuffer<float3> _Data;
            uniform float4 col;
			uniform float size;
			struct v2g 
			{
				float4 pos : SV_POSITION;
				int id : TEXCOORD0;
			};

			struct g2f {
				float4 posi : SV_POSITION;
			};


			v2g vert(uint id : SV_VertexID)
			{
				v2g OUT;
				float3 worldPos = _Data[id];
				OUT.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));
				OUT.id = id;
				//should move tangent and binormal calcs to here.
				return OUT;
			}

			[maxvertexcount(4)]
			void geom(point v2g IN[1], inout TriangleStream<g2f> outStream)
			{
				g2f OUT;
				float4 tangent = float4(size,0,0,0);
				float4 binormal = float4(0,size,0,0);

				OUT.posi = IN[0].pos + tangent - binormal;  outStream.Append(OUT);
				OUT.posi = IN[0].pos + tangent + binormal;  outStream.Append(OUT);
				OUT.posi = IN[0].pos - tangent - binormal;  outStream.Append(OUT);
				OUT.posi = IN[0].pos - tangent + binormal;  outStream.Append(OUT);

				outStream.RestartStrip();
			}

			float4 frag (g2f IN) : COLOR
			{
				return col;
			}


        ENDCG

        }

    }

}
