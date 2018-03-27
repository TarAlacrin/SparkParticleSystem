//
//	Code repository for GPU noise development blog
//	http://briansharpe.wordpress.com
//	https://github.com/BrianSharpe
//
//	I'm not one for copyrights.  Use the code however you wish.
//	All I ask is that credit be given back to the blog or myself when appropriate.
//	And also to let me know if you come up with any changes, improvements, thoughts or interesting uses for this stuff. :)
//	Thanks!
//
//	Brian Sharpe
//	brisharpe CIRCLE_A yahoo DOT com
//	http://briansharpe.wordpress.com
//	https://github.com/BrianSharpe
//
//===============================================================================
//  Scape Software License
//===============================================================================
//
//Copyright (c) 2007-2012, Giliam de Carpentier
//All rights reserved.
//
//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met: 
//
//1. Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer. 
//2. Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNERS OR CONTRIBUTORS BE LIABLE 
//FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
//DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
//CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.;

Shader "Noise/3dsimpSwissAnim" 
{
	Properties 
	{
		_Octaves ("Octaves", Float) = 8.0
		_Frequency ("Frequency", Float) = 1.0
		_Amplitude ("Amplitude", Float) = 1.0
		_Lacunarity ("Lacunarity", Float) = 1.92
		_Persistence ("Persistence", Float) = 0.8
		_Offset ("Offset", Vector) = (0.0, 0.0, 0.0, 0.0)
		_RidgeOffset ("Ridge Offset", Float) = 1.0
		_Warp ("Warp", Float) = 0.25
		_AnimSpeed("Animation Speed", Float) = 1.0
	}

	CGINCLUDE
		//
		//	FAST32_hash
		//	A very fast hashing function.  Requires 32bit support.
		//	http://briansharpe.wordpress.com/2011/11/15/a-fast-and-simple-32bit-floating-point-hash-function/
		//
		//	The hash formula takes the form....
		//	hash = mod( coord.x * coord.x * coord.y * coord.y, SOMELARGEFLOAT ) / SOMELARGEFLOAT
		//	We truncate and offset the domain to the most interesting part of the noise.
		//	SOMELARGEFLOAT should be in the range of 400.0->1000.0 and needs to be hand picked.  Only some give good results.
		//	3D Noise is achieved by offsetting the SOMELARGEFLOAT value by the Z coordinate
		//
		void FAST32_hash_3D( 	float3 gridcell,
								float3 v1_mask,		//	user definable v1 and v2.  ( 0's and 1's )
								float3 v2_mask,
								out float4 hash_0,
								out float4 hash_1,
								out float4 hash_2	)		//	generates 3 random numbers for each of the 4 3D cell corners.  cell corners:  v0=0,0,0  v3=1,1,1  the other two are user definable
		{
			//    gridcell is assumed to be an integer coordinate
		
			//	TODO: 	these constants need tweaked to find the best possible noise.
			//			probably requires some kind of brute force computational searching or something....
			const float2 OFFSET = float2( 50.0, 161.0 );
			const float DOMAIN = 69.0;
			const float3 SOMELARGEFLOATS = float3( 635.298681, 682.357502, 668.926525 );
			const float3 ZINC = float3( 48.500388, 65.294118, 63.934599 );
		
			//	truncate the domain
			gridcell.xyz = gridcell.xyz - floor(gridcell.xyz * ( 1.0 / DOMAIN )) * DOMAIN;
			float3 gridcell_inc1 = step( gridcell, float3( DOMAIN - 1.5, DOMAIN - 1.5, DOMAIN - 1.5) ) * ( gridcell + 1.0 );
		
			//	compute x*x*y*y for the 4 corners
			float4 P = float4( gridcell.xy, gridcell_inc1.xy ) + OFFSET.xyxy;
			P *= P;
			float4 V1xy_V2xy = lerp( P.xyxy, P.zwzw, float4( v1_mask.xy, v2_mask.xy ) );		//	apply mask for v1 and v2
			P = float4( P.x, V1xy_V2xy.xz, P.z ) * float4( P.y, V1xy_V2xy.yw, P.w );
		
			//	get the lowz and highz mods
			float3 lowz_mods = float3( 1.0 / ( SOMELARGEFLOATS.xyz + gridcell.zzz * ZINC.xyz ) );
			float3 highz_mods = float3( 1.0 / ( SOMELARGEFLOATS.xyz + gridcell_inc1.zzz * ZINC.xyz ) );
		
			//	apply mask for v1 and v2 mod values
		    v1_mask = ( v1_mask.z < 0.5 ) ? lowz_mods : highz_mods;
		    v2_mask = ( v2_mask.z < 0.5 ) ? lowz_mods : highz_mods;
		
			//	compute the final hash
			hash_0 = frac( P * float4( lowz_mods.x, v1_mask.x, v2_mask.x, highz_mods.x ) );
			hash_1 = frac( P * float4( lowz_mods.y, v1_mask.y, v2_mask.y, highz_mods.y ) );
			hash_2 = frac( P * float4( lowz_mods.z, v1_mask.z, v2_mask.z, highz_mods.z ) );
		}
		//
		//	Given an arbitrary 3D point this calculates the 4 vectors from the corners of the simplex pyramid to the point
		//	It also returns the integer grid index information for the corners
		//
		void Simplex3D_GetCornerVectors( 	float3 P,					//	input point
											out float3 Pi,			//	integer grid index for the origin
											out float3 Pi_1,			//	offsets for the 2nd and 3rd corners.  ( the 4th = Pi + 1.0 )
											out float3 Pi_2,
											out float4 v1234_x,		//	vectors from the 4 corners to the intput point
											out float4 v1234_y,
											out float4 v1234_z )
		{
			//
			//	Simplex math from Stefan Gustavson's and Ian McEwan's work at...
			//	http://github.com/ashima/webgl-noise
			//
		
			//	simplex math constants
			const float SKEWFACTOR = 1.0/3.0;
			const float UNSKEWFACTOR = 1.0/6.0;
			const float SIMPLEX_CORNER_POS = 0.5;
			const float SIMPLEX_PYRAMID_HEIGHT = 0.70710678118654752440084436210485;	// sqrt( 0.5 )	height of simplex pyramid.
		
			P *= SIMPLEX_PYRAMID_HEIGHT;		// scale space so we can have an approx feature size of 1.0  ( optional )
		
			//	Find the vectors to the corners of our simplex pyramid
			Pi = floor( P + dot( P, float3( SKEWFACTOR, SKEWFACTOR, SKEWFACTOR) ) );
			float3 x0 = P - Pi + dot(Pi, float3( UNSKEWFACTOR, UNSKEWFACTOR, UNSKEWFACTOR ) );
			float3 g = step(x0.yzx, x0.xyz);
			float3 l = 1.0 - g;
			Pi_1 = min( g.xyz, l.zxy );
			Pi_2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - Pi_1 + UNSKEWFACTOR;
			float3 x2 = x0 - Pi_2 + SKEWFACTOR;
			float3 x3 = x0 - SIMPLEX_CORNER_POS;
		
			//	pack them into a parallel-friendly arrangement
			v1234_x = float4( x0.x, x1.x, x2.x, x3.x );
			v1234_y = float4( x0.y, x1.y, x2.y, x3.y );
			v1234_z = float4( x0.z, x1.z, x2.z, x3.z );
		}
		//
		//	SimplexPerlin3D_Deriv
		//	SimplexPerlin3D noise with derivatives
		//	returns float3( value, xderiv, yderiv, zderiv )
		//
		float4 SimplexPerlin3D_Deriv(float3 P)
		{
			//	calculate the simplex vector and index math
			float3 Pi;
			float3 Pi_1;
			float3 Pi_2;
			float4 v1234_x;
			float4 v1234_y;
			float4 v1234_z;
			Simplex3D_GetCornerVectors( P, Pi, Pi_1, Pi_2, v1234_x, v1234_y, v1234_z );
		
			//	generate the random vectors
			//	( various hashing methods listed in order of speed )
			float4 hash_0;
			float4 hash_1;
			float4 hash_2;
			FAST32_hash_3D( Pi, Pi_1, Pi_2, hash_0, hash_1, hash_2 );
			hash_0 -= 0.49999;
			hash_1 -= 0.49999;
			hash_2 -= 0.49999;
		
			//	normalize random gradient vectors
			float4 norm = rsqrt( hash_0 * hash_0 + hash_1 * hash_1 + hash_2 * hash_2 );
			hash_0 *= norm;
			hash_1 *= norm;
			hash_2 *= norm;
		
			//	evaluate gradients
			float4 grad_results = hash_0 * v1234_x + hash_1 * v1234_y + hash_2 * v1234_z;
		
			//	evaluate the surflet f(x)=(0.5-x*x)^3
			float4 m = v1234_x * v1234_x + v1234_y * v1234_y + v1234_z * v1234_z;
			m = max(0.5 - m, 0.0);		//	The 0.5 here is SIMPLEX_PYRAMID_HEIGHT^2
			float4 m2 = m*m;
			float4 m3 = m*m2;
		
			//	calc the deriv
			float4 temp = -6.0 * m2 * grad_results;
			float xderiv = dot( temp, v1234_x ) + dot( m3, hash_0 );
			float yderiv = dot( temp, v1234_y ) + dot( m3, hash_1 );
			float zderiv = dot( temp, v1234_z ) + dot( m3, hash_2 );
		
			const float FINAL_NORMALIZATION = 37.837227241611314102871574478976;	//	scales the final result to a strict 1.0->-1.0 range
		
			//	sum with the surflet and return
			return float4( dot( m3, grad_results ), xderiv, yderiv, zderiv ) * FINAL_NORMALIZATION;
		}
		float SimplexDerivedSwiss(float3 p, int octaves, float3 offset, float frequency, float amplitude, float lacunarity, float persistence, float warp, float ridgeOffset)
		{
			float sum = 0.0;
			float3 dsum = float3(0.0, 0.0, 0.0);
			for (int i = 0; i < octaves; i++)
			{
				float4 n = 0.5 * (0 + (ridgeOffset - abs(SimplexPerlin3D_Deriv((p+offset+warp*dsum)*frequency))));
				sum += amplitude * n.x;
				dsum += amplitude * n.yzw * -n.x;
				frequency *= lacunarity;
				amplitude *= persistence * saturate(sum);
			}
			return sum;
		}
	ENDCG

	SubShader 
	{



		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma glsl
		#pragma target 3.0
		
		fixed _Octaves;
		float _Frequency;
		float _Amplitude;
		float3 _Offset;
		float _Lacunarity;
		float _Persistence;
		float _RidgeOffset;
		float _Warp;
		float _AnimSpeed;

		struct Input 
		{
			float3 pos;

		};

		void vert (inout appdata_full v, out Input OUT)
		{
			UNITY_INITIALIZE_OUTPUT(Input, OUT);
			OUT.pos = float3(v.texcoord.xy, _Time.y * _AnimSpeed);
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float h = SimplexDerivedSwiss(IN.pos, _Octaves, _Offset, _Frequency, _Amplitude, _Lacunarity, _Persistence, _Warp, _RidgeOffset);
			
			h = h * 0.5 + 0.5;
			
			float4 color = float4(h, h, h, h);

			o.Albedo = color.rgb;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	
	FallBack "Diffuse"
}