using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gravObjectController : MonoBehaviour 
{
	BufferHandlerPS1 buffHandler;
	Transform gravTran;
	MeshRenderer gravRend;
	public Camera camToControl;
	public Text frameRate;
	// Use this for initialization
	void Start () 
	{
		if (camToControl == null)
			camToControl = this.gameObject.GetComponent<Camera>();

		buffHandler = Object.FindObjectOfType<BufferHandlerPS1>();
		if(buffHandler.gravityObject == null)
			Destroy(this);
		else
		{
			gravTran = buffHandler.gravityObject;
			gravRend = gravTran.gameObject.GetComponent<MeshRenderer>();
			gravRend.enabled = false;
		}
	}

	bool wasVisable = false;
	float distFromCam;

	[HideInInspector]
	Vector3 oldEmitPos;

	bool isMatched = false;

	List<int> lastFewFrames = new List<int>() {60,60,60 ,60,60,60 ,60,60,60 ,60,60,60 ,60,60,60 ,60,60,60 ,60,60,60};
	// Update is called once per frame
	void Update() 
	{
		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			//gravRend.enabled = wasVisable;
			distFromCam = camToControl.WorldToScreenPoint(gravTran.position).z;
		}

		if(Input.GetKeyUp(KeyCode.LeftShift))
		{
			//gravRend.enabled = false;
		}

		if(Input.GetKeyDown(KeyCode.RightShift))
		{
			gravRend.enabled = !gravRend.enabled;
		}

		if(Input.GetKey(KeyCode.LeftShift))
		{
			distFromCam += Input.mouseScrollDelta.y*2.5f;
			Vector3 mousePos = Input.mousePosition;
			gravTran.position = camToControl.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distFromCam));
		}

		if(Input.GetKeyDown(KeyCode.Tab))
		{

			if(isMatched)
			{
				isMatched = false;
				oldEmitPos = MiscPSystemControls.inst.EmitPositionBase;
			} else
			{
				isMatched = true;
				MiscPSystemControls.inst.EmitPositionBase = oldEmitPos;
			}
			//gravRend.enabled = !gravRend.enabled;
		}

		if(isMatched)
			MiscPSystemControls.inst.EmitPositionBase = gravTran.position;
		/*if(Input.GetKeyDown(KeyCode.Return))
		{
			SceneManager.LoadScene(0);
		}*/

		if (Input.GetKeyDown(KeyCode.PageUp))
		{
			Debug.Log("U");
			ParticleCountItterator.multiplier ++;
			SceneManager.LoadScene(0);
		}
		else
		if(Input.GetKeyDown(KeyCode.PageDown))
		{
			Debug.Log("D");
			ParticleCountItterator.multiplier --;
			if(ParticleCountItterator.multiplier < -16)
			{
				ParticleCountItterator.multiplier = -16;
			}
			SceneManager.LoadScene(0);
		}
		
		lastFewFrames.Add(Mathf.CeilToInt(1.0f/Time.deltaTime) );
		lastFewFrames.RemoveAt(0);
		int average = lastFewFrames[lastFewFrames.Count-1];
		foreach(int i in lastFewFrames)
			average += i;

		average /= lastFewFrames.Count+1;


		frameRate.text = Mathf.CeilToInt(average) + "fps";
	}
}
