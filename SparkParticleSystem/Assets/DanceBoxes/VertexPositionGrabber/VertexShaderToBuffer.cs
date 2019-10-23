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

			initData = new Vector4[mesh.vertices.Length]; 
			
			for (int ind = 0; ind < initData.Length; ind ++)
			{
				initData[ind] = new Vector4(mesh.vertices[ind].x, mesh.vertices[ind].y, mesh.vertices[ind].z, ind);
			}

			vertCount = initData.Length;
			compute_buffer[READ] = new ComputeBuffer(initData.Length, sizeof(float) * 4, ComputeBufferType.Append);
			compute_buffer[WRITE] = new ComputeBuffer(initData.Length, sizeof(float) * 4, ComputeBufferType.Append);

			compute_buffer[READ].SetData(initData);
			compute_buffer[WRITE].SetData(initData);
			argBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);

			string initdatastr = "init vert dat";
			for (int i = 0; i < initData.Length; i++)
				initdatastr += "\n" + initData[i];
			Debug.Log(initdatastr);


			vertexReciever.PassVertexPositions(compute_buffer, mesh.triangles, mesh.vertices.Length);
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
			Vector4[] data = new Vector4[vertCount];
			compute_buffer[READ].GetData(data);
			string debg = "";


			debg += ("vertex count " + argdata[0]);
			debg += "\t" + ("instance count " + argdata[1]);
			debg += "\t" + ("start vertex " + argdata[2]);
			debg += "\t" + ("start instance " + argdata[3]);
			debg += "\n";
			for (int i = 0; i < data.Length; i++)
			{
				if(data[i].w == 0)
					debg += data[i];//"("+data[i].x + ","+ data[i].y+","+ data[i].z+") ";
			}	
			Debug.LogError(debg);
		}

		void OnRenderObject()
		{
			//DisplayMAT.SetPass(0);
			//DisplayMAT.SetBuffer("_Data", compute_buffer[READ]);
			//DisplayMAT.SetColor("col", Color.blue);
			//DisplayMAT.SetFloat("size", 0.1f);

			//int[] argdata = new int[] { 0, 1, 0, 0 };
			//argBuffer.SetData(argdata);
			//ComputeBuffer.CopyCount(compute_buffer[READ], argBuffer, 0);
			//argBuffer.GetData(argdata);
			//DoDebug(argdata);

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