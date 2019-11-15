#define THREADSIZE 64


float mod(float x, float m)
{ 
	return x - m * floor(x / m);
}