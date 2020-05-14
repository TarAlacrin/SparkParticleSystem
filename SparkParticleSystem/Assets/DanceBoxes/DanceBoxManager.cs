using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public class DanceBoxManager : MonoBehaviour
	{
		public static DanceBoxManager inst;

		//128//512//1024//2048//4096//8192//16384//32768//65536//131072//262144//524288//1048576
		public int singleDimensionCount = 64;

		[Space]
		private int threadGroupSize = 64;
		[Space]
		public Transform sizeAdjuster = null;

		public int captureFrametime = 20;

		public Vector3 voxelDimensions{
			get{
				return new Vector3(singleDimensionCount, singleDimensionCount, singleDimensionCount);
			}
		}

		public Vector4 voxelDimensions4 {
			get {
				return new Vector4(voxelDimensions.x, voxelDimensions.y, voxelDimensions.z, voxelDimensions.x* voxelDimensions.y);
			}
		}

		public Vector4 inverseVoxelDimensions4
		{
			get
			{
				return new Vector4(1f/voxelDimensions.x, 1f/voxelDimensions.y, 1f/voxelDimensions.z, 1f/(voxelDimensions.x * voxelDimensions.y));
			}
		}



		public int voxelDimX{
			get { return (int)voxelDimensions.x; }
		}
		public int voxelDimY{
			get { return (int)voxelDimensions.y; }
		}
		public int voxelDimZ{
			get { return (int)voxelDimensions.z; }
		}

		public int totalVoxels{
			get{
				return (int)(voxelDimensions.x * voxelDimensions.y * voxelDimensions.z);
			}
		}

		public int totalVoxelsThreadGroup {
			get	{
				return GetThreadNumbers(totalVoxels);
			}
		}

		public int sizeOfQuadData{
			get{
				return System.Runtime.InteropServices.Marshal.SizeOf<QuadData>();
			}
		}
		public int sizeOfVoxelData{
			get{
				return System.Runtime.InteropServices.Marshal.SizeOf<VoxelData>();
			}
		}
		public int sizeOfIntersectionData{
			get{
				return System.Runtime.InteropServices.Marshal.SizeOf<IntersectionData>();
			}
		}


		private void Awake()
		{
			if (sizeAdjuster != null)
				sizeAdjuster.localScale = voxelDimensions;
			//Debug.Log("sizeOfVOXEL:" + sizeOfVoxelData + " SIZE OF QUAD?:" + sizeOfQuadData + " sizeofflot: " + sizeof(float));
			DanceBoxManager.inst = this;
			if(captureFrametime >0)
			Time.captureFramerate = captureFrametime;
		}

		public int GetThreadNumbers(int inDesiredCount)
		{

			float countCalc = inDesiredCount;
			countCalc /= (float)threadGroupSize;
			if (countCalc != Mathf.Round(countCalc))
			{
				Debug.LogError("HEY! IM TRYING TO INVOKE PART OF A THREADGROUP: " + countCalc + " IS THE NUMBER OF THREADGROUPS IM TRYING TO INVOKE");
			}

			return inDesiredCount / threadGroupSize;
		}

	}



	struct QuadData
	{
		Vector3 position;
		Vector3 normal;
		float age;

		public override string ToString()
		{
			return "" + position;
		} 
	}

	struct VoxelData
	{
		float age;
	}

	struct TriangleData
	{
		int p1;
		int p2;
		int p3;
		public TriangleData(int i1, int i2, int i3)
		{
			p1 = i1; p2 = i2; p3 = i3;
		}
	}

	struct IntersectionData
	{
		float intersectionDistanceAndNormal;
		int voxelID;
		int triangleID;
	}
}

