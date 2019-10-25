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

		ComputeBuffer triangleIntersectionARGSBuffer;
		ComputeBuffer rawVertexPositionARGSBuffer;

		ComputeBuffer[] vertPosBuffersUnsorted;
		ComputeBuffer[] filledVoxelGridBuffer = new ComputeBuffer[2];

		ComputeBuffer[] vertPosBuffersSORTED = new ComputeBuffer[2];

		ComputeBuffer[] triangleIntersectionBuffer = new ComputeBuffer[2];

		ComputeBuffer triangleBuffer;

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


		void IWantVertexPositions.PassVertexPositions(ComputeBuffer[] vertexPositionBuffers, int[] rawTriangleArray, int vertCount)
		{
			vertPosBuffersUnsorted = vertexPositionBuffers;
			TriangleData[] triangleArray = new TriangleData[rawTriangleArray.Length /3];
			for(int i = 0; i < triangleArray.Length; i++)
			{
				int i3 = i * 3;
				triangleArray[i] = new TriangleData(rawTriangleArray[i3], rawTriangleArray[i3 + 1], rawTriangleArray[i3 + 2]);
			}

			triangleBuffer = new ComputeBuffer(triangleArray.Length, sizeof(int) * 3, ComputeBufferType.Default);
			triangleBuffer.SetData(triangleArray);
			vertPosToCubeAgeCompute.SetBuffer(mainKernel, "RTriangleVertexes", triangleBuffer);
			vertPosToCubeAgeCompute.SetBuffer(secondKernel, "RTriangleVertexes", triangleBuffer);

			triangleCount = triangleArray.Length;
			triangleIntersectionBuffer[READ] = new ComputeBuffer(triangleCount*(DanceBoxManager.inst.totalVoxels/2), DanceBoxManager.inst.sizeOfIntersectionData, ComputeBufferType.Append);
			triangleIntersectionBuffer[WRITE] = new ComputeBuffer(triangleCount*(DanceBoxManager.inst.totalVoxels/2), DanceBoxManager.inst.sizeOfIntersectionData, ComputeBufferType.Append);
			vertexCount = vertCount;
			vertPosBuffersSORTED[READ] = new ComputeBuffer(vertexCount, sizeof(float) *4, ComputeBufferType.Default);
			vertPosBuffersSORTED[WRITE] = new ComputeBuffer(vertexCount, sizeof(float) * 4, ComputeBufferType.Default);
			vertPosToCubeAgeCompute.SetInt("_MaxCountVertexBuffer", vertCount);
		}

		void Update()
		{
			BufferTools.Swap(filledVoxelGridBuffer);
			BufferTools.Swap(triangleIntersectionBuffer);
			BufferTools.Swap(vertPosBuffersSORTED);

			if (vertPosBuffersUnsorted !=null && triangleCount >0)
			{
				DoCollisions();
			}
		}

		void DoCollisions()
		{
			DoSorting();
			DoIntersections();
			DoFillingVoxelGrid();
			voxelAgeRecipient.GiveSwappedVoxelAgeBuffer(filledVoxelGridBuffer[READ]);
		}

		void DoSorting()
		{
			vertPosToCubeAgeCompute.SetBuffer(sortingKernel, "RARawVertexBuffer", vertPosBuffersUnsorted[READ]);
			vertPosToCubeAgeCompute.SetBuffer(sortingKernel, "WSortedVertexBuffer", vertPosBuffersSORTED[WRITE]);
			int[] rawvertargs = BufferTools.GetArgs(vertPosBuffersUnsorted[READ], rawVertexPositionARGSBuffer);
			vertPosToCubeAgeCompute.SetInt("_MaxCountVertexBuffer", rawvertargs[0]);

			if (rawvertargs[0] > 0)
				vertPosToCubeAgeCompute.Dispatch(sortingKernel, rawvertargs[0], 1, 1);
		}


		void DoIntersections()
		{
			vertPosToCubeAgeCompute.SetBuffer(mainKernel, "RVertexPositions", vertPosBuffersSORTED[READ]);
			triangleIntersectionBuffer[WRITE].SetCounterValue(0);
			vertPosToCubeAgeCompute.SetBuffer(mainKernel, "WAIntersections", triangleIntersectionBuffer[WRITE]);

			if (debug)
				BufferTools.DebugComputeRaw<Vector4>(vertPosBuffersSORTED[READ], "sorted vertex pos check", vertexCount);

			vertPosToCubeAgeCompute.Dispatch(mainKernel, triangleCount, 1, 1);

		}

		void DoFillingVoxelGrid()
		{
			int[] args = BufferTools.GetArgs(triangleIntersectionBuffer[READ], triangleIntersectionARGSBuffer);
			//Debug.Log("numintersections: " + args[0]);
			vertPosToCubeAgeCompute.SetInt("IntersectionCount", args[0]);

			vertPosToCubeAgeCompute.SetBuffer(secondKernel, "RVertexPositions", vertPosBuffersSORTED[READ]);
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
			vertPosBuffersSORTED[READ].Dispose();
			vertPosBuffersSORTED[WRITE].Dispose();
			triangleBuffer.Dispose();
			triangleIntersectionARGSBuffer.Dispose();
			rawVertexPositionARGSBuffer.Dispose();
		}


		private void LateUpdate()
		{
			/*BufferTools.Swap(voxelAgeBuffer);
			BufferTools.Swap(triangleIntersectionBuffer);
			BufferTools.Swap(vertPosBuffersSORTED);*/
		}

	}

}
