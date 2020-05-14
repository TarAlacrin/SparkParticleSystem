Shader "SDnBshades/DanceBox" 
{

	Properties
	{
		_Color("Color", Color) = (0.105,0.0469,1,1)
		_DeadColor("Dead Color", Color) = (0,0,0,1)
		_Size("Size", float) = 1
	}
	SubShader 
	{
		//uses DrawProcedural so tag doesnt really matter
		Tags { "Queue" = "Transparent" }
	
		Pass 
		{
		
		//ZWrite Off 
		//ZTest LEqual 
		//Cull Off 
		Fog { Mode Off }
    	//Blend SrcAlpha OneMinusSrcAlpha
		//Blend One One
     	
		CGPROGRAM
		#pragma target 5.0

		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag

		#include "UnityCG.cginc"


		struct inputData {
			float3 position;
			float3 normal;
			float age;
		};

		StructuredBuffer< inputData> _Data;

		fixed4 _DeadColor;

		fixed4 _Color;
		float _Size;

		struct v2g 
		{
			float4 pos : SV_POSITION;
			int id : TEXCOORD0;
			float4 col : COLOR;
		};

		v2g vert(uint id : SV_VertexID)
		{
			v2g OUT;
			float3 worldPos = _Data[id].position;
			OUT.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));
			OUT.id = id;
			OUT.col = lerp(_DeadColor, _Color, _Data[id].age*0.1f);
			//TODO should move tangent and binormal calcs to here.
			return OUT;
		}
		
		struct g2f {
		float4 posi : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 colo : COLOR;
		};

		[maxvertexcount(4)]
		void geom(point v2g IN[1], inout TriangleStream<g2f> outStream)
		{
			g2f OUT;
			OUT.colo = (saturate(_Data[IN[0].id].age*0.5f)+0.1f)*float4(abs(_Data[IN[0].id].normal), 1);//*(0.05*cos(IN[0].id*0.27)+0.95); //IN[0].col;
			

			float3 up = normalize(lerp(float3(0, 0, 1), float3(0, 1, 0), saturate(ceil(length(abs(_Data[IN[0].id].normal) - float3(0, 1, 0)) )) ));
			float3 right = normalize( cross(up, _Data[IN[0].id].normal));
			float4 binormal = mul(UNITY_MATRIX_VP, float4(up, 0)*0.5f*_Size);
			float4 tangent = mul(UNITY_MATRIX_VP, float4(right, 0)*0.5f*_Size);
			
			OUT.posi = IN[0].pos + tangent - binormal; OUT.uv = float2(1, 0); outStream.Append(OUT);
			OUT.posi = IN[0].pos + tangent + binormal; OUT.uv = float2(1, 1); outStream.Append(OUT);
			OUT.posi = IN[0].pos - tangent - binormal; OUT.uv = float2(0, 0); outStream.Append(OUT);
			OUT.posi = IN[0].pos - tangent + binormal; OUT.uv = float2(0, 1); outStream.Append(OUT);

			outStream.RestartStrip();
		}

		float4 frag (g2f IN) : COLOR
		{
			return float4(IN.colo.rgb ,IN.colo.a ); 
		}

		ENDCG

		}
	}

Fallback Off
}















