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
		ComputeBuffer[] vertexPositions;
		ComputeBuffer[] voxelAgeBuffer = new ComputeBuffer[2];

		ComputeBuffer[] triangleIntersectionBuffer = new ComputeBuffer[2];

		ComputeBuffer triangleBuffer;
		int triangleCount;
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


		void Start()
		{
			voxelAgeBuffer[READ] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);
			voxelAgeBuffer[WRITE] = new ComputeBuffer(DanceBoxManager.inst.totalVoxels, DanceBoxManager.inst.sizeOfVoxelData, ComputeBufferType.Default);
			triangleIntersectionARGSBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);

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
			vertPosToCubeAgeCompute.SetBuffer(mainKernel, "RTriangleVertexes", triangleBuffer);
			vertPosToCubeAgeCompute.SetBuffer(secondKernel, "RTriangleVertexes", triangleBuffer);

			triangleCount = triangleArray.Length;
			triangleIntersectionBuffer[READ] = new ComputeBuffer(triangleCount*(DanceBoxManager.inst.totalVoxels/2), DanceBoxManager.inst.sizeOfIntersectionData, ComputeBufferType.Append);
			triangleIntersectionBuffer[WRITE] = new ComputeBuffer(triangleCount*(DanceBoxManager.inst.totalVoxels/2), DanceBoxManager.inst.sizeOfIntersectionData, ComputeBufferType.Append);
		}

		void Update()
		{
			if(vertexPositions !=null && triangleCount >0)
			{
				DoCollisions();
			}
		}

		void DoCollisions()
		{
			vertPosToCubeAgeCompute.SetBuffer(mainKernel, "RVertexPositions", vertexPositions[READ]);
			triangleIntersectionBuffer[WRITE].SetCounterValue(0);
			vertPosToCubeAgeCompute.SetBuffer(mainKernel, "WAIntersections", triangleIntersectionBuffer[WRITE]);

			if (debug)
				BufferTools.DebugComputeRaw<Vector3>(vertexPositions[READ], "vertex pos check", triangleCount);

			vertPosToCubeAgeCompute.Dispatch(mainKernel, triangleCount, 1, 1);

			int[] args = BufferTools.GetArgs(triangleIntersectionBuffer[READ], triangleIntersectionARGSBuffer);

			Debug.Log("numintersections: " + args[0]);
			vertPosToCubeAgeCompute.SetInt("IntersectionCount", args[0]);

			vertPosToCubeAgeCompute.SetBuffer(secondKernel, "RVertexPositions", vertexPositions[READ]);
			vertPosToCubeAgeCompute.SetBuffer(secondKernel, "RAIntersections", triangleIntersectionBuffer[READ]);
			vertPosToCubeAgeCompute.SetBuffer(secondKernel, "WVoxelAgeBuffer", voxelAgeBuffer[WRITE]);

			vertPosToCubeAgeCompute.Dispatch(secondKernel, (int)(DanceBoxManager.inst.voxelDimensions.x * DanceBoxManager.inst.voxelDimensions.y), 1, 1);

			if(debug)
				BufferTools.DebugComputeRaw<float>(voxelAgeBuffer[READ], " SHOULD HAVE TOTALINTERSECTION COUNTS IN ALL TINGS", (int)DanceBoxManager.inst.voxelDimensions.x);

			//voxelAgeRecipient.GiveSwappedVoxelAgeBuffer(voxelAgeBuffer[READ]);
		}

		private void OnDisable()
		{
			voxelAgeBuffer[READ].Dispose();
			voxelAgeBuffer[WRITE].Dispose();
			triangleIntersectionBuffer[READ].Dispose();
			triangleIntersectionBuffer[WRITE].Dispose();
			triangleBuffer.Dispose();
			triangleIntersectionARGSBuffer.Dispose();
		}


		private void LateUpdate()
		{
			BufferTools.Swap(voxelAgeBuffer);
			BufferTools.Swap(triangleIntersectionBuffer);
		}

	}

}
