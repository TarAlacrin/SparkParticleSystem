using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public class VertexPositionsToCubeAgeHandler : MonoBehaviour, IWantVertexPositions
	{
		public bool debug = false;

		public const int READ = 1;
		public const int WRITE = 0;

		public ComputeShader vertPosToCubeAgeCompute;

		public GameObject voxelAgeRecipientObject;
		IWantVoxelAges voxelAgeRecipient;


		ComputeBuffer[] triVertPosBuffers;
		ComputeBuffer rawVertexPositionARGSBuffer;

		ComputeBuffer[] filledVoxelGridBuffer = new ComputeBuffer[2];

		//ComputeBuffer[] vertPosBuffersSORTED = new ComputeBuffer[2];

		ComputeBuffer[] triangleIntersectionBuffer = new ComputeBuffer[2];
		ComputeBuffer triangleIntersectionARGSBuffer;

		//ComputeBuffer triangleBuffer;

		int triangleCount;
		int vertexCount;
		public const string _mainKernelName = "CSMain"; //"CSBlockMain";//"CSTwirl";
		public int mainKernel
		{
			get
			{
				return vertPosToCubeAgeCompute.FindKernel(_mainKernelName);
			}
		}

		public const string _secondKernelName = "CSSecond"; //"CSBlockMain";//"CSTwirl";
		public int secondKernel
		{
			get
			{
				return vertPosToCubeAgeCompute.FindKernel(_secondKernelName);
			}
		}

		public const string _sortingKernelName = "CSSort"; //"CSBlockMain";//"CSTwirl";
		public int sortingKernel
		{
			get
			{
				return vertPosToCubeAgeCompute.FindKernel(_sortingKernelName);
			}
		}


		void Start()
		{
			filledVoxelGridBuffer[READ] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);
			filledVoxelGridBuffer[WRITE] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);
			triangleIntersectionARGSBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
			rawVertexPositionARGSBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);

			voxelAgeRecipient = voxelAgeRecipientObject.GetComponent<IWantVoxelAges>();
			vertPosToCubeAgeCompute.SetVector("_Dimensions", DanceBoxManager.inst.voxelDimensions4);
		}


		void IWantVertexPositions.PassVertexPositions(ComputeBuffer[] parVertexPositionBuffers, int parVertexCount)
		{
			triVertPosBuffers = parVertexPositionBuffers;
			triangleCount = parVertexCount / 3;
			triangleIntersectionBuffer[READ] = new ComputeBuffer(triangleCount*(DanceBoxManager.inst.totalVoxels), DanceBoxManager.inst.sizeOfIntersectionData, ComputeBufferType.Append);
			triangleIntersectionBuffer[WRITE] = new ComputeBuffer(triangleCount*(DanceBoxManager.inst.totalVoxels), DanceBoxManager.inst.sizeOfIntersectionData, ComputeBufferType.Append);
			vertexCount = parVertexCount;
			vertPosToCubeAgeCompute.SetInt("_MaxCountVertexBuffer", vertexCount);
		}

		void Update()
		{
			BufferTools.Swap(filledVoxelGridBuffer);
			BufferTools.Swap(triangleIntersectionBuffer);

			if (triVertPosBuffers != null && triangleCount > 0)
			{
				DoCollisions();
			}


		}

		private void LateUpdate()
		{
		}


		void DoCollisions()
		{
			//DoSorting();
			DoIntersections();
			DoFillingVoxelGrid();
			voxelAgeRecipient.GiveSwappedVoxelAgeBuffer(filledVoxelGridBuffer[READ]);
		}

		void DoSorting()
		{
			/*vertPosToCubeAgeCompute.SetBuffer(sortingKernel, "RARawVertexBuffer", triVertPosBuffers[READ]);
			vertPosToCubeAgeCompute.SetBuffer(sortingKernel, "WSortedVertexBuffer", vertPosBuffersSORTED[WRITE]);
			int[] rawvertargs = BufferTools.GetArgs(triVertPosBuffers[READ], rawVertexPositionARGSBuffer);
			vertPosToCubeAgeCompute.SetInt("_MaxCountVertexBuffer", rawvertargs[0]);

			if (rawvertargs[0] > 0)
				vertPosToCubeAgeCompute.Dispatch(sortingKernel, rawvertargs[0], 1, 1);
		*/}


		void DoIntersections()
		{
			vertPosToCubeAgeCompute.SetBuffer(mainKernel, "RTriangleVertexes", triVertPosBuffers[READ]);
			triangleIntersectionBuffer[WRITE].SetCounterValue(0);
			vertPosToCubeAgeCompute.SetBuffer(mainKernel, "WAIntersections", triangleIntersectionBuffer[WRITE]);

			if (debug)
				BufferTools.DebugComputeRaw<Vector4>(triVertPosBuffers[READ], "unsorted vertex pos check", vertexCount);

			vertPosToCubeAgeCompute.Dispatch(mainKernel, triangleCount, 1, 1);

		}

		void DoFillingVoxelGrid()
		{
			int[] args = BufferTools.GetArgs(triangleIntersectionBuffer[READ], triangleIntersectionARGSBuffer);
			Debug.Log("numintersections: " + args[0]);
			vertPosToCubeAgeCompute.SetInt("IntersectionCount", args[0]);

			vertPosToCubeAgeCompute.SetBuffer(secondKernel, "RTriangleVertexes", triVertPosBuffers[READ]);
			vertPosToCubeAgeCompute.SetBuffer(secondKernel, "RAIntersections", triangleIntersectionBuffer[READ]);
			vertPosToCubeAgeCompute.SetBuffer(secondKernel, "WVoxelAgeBuffer", filledVoxelGridBuffer[WRITE]);

			vertPosToCubeAgeCompute.Dispatch(secondKernel, (int)(DanceBoxManager.inst.voxelDimensions.x * DanceBoxManager.inst.voxelDimensions.y), 1, 1);
			//if (debug)
			//	BufferTools.DebugComputeRaw<float>(filledVoxelGridBuffer[READ], " SHOULD HAVE TOTALINTERSECTION COUNTS IN ALL TINGS", (int)DanceBoxManager.inst.voxelDimensions.x);
		}

		private void OnDisable()
		{
			filledVoxelGridBuffer[READ].Dispose();
			filledVoxelGridBuffer[WRITE].Dispose();
			triangleIntersectionBuffer[READ].Dispose();
			triangleIntersectionBuffer[WRITE].Dispose();
			//vertPosBuffersSORTED[READ].Dispose();
			//vertPosBuffersSORTED[WRITE].Dispose();
			//triangleBuffer.Dispose();
			triangleIntersectionARGSBuffer.Dispose();
			rawVertexPositionARGSBuffer.Dispose();
		}



	}

}
