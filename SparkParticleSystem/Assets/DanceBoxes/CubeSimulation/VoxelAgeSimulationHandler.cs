using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public class VoxelAgeSimulationHandler : MonoBehaviour, IWantVoxelAges
	{
		public float DecaySpeed = 5f;
		public const int READ = 1;
		public const int WRITE = 0;

		public GameObject voxelAgeRecipientObject;
		IWantVoxelAges voxelAgeRecipient;
		public ComputeShader cubeAgeSimulationShader;

		ComputeBuffer[] voxelAgeBuffer = new ComputeBuffer[2];

		public const string _casKernelName = "CSMain";
		public int caskernal
		{
			get
			{
				return cubeAgeSimulationShader.FindKernel(_casKernelName);
			}
		}

		void Start()
		{
			for (int i = 0; i < voxelAgeBuffer.Length; i++)
			{
				voxelAgeBuffer[i] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);
			}

			cubeAgeSimulationShader.SetVector("_Dimensions", DanceBoxManager.inst.voxelDimensions4);
			cubeAgeSimulationShader.SetVector("_InvDimensions", DanceBoxManager.inst.inverseVoxelDimensions4);
			voxelAgeRecipient = voxelAgeRecipientObject.GetComponent<IWantVoxelAges>();
		}

		void IWantVoxelAges.GiveSwappedVoxelAgeBuffer(ComputeBuffer voxelAgeStatesREAD)
		{
			BufferTools.Swap(voxelAgeBuffer);

			cubeAgeSimulationShader.SetVector("_Dimensions", DanceBoxManager.inst.voxelDimensions4);
			cubeAgeSimulationShader.SetVector("_InvDimensions", DanceBoxManager.inst.inverseVoxelDimensions4);
			cubeAgeSimulationShader.SetFloat("_DeltaTime", Time.deltaTime* DecaySpeed);

			cubeAgeSimulationShader.SetBuffer(caskernal, "ROldCubeAges", voxelAgeBuffer[READ]);
			cubeAgeSimulationShader.SetBuffer(caskernal, "RNewCubeAges", voxelAgeStatesREAD);
			cubeAgeSimulationShader.SetBuffer(caskernal, "WCubeAges", voxelAgeBuffer[WRITE]);

			cubeAgeSimulationShader.Dispatch(caskernal, DanceBoxManager.inst.totalVoxelsThreadGroup, 1, 1);
			voxelAgeRecipient.GiveSwappedVoxelAgeBuffer(voxelAgeBuffer[READ]);
		}


		private void OnDisable()
		{
			voxelAgeBuffer[READ].Dispose();
			voxelAgeBuffer[WRITE].Dispose();
		}
	}




}
