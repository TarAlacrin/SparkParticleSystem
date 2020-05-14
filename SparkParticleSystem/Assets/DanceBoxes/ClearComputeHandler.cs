using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DanceBoxes
{
	public class ClearComputeHandler : MonoBehaviour
	{
		public static ClearComputeHandler inst;

		public ComputeShader clearBufferShader;
		public const string _cbKernelName = "CSMain"; //"CSBlockMain";//"CSTwirl";
		public int cbkernal
		{
			get
			{
				return clearBufferShader.FindKernel(_cbKernelName);
			}
		}


		/*public const string _cbKernelNameSmallGroups = "CSMainSmall"; //"CSBlockMain";//"CSTwirl";
		public int cbkernalsmall
		{
			get
			{
				return clearBufferShader.FindKernel(_cbKernelNameSmallGroups);
			}
		}*/



		// Start is called before the first frame update
		void Awake()
		{
			ClearComputeHandler.inst = this;
		}

		public void ClearCompute(ComputeBuffer toClear, int NumThreadGroups, int sizeOfUnitIn32BitChunks, float fillValue = 0f)
		{
			//clearBufferShader.SetInt("_OneUnitSize", sizeOfUnitIn32BitChunks);
			clearBufferShader.SetBuffer(cbkernal, "WClearBuffer", toClear);
			//clearBufferShader.SetFloat("_FillValue", fillValue);
			clearBufferShader.Dispatch(cbkernal, NumThreadGroups, 1, 1);
		}


		/*public void ClearComputeSmall(ComputeBuffer toClear, int NumThreads, int sizeOfUnitIn32BitChunks, float fillValue = 0f)
		{
			clearBufferShader.SetInt("_OneUnitSize", sizeOfUnitIn32BitChunks);
			clearBufferShader.SetBuffer(cbkernalsmall, "WClearBuffer", toClear);
			clearBufferShader.SetFloat("_FillValue", fillValue);
			clearBufferShader.Dispatch(cbkernalsmall, NumThreads, 1, 1);
		}*/
	}
}