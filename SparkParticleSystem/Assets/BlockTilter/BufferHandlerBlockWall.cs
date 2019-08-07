using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BufferHandlerBlockWall : MonoBehaviour
{
	public const int READ = 1;
	public const int WRITE = 0;
	//public Material material;

	public RenderTexture inVid;
	ComputeBuffer[] BlockBufferGoal = new ComputeBuffer[2];
	ComputeBuffer[] BlockBufferCur = new ComputeBuffer[2];



	//ComputeBuffer[] ColBuffer = new ComputeBuffer[2];



	public ComputeShader cShade;


	//128//512//1024//2048//4096//8192//16384//32768//65536//131072//262144//524288//1048576
	public int count = 512;


	int sqrtCount;

	public const string _SimKernelName = "CSBlockMain";//"CSTwirl";


	public static BufferHandlerBlockWall inst;



	public int frameSpacing = 0;
	int curFrameCounter = 0;

	public float simSpeed = 1f;
	[HideInInspector]
	public float IsPlaying = 1f;

	bool isPause = true;

	

	void Start()
	{
		Application.targetFrameRate = -1;
		BufferHandlerBlockWall.inst = this;
		int sqrtCount = (int)Mathf.Sqrt(count);

		//maybe eventually expand these out to also include a color datapoint
		BlockBufferGoal[READ]  = new ComputeBuffer(count, sizeof(float) * 4, ComputeBufferType.Default);
		BlockBufferGoal[WRITE] = new ComputeBuffer(count, sizeof(float) * 4, ComputeBufferType.Default);



		BlockBufferCur[READ]  = new ComputeBuffer(count, sizeof(float) * 4, ComputeBufferType.Default);
		BlockBufferCur[WRITE] = new ComputeBuffer(count, sizeof(float) * 4, ComputeBufferType.Default);

		Vector4[] initialBlkData = new Vector4[count];

		for(int i = 0; i < initialBlkData.Length; i++)
		{
			initialBlkData[i] = new Vector4(0.75f, 0.25f, 0f, 0f);
		}

		BlockBufferCur[READ].SetData(initialBlkData);
		BlockBufferCur[WRITE].SetData(initialBlkData);


		BlockBufferGoal[READ].SetData(initialBlkData);
		BlockBufferGoal[WRITE].SetData(initialBlkData);




	}

	void Update()
	{
		//causes the gpu to only update on certain frames
		if(curFrameCounter >= frameSpacing)
		{
			DoUpdate();
			curFrameCounter = 0;
		}

		curFrameCounter++;
	}


	void DoUpdate()
	{


	}

	void Swap(ComputeBuffer[] buffer) 
	{
		ComputeBuffer tmp = buffer[READ];
		buffer[READ] = buffer[WRITE];
		buffer[WRITE] = tmp;
	}


	int[] GetArgs(ComputeBuffer compOBuffToCheck, ComputeBuffer pargsBuffer)
	{
		int[] args = new int[]{ 0, 1, 0, 0 };
		pargsBuffer.SetData(args);
		ComputeBuffer.CopyCount(compOBuffToCheck, pargsBuffer, 0);
		pargsBuffer.GetData(args);

		return args;
	}





	void DoSimming()
	{
		
		//SET THE BUFFERS FOR THE SIM:
		//cShade.SetBuffer(cShade.FindKernel(_SimKernelName), "WvertDat", IdAgeBuffer[WRITE]);
		//cShade.SetBuffer(cShade.FindKernel(_SimKernelName), "RvertDat", IdAgeBuffer[READ]);
		//
		//cShade.SetBuffer(cShade.FindKernel(_SimKernelName), "RvertPos", PosBuffer[READ]);
		//cShade.SetBuffer(cShade.FindKernel(_SimKernelName), "RvertVel", VelBuffer[READ]);
		//cShade.SetBuffer(cShade.FindKernel(_SimKernelName), "WvertPos", PosBuffer[WRITE]);
		//cShade.SetBuffer(cShade.FindKernel(_SimKernelName), "WvertVel", VelBuffer[WRITE]);
		//
		//cShade.SetBuffer(cShade.FindKernel(_SimKernelName), "AliveList", LiveBuffer);		
		//cShade.SetBuffer(cShade.FindKernel(_SimKernelName), "AdeadList", DeadBuffer);
		//
		//cShade.SetBuffer(cShade.FindKernel(_SimKernelName), "RcubeDat", CubeDimBuffer[READ]);
		//
		//
		//this.cubeMat.SetBuffer("_ParticlePosition", PosBuffer[READ]);
		//this.cubeMat.SetBuffer("_LivingParticles", LiveBuffer);
		//this.cubeMat.SetInt("_LiveParticleCount", this.liveParticles);
		//
		////RUN THE SIM
		//cShade.Dispatch(cShade.FindKernel(_SimKernelName), count/64, 1, 1);

	}



	void OnRenderObject()
	{

		//int[] args = GetArgs(LiveBuffer, argBuffer);
		//this.liveParticles = args[0];
		//
		//int[] dedargs = GetArgs(DeadBuffer, deadBuffArgBuff);
		//this.deadParticles = dedargs[0];


        /*lightHaloMat.SetPass(0);
        lightHaloMat.SetBuffer("_VertPos", PosBuffer[READ]);
        lightHaloMat.SetBuffer("_VertVel", VelBuffer[READ]);
        lightHaloMat.SetBuffer("_VertDat", IdAgeBuffer[READ]);
        lightHaloMat.SetBuffer("_LivingID", LiveBuffer);

        Graphics.DrawProceduralIndirect(MeshTopology.Points, argBuffer);
		*/
       //material.SetPass(0);
       //material.SetBuffer("_VertPos", PosBuffer[READ]);
       //material.SetBuffer("_VertVel", VelBuffer[READ]);
       //material.SetBuffer("_VertDat", IdAgeBuffer[READ]);
       //material.SetBuffer("_LivingID", LiveBuffer);

        //Graphics.DrawProceduralIndirect(MeshTopology.Points, argBuffer);

	}
	
	void OnDestroy()
	{
		//PosBuffer[READ].Release();
		//VelBuffer[READ].Release();
		//PosBuffer[WRITE].Release();
		//VelBuffer[WRITE].Release();

		//IdAgeBuffer[WRITE].Release();
		//IdAgeBuffer[READ].Release();

		//ColBuffer[WRITE].Release();
		//ColBuffer[READ].Release();

		//argBuffer.Release();
		//this.deadBuffArgBuff.Release();

		//DeadBuffer.Release();
		//LiveBuffer.Release();

		//CubeDimBuffer[READ].Release();
		//CubeDimBuffer[WRITE].Release();

	}





}


