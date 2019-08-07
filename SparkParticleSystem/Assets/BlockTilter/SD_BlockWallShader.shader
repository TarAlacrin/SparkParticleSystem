// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SD_Blockwall/Blocks" 
{

	Properties
	{
		_BlckDims("Block xyz dims, spacing factor", Vector) = (1, 1, 0.25, 0.1)


	}
	SubShader 
	{
		//uses DrawProcedural so tag doesnt really matter
		Tags { "Queue" = "Transparent" }
	
		Pass 
		{
		
		ZWrite Off 
		ZTest LEqual 
		Cull Off 
		Fog { Mode Off }
		BlendOp Max
    	//Blend SrcAlpha OneMinusSrcAlpha
		//Blend One One
     	
		CGPROGRAM
		#pragma target 5.0

		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag

		#include "UnityCG.cginc"
	
		uniform float4 _BlckDims;
		StructuredBuffer<float4> _BlockDats;
		//StructuredBuffer<float4> _BlockCols;


		//uniform float3 _MaxAge;

		struct v2g 
		{
			float4 pos : SV_POSITION;
			float4 uvPos :TEXCOORD0;
			float4 hordirAndid : TEXCOORD1;
			float4 fordir : TEXCOORD2;
			float4 col : COLOR;
		};



		v2g vert(uint id : SV_VertexID)
		{
			v2g OUT;

			float2 gridPos = id/dim.x + id%dim.y;

			uint relID = id;//_LivingID[id];//id

			float3 modelPos = 0;//_BlockDats[relID].xyw;
			modelPos.xy =  gridPos.xy;
			modelPos *= _BlckDims.xyz + _BlckDims.www;
			//offset the blocks by half the width and the spacer;
			modelPos.xy += (_BlckDims.xy*0.5 + _BlckDims.ww);


			OUT.pos = UnityObjectToClipPos (float4(modelPos.xyz,1.0f));

			//determine axis and facing information
			//OUT.hordirAndid.w = relID;


			
			//OUT.fordir.w = speedmod;//maybe have .w be the overall size mult?
			//OUT.fordir.xyz = normalize(_VertVel[relID]);

			//float3 viewDir = normalize( _WorldSpaceCameraPos.xyz - worldPos);
			//this is to determine if the boy should have a bit of a false pitch up or down so it doesn't dissapear if facing the camera.
			
			//float3 ydir =  float3(0.0,1.0,0.0);//lerp(float3(0.0,1.0,0.0), -viewDir,  viewDotVel);
			//float3 xdir = normalize(cross(OUT.fordir.xyz, ydir));
			//float3 altHorzYdir = normalize(cross(OUT.fordir.xyz, xdir));
			float4 colorez = float4(0.8,0.25,0,1.0);
			OUT.col.rgba = colorez;
			//OUT.col.a = colorez.a;

			return OUT;
		}
		
		struct g2f {
			float4 posi : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 colo : COLOR;
		};

		[maxvertexcount(6)]
		void geom(point v2g IN[1], inout TriangleStream<g2f> outStream)
		{
			g2f OUT;
			OUT.colo = IN[0].col;
			int id = IN[0].hordirAndid.w;

			//calculate the various anglez

			//float3 zDim = IN[0].fordir.xyz*(_PartWSDims.z + _PartWSDims.w*IN[0].fordir.w);
			//float3 xDim = IN[0].hordirAndid.xyz*(_PartWSDims.x);

			//float3 posPoint1 = _VertPos[id] + xDim*0.5;
			//float3 posPoint2 = _VertPos[id] - xDim*0.5;
			//float3 posPoint3 = _VertPos[id] - zDim;


			//OUT.posi = mul (UNITY_MATRIX_VP, float4(posPoint1, 1)); OUT.uv=float2(0,0); outStream.Append(OUT);
			//OUT.posi = mul (UNITY_MATRIX_VP, float4(posPoint2, 1)); OUT.uv=float2(1,0); outStream.Append(OUT);

			//OUT.posi = mul (UNITY_MATRIX_VP, float4(posPoint3, 1)); OUT.uv=float2(0,1); outStream.Append(OUT);

			outStream.RestartStrip();
		}

		float4 frag (g2f IN) : COLOR
		{
			//return IN.colo; 
			return IN.colo.rgba; 
		}

		ENDCG

		}
	}

Fallback Off
}















