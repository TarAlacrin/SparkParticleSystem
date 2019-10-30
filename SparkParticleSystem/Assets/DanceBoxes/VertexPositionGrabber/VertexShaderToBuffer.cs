// https://github.com/przemyslawzaworski
// Set plane position (Y=0.0)
// Collider is generated from vertex shader, using RWStructuredBuffer to transfer data.

using UnityEngine;
using System.Collections.Generic;

namespace DanceBoxes
{
	public class VertexShaderToBuffer : MonoBehaviour
	{

		public bool debug = false;
		public const int READ = 1;
		public const int WRITE = 0;

		public Material DisplayMAT;
		public Material material;
		ComputeBuffer[] triVertexPositionBuffer = new ComputeBuffer[2];
		ComputeBuffer argBuffer;
		Mesh mesh;
		Vector4[] initData;

		public GameObject vertexPositionRecieverObject;
		IWantVertexPositions vertexReciever;


		int vertCount;

		void Start()
		{
			vertexReciever = vertexPositionRecieverObject.GetComponent<IWantVertexPositions>();

			MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
			if (mf != null)
				mesh = mf.mesh;
			else
			{
				SkinnedMeshRenderer smr = this.gameObject.GetComponent<SkinnedMeshRenderer>();
				mesh =  smr.sharedMesh;
			}


			vertCount = mesh.triangles.Length;
			mesh.MarkDynamic();
			initData = new Vector4[vertCount]; 
			
			for (int ind = 0; ind < initData.Length; ind ++)
			{
				initData[ind] = new Vector4(mesh.vertices[ind % mesh.vertexCount].x, mesh.vertices[ind % mesh.vertexCount].y, mesh.vertices[ind % mesh.vertexCount].z, ind);
			}

			triVertexPositionBuffer[READ] = new ComputeBuffer(vertCount/3, sizeof(float) * 4*3, ComputeBufferType.Append);
			triVertexPositionBuffer[WRITE] = new ComputeBuffer(vertCount/3, sizeof(float) * 4*3, ComputeBufferType.Append);

			triVertexPositionBuffer[READ].SetData(initData);
			triVertexPositionBuffer[WRITE].SetData(initData);
			triVertexPositionBuffer[READ].SetCounterValue(0);
			triVertexPositionBuffer[WRITE].SetCounterValue(0);

			argBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);

			string initdatastr = "init vert dat == -lengt: " + initData.Length +" -count: " + mesh.vertexCount + " -buffercount: " + mesh.vertexBufferCount + " -thisMaxVertCount: " + vertCount +" -thisMaxTriCount: " + (mesh.triangles.Length/3);
			for (int i = 0; i < initData.Length; i++)
				initdatastr += "\n" + initData[i];
			Debug.Log(initdatastr);


			int smallest = 100000;
			int biggest = -1;
			string toprint = "\t tri DATA: ";
			for(int t = 0; t< mesh.triangles.Length;t++)
			{
				//if (t % 3 == 0)
				//	toprint += "\n";
				toprint += t + ", ";
				if (mesh.triangles[t] > biggest)
					biggest = t;
				if (mesh.triangles[t] < smallest)
					smallest = t;
			}

			Debug.Log("biggest tri vertexIndex: " + biggest + " smollset:" + smallest + toprint);

		

			vertexReciever.PassVertexPositions(triVertexPositionBuffer, vertCount);
		}

		private void Update()
		{
			BufferTools.Swap(triVertexPositionBuffer);

			if(debug)
			{
				int[] argdata = new int[] { 0, 1, 0, 0 };
				argBuffer.SetData(argdata);
				ComputeBuffer.CopyCount(triVertexPositionBuffer[READ], argBuffer, 0);
				argBuffer.GetData(argdata);
				DoDebug(argdata);
			}

			DoRender();
		}


		void DoRender()
		{
			Graphics.ClearRandomWriteTargets();
			triVertexPositionBuffer[WRITE].SetCounterValue(0);
			material.SetPass(0);
			material.hideFlags = HideFlags.HideAndDontSave;
			material.SetBuffer("WATriVertexPositionBuffer", triVertexPositionBuffer[WRITE]);
			Graphics.SetRandomWriteTarget(1, triVertexPositionBuffer[WRITE], false);
		}

		void DoDebug(int[] argdata)
		{
			string debg = "";

			debg += ("vertex count " + argdata[0]);
			debg += "\t" + ("instance count " + argdata[1]);
			debg += "\t" + ("start vertex " + argdata[2]);
			debg += "\t" + ("start instance " + argdata[3]);
			debg += "\n";


			if(debug)
			{
				Vector4[] data = new Vector4[vertCount];//argdata[0]];
				triVertexPositionBuffer[READ].GetData(data);
				debg += SortAndPrintData(data);
			}

			Debug.LogError(debg);
		}

		string SortAndPrintData(Vector4[] data)
		{
			string debg = "";
			List<int> sourceData = new List<int>(data.Length / 3);

			for (int i = 0; i < data.Length/3; i++)
				sourceData.Add(i);
			int finalCount = 0;


			for (int p1 =0; p1 < mesh.vertexCount; p1++)
			{
				for (int p2 = 0; p2 < mesh.vertexCount; p2++)
				{
					for (int p3 = 0; p3 < mesh.vertexCount; p3++)
					{
						for (int t= sourceData.Count-1; t>=0;t--)
						{
							int tindex = sourceData[t]*3;

							if ((int)data[tindex].w == p1 && (int)data[tindex + 1].w == p2 && (int)data[tindex + 2].w == p3)
							{
								sourceData.RemoveAt(t);
								debg = string.Format("{0} \n t:{1}{2}", debg, finalCount, string.Format("\n {0}, {1}, {2}", data[tindex], data[tindex + 1], data[tindex + 2]));
								finalCount++;
							}
						}
					}

				}

			}

			/*debg += "\n\n OLDE " + mesh.vertexCount + " \n";
			for(int t=0; t<data.Length/3; t++)
			{
				debg += "\n " + t;
				debg += string.Format("\n {0}, {1}, {2}", data[t*3], data[t*3 + 1], data[t*3 + 2]);
			}*/

			return debg + "\n\n\t finalTriCountPrinted: "+ finalCount +" / " + (data.Length/3);
		}

		void OnRenderObject()
		{
			//DisplayMAT.SetPass(0);
			//DisplayMAT.SetBuffer("_Data", triVertexPositionBuffer[READ]);
			//DisplayMAT.SetColor("col", Color.blue);
			//DisplayMAT.SetFloat("size", 0.1f);
			//
			//int[] argdata = new int[] { 0, 1, 0, 0 };
			//argBuffer.SetData(argdata);
			//ComputeBuffer.CopyCount(triVertexPositionBuffer[READ], argBuffer, 0);
			//argBuffer.GetData(argdata);
			//DoDebug(argdata);

			//Graphics.DrawProceduralIndirect(MeshTopology.Points, argBuffer, 0);
		}

		void OnDisable()
		{
			triVertexPositionBuffer[READ].Dispose();
			triVertexPositionBuffer[WRITE].Dispose();
			argBuffer.Dispose();
		}
	}
}