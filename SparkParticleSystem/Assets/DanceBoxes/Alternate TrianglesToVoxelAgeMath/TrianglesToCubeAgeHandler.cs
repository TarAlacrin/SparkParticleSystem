using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public class TrianglesToCubeAgeHandler : MonoBehaviour, IWantVertexPositions
	{
		public bool debug = false;

		public const int READ = 1;
		public const int WRITE = 0;

		public ComputeShader vertPosToCubeAgeCompute;

		public GameObject voxelAgeRecipientObject;
		IWantVoxelAges voxelAgeRecipient;


		ComputeBuffer[] triVertPosBuffers;

		ComputeBuffer[] filledVoxelGridBuffer = new ComputeBuffer[2];

		ComputeBuffer[] penDownVoxelBuffer = new ComputeBuffer[2];

		ComputeBuffer[] triangleIntersectionBuffer = new ComputeBuffer[2];
		ComputeBuffer triangleIntersectionARGSBuffer;

		int triangleCount;
		int vertexCount;
		public const string _intersectionsKernelName = "CSRunIntersections";
		public int intersectionsKernel
		{
			get
			{
				return vertPosToCubeAgeCompute.FindKernel(_intersectionsKernelName);
			}
		}

		public const string _intrsct2penPosKernelName = "CSIntersectionsToPenPos";
		public int intrsct2penPosKernel
		{
			get
			{
				return vertPosToCubeAgeCompute.FindKernel(_intrsct2penPosKernelName);
			}
		}

		public const string _penpos2VxlAgesKernelName = "CSPenposToVoxelAges"; 
		public int penpos2VxlAgesKernel
		{
			get
			{
				return vertPosToCubeAgeCompute.FindKernel(_penpos2VxlAgesKernelName);
			}
		}


		void Start()
		{
			filledVoxelGridBuffer[READ] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);
			filledVoxelGridBuffer[WRITE] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);

			penDownVoxelBuffer[WRITE] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, sizeof(float), ComputeBufferType.Default);
			penDownVoxelBuffer[READ] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, sizeof(float), ComputeBufferType.Default);

			triangleIntersectionARGSBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
			int[] argdata = new int[] { 0, 1, 0, 0 };
			triangleIntersectionARGSBuffer.SetData(argdata);

			voxelAgeRecipient = voxelAgeRecipientObject.GetComponent<IWantVoxelAges>();
			vertPosToCubeAgeCompute.SetVector("_Dimensions", DanceBoxManager.inst.voxelDimensions4);
			vertPosToCubeAgeCompute.SetVector("_InvDimensions", DanceBoxManager.inst.inverseVoxelDimensions4);
		}


		void IWantVertexPositions.PassVertexPositions(ComputeBuffer[] parVertexPositionBuffers, int parVertexCount)
		{
			triVertPosBuffers = parVertexPositionBuffers;
			triangleCount = parVertexCount / 3;

			int intersectionCount = (int)( (triangleCount * DanceBoxManager.inst.singleDimensionCount));
			triangleIntersectionBuffer[READ] = new ComputeBuffer(intersectionCount, DanceBoxManager.inst.sizeOfIntersectionData, ComputeBufferType.Append);
			triangleIntersectionBuffer[WRITE] = new ComputeBuffer(intersectionCount, DanceBoxManager.inst.sizeOfIntersectionData, ComputeBufferType.Append);
			//Debug.Log("INTERSECTION COUNT IS OFF THE CHART " + intersectionCount);

			vertexCount = parVertexCount;
			vertPosToCubeAgeCompute.SetInt("_MaxCountVertexBuffer", vertexCount);
		}

		void Update()
		{
			BufferTools.Swap(filledVoxelGridBuffer);
			BufferTools.Swap(triangleIntersectionBuffer);
			BufferTools.Swap(penDownVoxelBuffer);

			if (triVertPosBuffers != null && triangleCount > 0)
			{
				DoCollisions();
			}
		}


		void DoCollisions()
		{
			DoIntersections();
			DoPenDownCalculating();
			DoFillingVoxelGrid();
			voxelAgeRecipient.GiveSwappedVoxelAgeBuffer(filledVoxelGridBuffer[READ]);
		}




		void DoIntersections()
		{
			vertPosToCubeAgeCompute.SetBuffer(intersectionsKernel, "RTriangleVertexes", triVertPosBuffers[READ]);
			triangleIntersectionBuffer[WRITE].SetCounterValue(0);
			vertPosToCubeAgeCompute.SetBuffer(intersectionsKernel, "WAIntersections", triangleIntersectionBuffer[WRITE]);

			if (debug)
				BufferTools.DebugComputeRaw<Vector4>(triVertPosBuffers[READ], "unsorted vertex pos check", vertexCount);

			vertPosToCubeAgeCompute.Dispatch(intersectionsKernel, triangleCount, 1, 1);

		}

		void DoPenDownCalculating()
		{
			ComputeBuffer.CopyCount(triangleIntersectionBuffer[READ], triangleIntersectionARGSBuffer, 0);
			 
			vertPosToCubeAgeCompute.SetBuffer(intrsct2penPosKernel, "RArgsIntersectionCount", triangleIntersectionARGSBuffer);

			vertPosToCubeAgeCompute.SetBuffer(intrsct2penPosKernel, "RAIntersections", triangleIntersectionBuffer[READ]);
			vertPosToCubeAgeCompute.SetBuffer(intrsct2penPosKernel, "WPenPos", penDownVoxelBuffer[WRITE]);

			vertPosToCubeAgeCompute.Dispatch(intrsct2penPosKernel, (int)(DanceBoxManager.inst.totalVoxelsThreadGroup), 1, 1);
			//if (debug)
			//	BufferTools.DebugComputeRaw<float>(filledVoxelGridBuffer[READ], " SHOULD HAVE TOTALINTERSECTION COUNTS IN ALL TINGS", (int)DanceBoxManager.inst.voxelDimensions.x);
		}

		void DoFillingVoxelGrid()
		{
			vertPosToCubeAgeCompute.SetBuffer(penpos2VxlAgesKernel, "RPenPos", penDownVoxelBuffer[READ]);
			vertPosToCubeAgeCompute.SetBuffer(penpos2VxlAgesKernel, "WVoxelAgeBuffer", filledVoxelGridBuffer[WRITE]);

			vertPosToCubeAgeCompute.Dispatch(penpos2VxlAgesKernel, DanceBoxManager.inst.GetThreadNumbers((int)(DanceBoxManager.inst.voxelDimensions.x * DanceBoxManager.inst.voxelDimensions.y)), 1, 1);
		}

		private void OnDisable()
		{
			filledVoxelGridBuffer[READ].Dispose();
			filledVoxelGridBuffer[WRITE].Dispose();
			triangleIntersectionBuffer[READ].Dispose();
			triangleIntersectionBuffer[WRITE].Dispose();
			penDownVoxelBuffer[READ].Dispose();
			penDownVoxelBuffer[WRITE].Dispose();
			triangleIntersectionARGSBuffer.Dispose();
		}



	}

}
