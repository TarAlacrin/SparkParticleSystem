// https://github.com/przemyslawzaworski
// Set plane position (Y=0.0)
// Collider is generated from vertex shader, using RWStructuredBuffer to transfer data.

using UnityEngine;

namespace DanceBoxes
{
	public class VertexShaderToBuffer : MonoBehaviour
	{
		public const int READ = 1;
		public const int WRITE = 0;

		public Material DisplayMAT;
		public Material material;
		ComputeBuffer[] compute_buffer = new ComputeBuffer[2];
		ComputeBuffer argBuffer;
		Mesh mesh;
		Vector4[] initData;

		public GameObject vertexPositionRecieverObject;
		IWantVertexPositions vertexReciever;


		int vertCount;

		void Start()
		{
			vertexReciever = vertexPositionRecieverObject.GetComponent<IWantVertexPositions>();
			mesh = this.gameObject.GetComponent<MeshFilter>().sharedMesh;
			vertCount = mesh.triangles.Length * 3;

			initData = new Vector4[vertCount]; 
			
			for (int ind = 0; ind < initData.Length; ind ++)
			{
				initData[ind] = new Vector4(mesh.vertices[ind % mesh.vertexCount].x, mesh.vertices[ind % mesh.vertexCount].y, mesh.vertices[ind % mesh.vertexCount].z, ind);
			}

			compute_buffer[READ] = new ComputeBuffer(mesh.triangles.Length*3, sizeof(float) * 4, ComputeBufferType.Append);
			compute_buffer[WRITE] = new ComputeBuffer(mesh.triangles.Length*3, sizeof(float) * 4, ComputeBufferType.Append);

			compute_buffer[READ].SetData(initData);
			compute_buffer[WRITE].SetData(initData);
			compute_buffer[READ].SetCounterValue(0);
			compute_buffer[WRITE].SetCounterValue(0);

			argBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);

			string initdatastr = "init vert dat : -lengt: " + initData.Length +" -count: " + mesh.vertexCount + " -buffercount: " + mesh.vertexBufferCount;
			for (int i = 0; i < initData.Length; i++)
				initdatastr += "\n" + initData[i];
			Debug.Log(initdatastr);


			int smallest = 100000;
			int biggest = -1;
			string toprint = "\t tri DATA: ";
			for(int t = 0; t< mesh.triangles.Length;t++)
			{
				if (t % 3 == 0)
					toprint += "\n";
				toprint += t + ", ";
				if (mesh.triangles[t] > biggest)
					biggest = t;
				if (mesh.triangles[t] < smallest)
					smallest = t;
			}

			Debug.Log("biggest tri vertexIndex: " + biggest + " smollset:" + smallest + toprint);

		

			vertexReciever.PassVertexPositions(compute_buffer, mesh.triangles, vertCount);
		}


		void Update()
		{
			BufferTools.Swap(compute_buffer);

			DoRender();
		}

		void DoRender()
		{
			Graphics.ClearRandomWriteTargets();
			compute_buffer[WRITE].SetCounterValue(0);
			material.SetPass(0);
			material.hideFlags = HideFlags.HideAndDontSave;
			material.SetBuffer("appendBuffer", compute_buffer[WRITE]);
			Graphics.SetRandomWriteTarget(1, compute_buffer[WRITE], true);

		}

		void DoDebug(int[] argdata)
		{
			string debg = "";


			debg += ("vertex count " + argdata[0]);
			debg += "\t" + ("instance count " + argdata[1]);
			debg += "\t" + ("start vertex " + argdata[2]);
			debg += "\t" + ("start instance " + argdata[3]);
			debg += "\n";

			Vector4[] data = new Vector4[vertCount];//argdata[0]];
			compute_buffer[READ].GetData(data);

			for (int i = 0; i < data.Length/3; i++)
			{
				debg += "\n " + i;
				debg += "\n" + data[i] +", " + data[i+1] + ", " + data[i+2];

				/*foreach(Vector4 dat in data)
				{
					if (i == dat.w)
						debg += "\n" + dat;
				}*/


				//if(data[i].w == 0)
			}
			Debug.LogError(debg);
		}

		void OnRenderObject()
		{
			DisplayMAT.SetPass(0);
			DisplayMAT.SetBuffer("_Data", compute_buffer[READ]);
			DisplayMAT.SetColor("col", Color.blue);
			DisplayMAT.SetFloat("size", 0.1f);

			int[] argdata = new int[] { 0, 1, 0, 0 };
			argBuffer.SetData(argdata);
			ComputeBuffer.CopyCount(compute_buffer[READ], argBuffer, 0);
			argBuffer.GetData(argdata);
			DoDebug(argdata);

			//Graphics.DrawProceduralIndirect(MeshTopology.Points, argBuffer, 0);
		}

		void OnDisable()
		{
			compute_buffer[READ].Dispose();
			compute_buffer[WRITE].Dispose();
			argBuffer.Dispose();
		}
	}
}