using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

	public bool destroyDupe = true;
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		if(Time.frameCount > 1)
		{
			Destroy(this.gameObject);
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
