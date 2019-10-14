using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferHandlerDanceBoxes : MonoBehaviour
{
	public const int READ = 1;
	public const int WRITE = 0;

	ComputeBuffer[] quadDataBuffers = new ComputeBuffer[2];
	ComputeBuffer quadDataArgBuffer;

	public ComputeShader cShade;
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

	public const string _SimKernelName = "CSMain"; //"CSBlockMain";//"CSTwirl";
	public int simkernal
	{
		get
		{
			return cShade.FindKernel(_SimKernelName);
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
		quadDataBuffers[READ] = new ComputeBuffer(cubecount * 6, 7 * sizeof(float), ComputeBufferType.Append);
		quadDataBuffers[WRITE] = new ComputeBuffer(cubecount * 6, 7 * sizeof(float), ComputeBufferType.Append);

		quadDataBuffers[WRITE].SetCounterValue(0);
		quadDataArgBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
		cShade.SetBuffer(simkernal, "WQuadPositionAndAgeBuffer", quadDataBuffers[WRITE]);
		cShade.Dispatch(simkernal, count, count, count);
		Swap(quadDataBuffers);
		//cShade.SetBuffer(simkernal, "WQuadPositionAndAgeBuffer", quadDataBuffers[WRITE]);
		//cShade.Dispatch(simkernal, count, count, count);

		//int[] quadargs = new int[] { 0, 1, 0, 0 };
		//quadDataArgBuffer.SetData(quadargs);

	}

	// Update is called once per frame
	void Update()
    {
	}


	private void OnRenderObject()
	{
		int[] quadargs = GetArgs(quadDataBuffers[READ], quadDataArgBuffer);
		Debug.Log("Quadargs: " + quadargs[0] + ", " + quadargs[1] + ", " + quadargs[2] + ", " + quadargs[3]);
		material.SetPass(0); 
		material.SetBuffer("_Data", quadDataBuffers[READ]);
		Graphics.DrawProceduralIndirect(MeshTopology.Points, quadDataArgBuffer, 0);
	}

	private void OnDisable()
	{
		Debug.Log("DESTROYED!");
		quadDataBuffers[READ].Release();
		quadDataBuffers[WRITE].Release();
		quadDataArgBuffer.Release();
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
