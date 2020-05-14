using System.Collections;
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


	public static void CycleUp(ComputeBuffer[] buffer)
	{
		int maxIndx = buffer.Length - 1;
		ComputeBuffer tmp = buffer[maxIndx]; 
		for (int i = 0; i < maxIndx; i++)
			buffer[maxIndx-i] = buffer[maxIndx-i -1];

		buffer[0] = tmp;
	}

	public static void CycleDown(ComputeBuffer[] buffer)
	{
		int maxIndx = buffer.Length - 1;
		ComputeBuffer tmp = buffer[0];
		for (int i = 0; i < maxIndx; i++)
			buffer[i] = buffer[i + 1];

		buffer[maxIndx] = tmp;
	}



	public static int[] GetArgs(ComputeBuffer compOBuffToCheck, ComputeBuffer pargsBuffer)
	{
		int[] args = new int[] { 0, 1, 0, 0 };
		pargsBuffer.SetData(args);
		ComputeBuffer.CopyCount(compOBuffToCheck, pargsBuffer, 0);
		pargsBuffer.GetData(args);
		return args;
	}



	public static void DebugComputeRaw<T>(ComputeBuffer computeToDebug, string name, int length)
	{
		T[] data = new T[length];

		computeToDebug.GetData(data);

		string todebug = name + "ARRY: " + " @f:";
		for (int i = 0; i < length; i++)
		{
			todebug += data[i].ToString() + ", ";
		}

		Debug.LogWarning(todebug);
	}

	public static void DebugComputeGrid<T>(ComputeBuffer computeToDebug, string name, int dims)
	{
		T[] data = new T[dims* dims* dims];
		 
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
					todebug += data[ind].ToString() + ", ";
				}
			}
		}
		Debug.LogWarning(todebug);
	}

}
