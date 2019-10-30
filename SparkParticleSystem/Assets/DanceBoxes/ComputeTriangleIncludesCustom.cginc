


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



float colision(float3 p1, float3 p2, float3 p3)
{
    float3 pnormal = normalize(cross(p2 - p1, p3 - p2));
    
    //pn.x*X -pn.x*p1.x + pn.y*Y -pn.y*p1.y + pn.z*Z -pn.z*p1.z
    //pn.z*Z =;
    //Z = t*1 = (pn.x*p1.x + pn.y*p1.y + pn.z*p1.z)/pn.z 
    float t = ((pnormal.x * p1.x + pnormal.y * p1.y + pnormal.z * p1.z) / pnormal.z);

    if (saturate(t) == t)
        return t * sign(pnormal.z);
    else
        return 0;
}


float TriangleIntersectsUnitSquare(float3 p1, float3 p2, float3 p3)
{
    float intersect = (intpoint_inside_trigon(float2(0.0, 0.0), p1.xy, p2.xy, p3.xy));

    if (intersect >0)
    {
        intersect = colision(p1, p2, p3);
    }


    return intersect;
}