﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BufferTools 
{

	public const int READ = 1;
	public const int WRITE = 0;

	public static void Swap(ComputeBuffer[] buffer)
	{
		ComputeBuffer tmp = buffer[READ];
		buffer[READ] = buffer[WRITE];
		buffer[WRITE] = tmp;
	}

	public static int[] GetArgs(ComputeBuffer compOBuffToCheck, ComputeBuffer pargsBuffer)
	{
		int[] args = new int[] { 0, 1, 0, 0 };
		pargsBuffer.SetData(args);
		ComputeBuffer.CopyCount(compOBuffToCheck, pargsBuffer, 0);
		pargsBuffer.GetData(args);
		return args;
	}


	public static void DebugCompute(ComputeBuffer computeToDebug, string name, int dims)
	{
		float[] data = new float[dims* dims* dims];

		computeToDebug.GetData(data);

		string todebug = name + "ARRY: " + " @f:";
		for (int k = 0; k < dims; k++)
		{
			todebug += "\n\n\tZ= " + k + "\n";
			for (int j = 0; j < dims; j++)
			{
				todebug += "\nY:" + j + "\t";

				for (int i = 0; i < dims; i++)
				{
					int ind = i + j * dims + k * dims * dims;
					todebug += data[ind] + ", ";
				}
			}
		}
		Debug.LogWarning(todebug);
	}

}