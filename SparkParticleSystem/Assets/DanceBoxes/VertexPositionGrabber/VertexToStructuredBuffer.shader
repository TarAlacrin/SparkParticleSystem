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
CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
			 
            uniform AppendStructuredBuffer<float3> appendBuffer : register(u1);


			struct APPDATA
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			    uint id : SV_VertexID;      
            };
 
            struct v2f
            {
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
            };
 
			v2f vert (APPDATA IN)
            {
				v2f vs;
				appendBuffer.Append(mul(unity_ObjectToWorld, IN.vertex).xyz);

				vs.vertex = UnityObjectToClipPos(IN.vertex);
				vs.uv = IN.uv;
                return vs;
            }
 
			float4 frag (v2f ps) : SV_TARGET
            {
                return float4(1,1,1,1);
            }
 
ENDCG
        }
    }
}