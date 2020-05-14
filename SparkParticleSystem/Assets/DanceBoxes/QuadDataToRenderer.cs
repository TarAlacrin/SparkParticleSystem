using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public class QuadDataToRenderer : MonoBehaviour, IWantQuadData
	{
		public const int READ = 1;
		public const int WRITE = 0;

		ComputeBuffer[] quadDataBuffer;//gets this from a different class
		ComputeBuffer quadArgBuffer;

		public Material material;

		void Start()
		{
			quadArgBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
			int[] args = new int[] { 0, 1, 0, 0 };
			quadArgBuffer.SetData(args);
		}

		void IWantQuadData.GiveQuadData(ComputeBuffer[] quadDataAndAges)
		{
			this.quadDataBuffer = quadDataAndAges;
		}

		private void OnRenderObject()
		{
			if(quadDataBuffer != null)
			{
				ComputeBuffer.CopyCount(quadDataBuffer[READ], quadArgBuffer,0);
				material.SetPass(0);
				material.SetBuffer("_Data", quadDataBuffer[READ]);
				//BufferTools.DebugComputeRaw<int>(quadArgBuffer, "quadARGEBUFFEr", 4);
				Graphics.DrawProceduralIndirect(MeshTopology.Points, quadArgBuffer, 0);
			}
		}

		private void OnDisable()
		{
			quadArgBuffer.Dispose();
		}
	}
}
