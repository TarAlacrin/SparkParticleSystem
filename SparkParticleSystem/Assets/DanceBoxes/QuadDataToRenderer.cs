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
		}

		void IWantQuadData.GiveQuadData(ComputeBuffer[] quadDataAndAges)
		{
			this.quadDataBuffer = quadDataAndAges;
		}

		private void OnRenderObject()
		{
			if(quadDataBuffer != null)
			{
				int[] quadargs = BufferTools.GetArgs(quadDataBuffer[READ], quadArgBuffer);
				material.SetPass(0);
				material.SetBuffer("_Data", quadDataBuffer[READ]);

				Graphics.DrawProceduralIndirect(MeshTopology.Points, quadArgBuffer, 0);
			}
		}

		private void OnDisable()
		{
			quadArgBuffer.Dispose();
		}
	}
}
