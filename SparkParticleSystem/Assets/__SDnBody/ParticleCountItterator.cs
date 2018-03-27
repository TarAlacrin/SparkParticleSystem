using UnityEngine;
using System.Collections;

public static class ParticleCountItterator 
{
	public static int multiplier = 2;

	public static void Init(BufferHandlerPS1 bffHndl)
	{
		bffHndl.count = (int)(65536f*Mathf.Pow(2,(int)multiplier));
	}
}
