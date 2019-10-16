using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public interface IWantQuadData
	{
		void GiveQuadData(ComputeBuffer[] quadDataAndAges);
	}
}
