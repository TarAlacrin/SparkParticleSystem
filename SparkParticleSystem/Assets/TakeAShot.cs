using UnityEngine;
using System.Collections;
using System.IO;

public class TakeAShot : MonoBehaviour 
{
	public int renderMultiplier = 3;
	public KeyCode key = KeyCode.Print;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(key))
		{
			bool exists = true;
			int i = 0;
			while(exists)
			{
				i++;
				exists = System.IO.File.Exists(Application.dataPath +"\\goodScreen" + i +".jpg");
			}
			ScreenCapture.CaptureScreenshot(Application.dataPath +"\\goodScreen" + i +".jpg", renderMultiplier);
			Debug.Log("TookSpecialScreenShot #" + i);
		}
	}

}
