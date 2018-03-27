using UnityEngine;
using System.Collections;

public class HoldEscapeToQuit : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	public bool isCounting{
		get{
			return countdown != 2f;
			}
	}
	public float countdown;
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape))
			countdown = 2f;
		else if(Input.GetKey(KeyCode.Escape))
			countdown -= Time.deltaTime;

		if(Input.GetKeyDown(KeyCode.Escape))
		{
				Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
				Cursor.visible = !(Cursor.lockState == CursorLockMode.Locked);
		}

		if(countdown < 0f)
			Application.Quit();
	}
}
