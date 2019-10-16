using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public interface IWantVoxelAges
	{
		void GiveSwappedVoxelAgeBuffer(ComputeBuffer voxelAgeStatesREAD);
	}
}
