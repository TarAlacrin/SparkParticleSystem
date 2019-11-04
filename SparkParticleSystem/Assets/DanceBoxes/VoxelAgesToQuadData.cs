using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public class VoxelAgesToQuadData : MonoBehaviour, IWantVoxelAges
	{
		public bool debug = false;
		public const int READ = 1;
		public const int WRITE = 0;

		ComputeBuffer[] quadDataBuffer = new ComputeBuffer[2];

		public const string _ca2qdKernelName = "CSMain"; //"CSBlockMain";//"CSTwirl";
		public int ca2qdkernal
		{
			get
			{
				return cubeAgeToQuadDataShader.FindKernel(_ca2qdKernelName);
			}
		}

		public ComputeShader cubeAgeToQuadDataShader;

		public GameObject quadDataToRendererObject;
		IWantQuadData quadDataOutput;

		void Start()
		{
			quadDataBuffer[READ] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels * 6, DanceBoxManager.inst.sizeOfQuadData, ComputeBufferType.Append);
			quadDataBuffer[WRITE] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels * 6, DanceBoxManager.inst.sizeOfQuadData, ComputeBufferType.Append);
			cubeAgeToQuadDataShader.SetVector("_Dimensions", DanceBoxManager.inst.voxelDimensions4);
			quadDataOutput = quadDataToRendererObject.GetComponent<IWantQuadData>();
		}

		void IWantVoxelAges.GiveSwappedVoxelAgeBuffer(ComputeBuffer voxelAgeStates)
		{
			CubeAgeToQuad(voxelAgeStates);
		}


		void CubeAgeToQuad(ComputeBuffer voxelAgeStatesREAD)
		{
			if (debug)
			{
				BufferTools.DebugComputeGrid<float>(voxelAgeStatesREAD, "voxel Age state READ - ", DanceBoxManager.inst.singleDimensionCount);
			}

			cubeAgeToQuadDataShader.SetVector("_Dimensions", DanceBoxManager.inst.voxelDimensions4);
			cubeAgeToQuadDataShader.SetBuffer(ca2qdkernal, "RCubeAges", voxelAgeStatesREAD);
			cubeAgeToQuadDataShader.SetBuffer(ca2qdkernal, "WQuadPositionAndAgeBuffer", quadDataBuffer[WRITE]);
			quadDataBuffer[WRITE].SetCounterValue(0);//erases data from previous frame
			cubeAgeToQuadDataShader.Dispatch(ca2qdkernal, DanceBoxManager.inst.totalVoxelsThreadGroup, 1,1);

			quadDataOutput.GiveQuadData(quadDataBuffer);
		}


		void LateUpdate()
		{
			BufferTools.Swap(quadDataBuffer);//now the data is readable
		}



		private void OnDisable()
		{
			quadDataBuffer[READ].Dispose();
			quadDataBuffer[WRITE].Dispose();
		}


	}
}
