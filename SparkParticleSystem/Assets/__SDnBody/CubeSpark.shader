// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/CubeSpark" 
{
	Properties
	{

		_Color ("Color", Color) = (1,1,1,1)

		_LightColor("LightColor", Color) = (1,0,0,1)
		/*
		_PartWSDims("Actual WS XYZ scale, where Y= 'height' thickness, W=extra speed multiplier for the Z", Vector) = (1,1,1,2)
		_PartSSMax("Maximum XY Screenspace Size in percentage? hopefully", Vector) = (0.01, 0.01, 0,0)
		_VelMinMaxVar("Velocity minimum and maximum value for different things", Vector) = (0.0, 3.0,0.0,0.0)

		//_Color("Color", Color) = (0.105,0.0469,0.0625,0.227)

		
		
		_ColorStartA("ColorStartA", Color) = (0.90,0.46,0.3,1.0)
		_ColorStartB("ColorStartB", Color) = (0.80,0.66,0.3,1.0)

		_ColorVelMaxA("ColorVelMaxA", Color) = (0.90,0.46,0.3,2.0)
		_ColorVelMaxB("ColorVelMaxB", Color) = (0.97,0.76,0.5,2.0)


		_ColorAgeDeadA("ColorAgeDeathA", Color) = (0.225,0.225,0.225,0.227)
		_ColorAgeDeadB("ColorAgeDeathB", Color) = (0.125,0.125,0.125,0.127)
		*/

	//	_DeadColor("DeadColor", Color) = (0.105,0.0469,0.0625,0.227)

	}
	SubShader 
	{
	Pass{
		//uses DrawProcedural so tag doesnt really matter
		//Tags { "Queue" = "Transparent" }
		Tags { "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha

		
		//ZWrite Off 
		//ZTest LEqual 
		//Cull Off 
		//Fog { Mode Off }
		//BlendOp Max
    	//Blend SrcAlpha OneMinusSrcAlpha
		//Blend One One
     	
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#pragma target 5.0

		#include "UnityCG.cginc"

		//StructuredBuffer<float4> _Colors;		
		/*StructuredBuffer<float3> _VertPos;
		StructuredBuffer<float3> _VertVel;
		StructuredBuffer<float2> _VertDat;

		StructuredBuffer<int> _LivingID;
		*/



		uniform int _LiveParticleCount;
		uniform StructuredBuffer<float3> _ParticlePosition;
		uniform StructuredBuffer<int> _LivingParticles;

		/*
		float _Size;
		float4 _VelMinMaxVar;


		fixed4 _ColorStartA;
		fixed4 _ColorStartB;

		fixed4 _ColorVelMaxA;
		fixed4 _ColorVelMaxB;

		fixed4 _ColorAgeDeadA;
		fixed4 _ColorAgeDeadB;


		float4 _PartWSDims;
		float2 _PartSSMax;

		uniform float3 _MaxAge;
		*/
		fixed4 _Color;
		fixed4 _LightColor;


		/*struct v2g 
		{
			float4 pos : SV_POSITION;
			float4 hordirAndid : TEXCOORD0;
			float4 fordir : TEXCOORD1;
			float4 col : COLOR;
		};



		v2g vert(uint id : SV_VertexID)
		{
			v2g OUT;

			uint relID = _LivingID[id];//id

			float3 worldPos = _VertPos[relID];
			OUT.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));

			//determine axis and facing information
			OUT.hordirAndid.w = relID;


			float speed= length(_VertVel[relID]);
			float speedmod = (speed - _VelMinMaxVar.x)/_VelMinMaxVar.y;
			
			OUT.fordir.w = speedmod;//maybe have .w be the overall size mult?
			OUT.fordir.xyz = normalize(_VertVel[relID]);

			float3 viewDir = normalize( _WorldSpaceCameraPos.xyz - worldPos);
			//this is to determine if the boy should have a bit of a false pitch up or down so it doesn't dissapear if facing the camera.
			
			float3 ydir =  float3(0.0,1.0,0.0);//lerp(float3(0.0,1.0,0.0), -viewDir,  viewDotVel);
			float3 xdir = normalize(cross(OUT.fordir.xyz, ydir));
			float3 altHorzYdir = normalize(cross(OUT.fordir.xyz, xdir));
			altHorzYdir = mul(UNITY_MATRIX_IT_MV, float4(altHorzYdir,0.0f));
			altHorzYdir *= lerp(-1, 1, ceil(saturate(altHorzYdir.x)));
			altHorzYdir = mul(UNITY_MATRIX_T_MV, float4(altHorzYdir, 0.0f));



			float viewDotXdir = pow(abs(dot(viewDir, xdir)), 3.0);
			xdir = lerp(xdir, altHorzYdir, viewDotXdir);
			xdir *= lerp(_PartWSDims.x, _PartWSDims.y, viewDotXdir);

			OUT.hordirAndid.xyz = xdir;


			//and here i deal with the idea of what color to make it. 
			//This should eventually find its way into the compute shader though, ugh. 
			//so the light cast is the same color
			float agefract =  saturate(_VertDat[relID].y/_MaxAge.z);

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

		[maxvertexcount(3)]
		void geom(point v2g IN[1], inout TriangleStream<g2f> outStream)
		{
			g2f OUT;
			OUT.colo = IN[0].col;
			int id = IN[0].hordirAndid.w;

			//calculate the various anglez

			float3 zDim = IN[0].fordir.xyz*(_PartWSDims.z + _PartWSDims.w*IN[0].fordir.w);
			float3 xDim = IN[0].hordirAndid.xyz*(_PartWSDims.x);

			float3 posPoint1 = _VertPos[id] + xDim*0.5;
			float3 posPoint2 = _VertPos[id] - xDim*0.5;
			float3 posPoint3 = _VertPos[id] - zDim;


			OUT.posi = mul (UNITY_MATRIX_VP, float4(posPoint1, 1)); OUT.uv=float2(0,0); outStream.Append(OUT);
			OUT.posi = mul (UNITY_MATRIX_VP, float4(posPoint2, 1)); OUT.uv=float2(1,0); outStream.Append(OUT);

			//this creates the tale of the object
			OUT.colo.a *= 0.5;
			OUT.posi = mul (UNITY_MATRIX_VP, float4(posPoint3, 1)); OUT.uv=float2(0,1); outStream.Append(OUT);

			outStream.RestartStrip();
		}



		


		float4 frag (g2f IN) : COLOR
		{
			//return IN.colo; 
			return IN.colo.rgba; 
		}*/


		float maghatten(float3 vec)
		{
			return abs(vec.x) + abs(vec.y) + abs(vec.z);
		}

        struct v2f
        {
            float2 uv : TEXCOORD0;
            //UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
			float3 fragpos : TEXCOORD1;
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        
        v2f vert (appdata_base v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

			o.fragpos = mul(unity_ObjectToWorld, v.vertex).xyz;

            return o;
        }


		fixed4 lightcalc(float3 partPos, float3 pfragpos, float4 partCol)
		{
			return float4(lerp(float4(0.0,0.0,0.0,0.0), partCol, saturate(1.0- length(pfragpos - partPos)*0.6)));

		}
        
        fixed4 frag (v2f instrct) : COLOR
        {
            // sample the texture
            // fixed4 col = tex2D(_MainTex, i.uv);
            // apply fog
            //UNITY_APPLY_FOG(i.fogCoord, col);

			
			fixed4 col = _Color;

			for(int i=0; i < _LiveParticleCount ; i++)
			{
				fixed4 lightval = lightcalc(_ParticlePosition[_LivingParticles[i]], instrct.fragpos, _LightColor);
				col+= lightval*lightval.a;
				//col+= saturate(float4( _ParticlePosition[_LivingParticles[i]] ,1));

							

			}
			
			//col = saturate(col);

			//col = saturate(float4(instrct.fragpos,1));

			return col;
        }

		ENDCG
		}
	}

	FallBack "Diffuse"
}















