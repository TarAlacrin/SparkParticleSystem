int intpoint_inside_trigon(float2 s, float2 a, float2 b, float2 c)
{
    float as_x = s.x - a.x;
    float as_y = s.y - a.y;

    bool s_ab = (b.x - a.x) * as_y - (b.y - a.y) * as_x > 0;

    if ((c.x - a.x) * as_y - (c.y - a.y) * as_x > 0 == s_ab)
        return 0;

    if ((c.x - b.x) * (s.y - b.y) - (c.y - b.y) * (s.x - b.x) > 0 != s_ab)
        return 0;

    return 1;
}

float precalculated_colision(float3 p1, float2 pNxyDividedBypNz)
{
	float t = (p1.x *pNxyDividedBypNz.x + p1.y*pNxyDividedBypNz.y) + p1.z;

	return t;
}


float colision(float3 p1, float3 p2, float3 p3)
{
    float3 pnormal = normalize(cross(p2 - p1, p3 - p2));

	float t = precalculated_colision(p1, pnormal.xy / pnormal.z);


    if (saturate(t) == t)
        return t * sign(pnormal.z);
    else
        return 0;
}


float3 CalculateNormalDividendValue(float3 p1, float3 p2, float3 p3)
{
	float3 pnormal = normalize(cross(p2 - p1, p3 - p2));
	float2 pNxyDividedBypNz = pnormal.xy / pnormal.z;
	float3 pNxyDividedBypNzPlusSignZ = float3(pNxyDividedBypNz.x, pNxyDividedBypNz.y, sign(pnormal.z));
	return pNxyDividedBypNzPlusSignZ;
}


float TriangleIntersectsUnitSquare(float3 p1, float3 p2, float3 p3)
{
	float intersect = (intpoint_inside_trigon(float2(0.0, 0.0), p1.xy, p2.xy, p3.xy));

	if (intersect > 0)
	{
		intersect = colision(p1, p2,p3);
	}


	return intersect;
}



float TriangleIntersectsUnitSquarePrecalcedALTERNATE(float3 p1, float3 p2, float3 p3, float3 pNxyDividedBypNzPlusSignZ)
{
    float intersect = (intpoint_inside_trigon(float2(0.0, 0.0), p1.xy, p2.xy, p3.xy));

    if (intersect >0)
    {
        intersect = precalculated_colision(p1, pNxyDividedBypNzPlusSignZ.xy);
		if (saturate(intersect) == intersect)
			intersect *= pNxyDividedBypNzPlusSignZ.z;
		else
			intersect = 0;
	}


    return intersect;
}


float TriangleIntersectsUnitSquarePreCalculated(float3 p1, float3 p2, float3 p3, float3 pNxyDividedBypNzPlusSignZ)
{
	float intersect = (intpoint_inside_trigon(float2(0.0, 0.0), p1.xy, p2.xy, p3.xy));

	if (intersect > 0)
	{
		intersect = precalculated_colision(p1, pNxyDividedBypNzPlusSignZ.xy);
		//if (saturate(intersect) == intersect)
		//	intersect *= pNxyDividedBypNzPlusSignZ.z;
		//else
		//	intersect = 0;
	}
	
	return intersect;
}