using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferHandlerDanceBoxes : MonoBehaviour
{
	public const int READ = 1;
	public const int WRITE = 0;

	ComputeBuffer[] cubeAgeBuffer = new ComputeBuffer[2];
	ComputeBuffer[] quadDataBuffer = new ComputeBuffer[2];
	ComputeBuffer quadArgBuffer;


	//public ComputeShader cubeAgeFinalProcess;
	public ComputeShader cubeAgeToQuadDataShader;
	public Material material;
	

	//128//512//1024//2048//4096//8192//16384//32768//65536//131072//262144//524288//1048576
	public int count = 8;
	public int cubecount
	{
		get
		{
			return count * count * count;
		}
	}

	public const string _ca2qdKernelName = "CSMain"; //"CSBlockMain";//"CSTwirl";
	public int ca2qdkernal
	{
		get
		{
			return cubeAgeToQuadDataShader.FindKernel(_ca2qdKernelName);
		}
	}


	struct QuadData
	{
		Vector3 position;
		Vector3 normal;
		float age;
	}


	// Start is called before the first frame update
	void Start()
    {
		//quadDataBuffer[READ] = new ComputeBuffer(cubecount * 6, 7 * sizeof(float), ComputeBufferType.Append);
		//quadDataBuffer[WRITE] = new ComputeBuffer(cubecount * 6, 7 * sizeof(float), ComputeBufferType.Append);

		//cubeAgeBuffer[READ] = new ComputeBuffer(cubecount, sizeof(float), ComputeBufferType.Default);
		//cubeAgeBuffer[WRITE] = new ComputeBuffer(cubecount, sizeof(float), ComputeBufferType.Default);


		//quadArgBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);


		//GetCubeAge();
		//CubeAgeToQuad();
	}


	//void ()
	//{GetCubeAge
		//cubeAgeFinalProcess.SetBuffer(cubeAgeBuffer[WRITE],)
		//Swap(cubeAgeBuffer);
	//}


	void CubeAgeToQuad()
	{
		cubeAgeToQuadDataShader.SetBuffer(ca2qdkernal, "RCubeAges", cubeAgeBuffer[READ]);
		cubeAgeToQuadDataShader.SetBuffer(ca2qdkernal, "WQuadPositionAndAgeBuffer", quadDataBuffer[WRITE]);
		cubeAgeToQuadDataShader.SetInt("dimensionSize", count);
		quadDataBuffer[WRITE].SetCounterValue(0);
		cubeAgeToQuadDataShader.Dispatch(ca2qdkernal, count, count, count);
		Swap(quadDataBuffer);
	}

	// Update is called once per frame
	void Update()
    {
	}


	private void OnRenderObject()
	{
		int[] quadargs = GetArgs(quadDataBuffer[READ], quadArgBuffer);
		Debug.Log("Quadargs: " + quadargs[0] + ", " + quadargs[1] + ", " + quadargs[2] + ", " + quadargs[3]);
		material.SetPass(0); 
		material.SetBuffer("_Data", quadDataBuffer[READ]);
		Graphics.DrawProceduralIndirect(MeshTopology.Points, quadArgBuffer, 0);
	}

	private void OnDisable()
	{
		Debug.Log("DESTROYED!");
		quadDataBuffer[READ].Release();
		quadDataBuffer[WRITE].Release();
		quadArgBuffer.Release();
	}



	void Swap(ComputeBuffer[] buffer)
	{
		ComputeBuffer tmp = buffer[READ];
		buffer[READ] = buffer[WRITE];
		buffer[WRITE] = tmp;
	}

	int[] GetArgs(ComputeBuffer compOBuffToCheck, ComputeBuffer pargsBuffer)
	{
		int[] args = new int[] { 0, 1, 0, 0 };
		pargsBuffer.SetData(args);
		ComputeBuffer.CopyCount(compOBuffToCheck, pargsBuffer, 0);
		pargsBuffer.GetData(args);
		return args;
	}
}
