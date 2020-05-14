using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public class VoxelAgesToQuadData : MonoBehaviour, IWantVoxelAges
	{
		private bool debug = false;//dont use. Probably crashes
		public const int READ = 1;
		public const int WRITE = 0;

		ComputeBuffer[] quadDataBuffer = new ComputeBuffer[2];

		public const string _ca2qdKernelName = "CSMain"; //"CSBlockMain";//"CSTwirl";
		public int ca2qdkernal{
			get{
				return cubeAgeToQuadDataShader.FindKernel(_ca2qdKernelName);
			}
		}

		public ComputeShader cubeAgeToQuadDataShader;


		public GameObject quadDataToRendererObject;
		IWantQuadData quadDataOutput;

		void Start()
		{
			for(int i =0; i < quadDataBuffer.Length; i++)
			{
				quadDataBuffer[i] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels * 6, DanceBoxManager.inst.sizeOfQuadData, ComputeBufferType.Append);
			}
			//quadDataBuffer[WRITE] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels * 6, DanceBoxManager.inst.sizeOfQuadData, ComputeBufferType.Append);
			cubeAgeToQuadDataShader.SetVector("_Dimensions", DanceBoxManager.inst.voxelDimensions4);
			cubeAgeToQuadDataShader.SetVector("_InvDimensions", DanceBoxManager.inst.inverseVoxelDimensions4);

			quadDataOutput = quadDataToRendererObject.GetComponent<IWantQuadData>();
		}

		void IWantVoxelAges.GiveSwappedVoxelAgeBuffer(ComputeBuffer voxelAgeStates)
		{
			CubeAgeToQuad(voxelAgeStates);
		}

		void CubeAgeToQuad(ComputeBuffer voxelAgeStatesREAD)
		{
			//if (debug)
			//{
			//	debug = false;
			//	BufferTools.DebugComputeGrid<float>(voxelAgeStatesREAD, "voxel Age state READ - ", DanceBoxManager.inst.singleDimensionCount);
			//}


			cubeAgeToQuadDataShader.SetVector("_Dimensions", DanceBoxManager.inst.voxelDimensions4);
			cubeAgeToQuadDataShader.SetVector("_InvDimensions", DanceBoxManager.inst.inverseVoxelDimensions4);
			cubeAgeToQuadDataShader.SetFloat("_TIMETIME", Time.time);
			cubeAgeToQuadDataShader.SetBuffer(ca2qdkernal, "RCubeAges", voxelAgeStatesREAD);
			cubeAgeToQuadDataShader.SetBuffer(ca2qdkernal, "WQuadPositionAndAgeBuffer", quadDataBuffer[WRITE]);

			quadDataBuffer[WRITE].SetCounterValue(0);//"erases" data from previous frame
			cubeAgeToQuadDataShader.Dispatch(ca2qdkernal, DanceBoxManager.inst.totalVoxelsThreadGroup, 1,1);


			if (debug)
			{
				BufferTools.DebugComputeRaw<QuadData>(quadDataBuffer[READ], "outquadata - ", DanceBoxManager.inst.singleDimensionCount);
			}

			quadDataOutput.GiveQuadData(quadDataBuffer);
		}


		void ClearBuffer(int index)
		{
			ClearComputeHandler.inst.ClearCompute(quadDataBuffer[index],
			DanceBoxManager.inst.totalVoxelsThreadGroup * 6,
			1);
		}



		IEnumerator PostRenderUpdate()
		{
			while(this.enabled)
			{
				yield return new WaitForEndOfFrame();
				BufferTools.Swap(quadDataBuffer);//now the data is readable
				ClearBuffer(WRITE);
			}
		}


		private void OnEnable()
		{
			this.StartCoroutine(PostRenderUpdate());
		}



		private void OnDisable()
		{
			this.StopAllCoroutines();
			quadDataBuffer[READ].Dispose();
			quadDataBuffer[WRITE].Dispose();
		}


	}
}
