using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public class VertexPositionsToCubeAgeHandler : MonoBehaviour, IWantVertexPositions
	{
		public const int READ = 1;
		public const int WRITE = 0;

		public ComputeShader vertPosToCubeAgeCompute;

		public GameObject voxelAgeRecipientObject;
		IWantVoxelAges voxelAgeRecipient;

		ComputeBuffer[] vertexPositions;
		ComputeBuffer[] voxelAgeBuffer = new ComputeBuffer[2];
		ComputeBuffer triangleBuffer;
		public const string _vgKernelName = "CSMain"; //"CSBlockMain";//"CSTwirl";
		public int vgkernal
		{
			get
			{
				return vertPosToCubeAgeCompute.FindKernel(_vgKernelName);
			}
		}

		void Start()
		{
			voxelAgeBuffer[READ] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);
			voxelAgeBuffer[WRITE] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);
			voxelAgeRecipient = voxelAgeRecipientObject.GetComponent<IWantVoxelAges>();
			vertPosToCubeAgeCompute.SetVector("_Dimensions", DanceBoxManager.inst.voxelDimensions4);
		}


		void IWantVertexPositions.PassVertexPositions(ComputeBuffer[] buffers, int[] rawTriangleArray)
		{
			vertexPositions = buffers;
			TriangleData[] triangleArray = new TriangleData[rawTriangleArray.Length /3];
			for(int i = 0; i < triangleArray.Length; i++)
			{
				int i3 = i * 3;
				triangleArray[i] = new TriangleData(rawTriangleArray[i3], rawTriangleArray[i3 + 1], rawTriangleArray[i3 + 2]);
			}

			triangleBuffer = new ComputeBuffer(triangleArray.Length, sizeof(int) * 3, ComputeBufferType.Default);
			triangleBuffer.SetData(triangleArray);
			vertPosToCubeAgeCompute.SetBuffer(vgkernal, "RTriangleVertexes", triangleBuffer);
		}

		void Update()
		{
			if(vertexPositions !=null)
			{
				DoCollisions();
			}
		}

		void DoCollisions()
		{
			vertPosToCubeAgeCompute.SetBuffer(vgkernal, "RVertexPositions", vertexPositions[READ]);
			vertPosToCubeAgeCompute.SetBuffer(vgkernal, "WVoxelAgeBuffer", voxelAgeBuffer[WRITE]);
			vertPosToCubeAgeCompute.Dispatch(vgkernal, DanceBoxManager.inst.totalVoxels, 1, 1);

			voxelAgeRecipient.GiveSwappedVoxelAgeBuffer(voxelAgeBuffer[READ]);
		}

		private void OnDisable()
		{
			voxelAgeBuffer[READ].Dispose();
			voxelAgeBuffer[WRITE].Dispose();
			triangleBuffer.Dispose();
		}


		private void LateUpdate()
		{
			BufferTools.Swap(voxelAgeBuffer);
		}

	}

}
