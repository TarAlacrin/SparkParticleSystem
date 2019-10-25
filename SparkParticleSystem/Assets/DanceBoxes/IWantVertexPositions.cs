using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public interface IWantVertexPositions
	{
		void PassVertexPositions(ComputeBuffer[] buffers,  int vertexCount);
	}
}
