using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanceBoxes
{
	public class DanceBoxManager : MonoBehaviour
	{
		public static DanceBoxManager inst;

		//128//512//1024//2048//4096//8192//16384//32768//65536//131072//262144//524288//1048576
		public int singleDimensionCount = 8;

		public Vector3 voxelDimensions{
			get{
				return new Vector3(singleDimensionCount, singleDimensionCount, singleDimensionCount);
			}
		}

		public Vector4 voxelDimensions4 {
			get {
				return new Vector4(voxelDimensions.x, voxelDimensions.y, voxelDimensions.z,0);
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


		private void Awake()
		{
			//Debug.Log("sizeOfVOXEL:" + sizeOfVoxelData + " SIZE OF QUAD?:" + sizeOfQuadData + " sizeofflot: " + sizeof(float));
			DanceBoxManager.inst = this;
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
}

