Shader "SDnBshades/TriSparkLight" 
{

	Properties
	{
		_PartWSDims("Actual WS XYZ scale, where Y= 'height' thickness, W=extra speed multiplier for the Z", Vector) = (1,1,1,2)
		_PartSSMax("Maximum XY Screenspace Size in percentage? hopefully", Vector) = (0.01, 0.01, 0,0)
		_VelMinMaxVar("Velocity minimum and maximum value for different things", Vector) = (0.0, 3.0,0.0,0.0)

		_IdealLightRange("Ideal radius of light ", float) = 1.0


		
		_ColorStartA("ColorStartA", Color) = (0.90,0.46,0.3,1.0)
		_ColorStartB("ColorStartB", Color) = (0.80,0.66,0.3,1.0)

		_ColorVelMaxA("ColorVelMaxA", Color) = (0.90,0.46,0.3,2.0)
		_ColorVelMaxB("ColorVelMaxB", Color) = (0.97,0.76,0.5,2.0)


		_ColorAgeDeadA("ColorAgeDeathA", Color) = (0.225,0.225,0.225,0.227)
		_ColorAgeDeadB("ColorAgeDeathB", Color) = (0.125,0.125,0.125,0.127)


	//	_DeadColor("DeadColor", Color) = (0.105,0.0469,0.0625,0.227)

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
		//BlendOp Max 
    	Blend SrcAlpha OneMinusSrcAlpha
		//Blend One One
     	
		CGPROGRAM
		#pragma target 5.0

		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag

		#include "UnityCG.cginc"

		//StructuredBuffer<float4> _Colors;		
		StructuredBuffer<float3> _VertPos;
		StructuredBuffer<float3> _VertVel;
		StructuredBuffer<float2> _VertDat;

		StructuredBuffer<int> _LivingID;

		float _Size;
		float4 _VelMinMaxVar;
		float _IdealLightRange;

		fixed4 _ColorStartA;
		fixed4 _ColorStartB;

		fixed4 _ColorVelMaxA;
		fixed4 _ColorVelMaxB;

		fixed4 _ColorAgeDeadA;
		fixed4 _ColorAgeDeadB;


		float4 _PartWSDims;
		float2 _PartSSMax;

		uniform float3 _MaxAge;

		struct v2g 
		{
			float4 pos : SV_POSITION;
			float4 fordir : TEXCOORD1;
			float4 col : COLOR;
			int identity : TEXCOORD0;
		};



		v2g vert(uint id : SV_VertexID)
		{
			v2g OUT;

			uint relID = _LivingID[id];//id

			float3 worldPos = _VertPos[relID];
			OUT.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));

			OUT.identity = relID;

			float speed= length(_VertVel[relID]);
			float speedmod = (speed - _VelMinMaxVar.x)/_VelMinMaxVar.y;
			
			OUT.fordir.w = speedmod;//maybe have .w be the overall size mult?
			OUT.fordir.xyz = normalize(_VertVel[relID]);

			//and here i deal with the idea of what color to make it. 
			//This should eventually find its way into the compute shader though, ugh. 
			//so the light cast is the same color
			float agefract =  saturate((_VertDat[relID].y-1.5)/_MaxAge.z);

			float rando = frac(relID*1.23);
			float rando2 = frac(relID*0.2378);
			float rando3 = frac((rando2*100.7357)+(rando*100.3287));
			
			fixed4 colorez = lerp(_ColorStartA, _ColorStartB, rando);
			fixed4 additionalCol = lerp(_ColorVelMaxA, _ColorVelMaxB, rando2);
			additionalCol*=additionalCol.a*speedmod*10;
			colorez += additionalCol;

			fixed4 dedcol = lerp(_ColorAgeDeadA, _ColorAgeDeadB, rando3);

			colorez = lerp(dedcol,colorez, saturate(2*(1-(1-agefract)*(1-agefract))));

			OUT.col.rgb = colorez;
			OUT.col.a = colorez.a;

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
			int id = IN[0].identity;

			//calculate the various anglez
			float3 zDim = IN[0].fordir.xyz*(_PartWSDims.z + _PartWSDims.w*IN[0].fordir.w);

			float3 vertpos = _VertPos[id] - zDim*0.3;

			float isvert = floor(abs(dot(IN[0].fordir.xyz, float3(0,1,0))));
			float3 xDim = lerp(cross(IN[0].fordir.xyz, float3(0,1,0)), float3(1,0,0) , isvert);
			zDim = lerp(IN[0].fordir.xyz, float3(0,0,1),isvert);

			xDim.y = 0;
			zDim.y = 0;
			xDim.xz = normalize(xDim.xz);
			zDim.xz = normalize(zDim.xz);

			float rad = _PartWSDims.x*0.5 + _PartWSDims.y;
			
			float mult = saturate(vertpos.y/_IdealLightRange);
			mult = 1 - mult*mult;
			
			vertpos.y = 0;
			rad *= mult*IN[0].col.a*IN[0].col.a*IN[0].col.a;

			OUT.colo = IN[0].col;

			float3 posPoint1 = vertpos + zDim*rad + xDim*rad;
			float3 posPoint2 = vertpos + zDim*rad - xDim*rad;
			float3 posPoint3 = vertpos - zDim*rad + xDim*rad;
			float3 posPoint4 = vertpos - zDim*rad - xDim*rad;

			OUT.posi = mul (UNITY_MATRIX_VP, float4(posPoint1, 1)); OUT.uv=float2(0,0); outStream.Append(OUT);
			OUT.posi = mul (UNITY_MATRIX_VP, float4(posPoint2, 1)); OUT.uv=float2(1,0); outStream.Append(OUT);
			OUT.posi = mul (UNITY_MATRIX_VP, float4(posPoint3, 1)); OUT.uv=float2(0,1); outStream.Append(OUT);
			OUT.posi = mul (UNITY_MATRIX_VP, float4(posPoint4, 1)); OUT.uv=float2(1,1); outStream.Append(OUT);

			outStream.RestartStrip();
		}

		float4 frag (g2f IN) : COLOR
		{
			//return IN.colo; 

			float alpha = 1 - saturate(length(abs(1 - 2*IN.uv.xy)));

			return float4(IN.colo.rgb, IN.colo.a*alpha*alpha); 
		}

		ENDCG

		}
	}

Fallback Off
}















