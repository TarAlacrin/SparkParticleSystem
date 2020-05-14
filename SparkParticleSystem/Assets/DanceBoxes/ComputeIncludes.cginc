#define THREADSIZE 64
float4 _Dimensions;
float4 _InvDimensions;


float mod(float x, float m)
{ 
	return x - m * floor(x / m);
}

float modprediv(float x, float m, float invM)
{
	return x - m * floor(x * invM);
}  



uint Index(int3 coordid)
{
	return coordid.x + coordid.y*_Dimensions.x + coordid.z*_Dimensions.w;//w=x*y
}

uint ClampedIndex(int3 coordid)
{
	coordid = clamp(coordid, int3(0, 0, 0), _Dimensions.xyz-int3(1,1,1));
	return coordid.x + coordid.y*_Dimensions.x + coordid.z*_Dimensions.w;//w=x*y
}


int3 Coord(float index)
{
	return int3(modprediv(index, _Dimensions.x, _InvDimensions.x),
		modprediv(floor(index * _InvDimensions.x), _Dimensions.y, _InvDimensions.y),
		floor(index *_InvDimensions.w)//w = ix*iy (or 1/(x*y) )
		);
}

