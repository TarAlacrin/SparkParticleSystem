using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public class DebugVoxelAgeGenerator : MonoBehaviour
	{

		public bool debug = false;
		public const int READ = 1;
		public const int WRITE = 0;

		public GameObject voxelAgeRecipientObject;
		IWantVoxelAges voxelAgeRecipient;

		public ComputeShader voxelAgeGenerator;

		ComputeBuffer[] voxelAgeBuffer = new ComputeBuffer[2];

		public const string _vgKernelName = "CSMain"; //"CSBlockMain";//"CSTwirl";
		public int vgkernal
		{
			get
			{
				return voxelAgeGenerator.FindKernel(_vgKernelName);
			}
		}

		void Start()
		{
			voxelAgeBuffer[READ] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);
			voxelAgeBuffer[WRITE] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);
			voxelAgeRecipient = voxelAgeRecipientObject.GetComponent<IWantVoxelAges>();
			voxelAgeGenerator.SetVector("_Dimensions", DanceBoxManager.inst.voxelDimensions4);
		}
		private void OnDisable()
		{
			voxelAgeBuffer[READ].Dispose();
			voxelAgeBuffer[WRITE].Dispose();
		}

		void Update()
		{
			voxelAgeGenerator.SetBuffer(vgkernal, "WVoxelAgeBuffer", voxelAgeBuffer[WRITE]);
			voxelAgeGenerator.Dispatch(vgkernal, DanceBoxManager.inst.totalVoxelsThreadGroup, 1, 1);// DanceBoxManager.inst.voxelDimX, DanceBoxManager.inst.voxelDimY, DanceBoxManager.inst.voxelDimZ);

			if (debug)
			{
				Debug.Log("Running");
				BufferTools.DebugComputeGrid<float>(voxelAgeBuffer[READ], "output voxel ages READ", DanceBoxManager.inst.singleDimensionCount);
			}

			voxelAgeRecipient.GiveSwappedVoxelAgeBuffer(voxelAgeBuffer[READ]);
		}

		private void LateUpdate()
		{
			BufferTools.Swap(voxelAgeBuffer);
		}
	}

}
