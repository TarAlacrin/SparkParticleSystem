using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHideUI : MonoBehaviour 
{
	public List<GameObject> ToToggleOnOff;
	public Toggle tog;

	public GameObject refresher;

	bool refesh = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(refesh)
			this.gameObject.transform.position += Vector3.zero;
	}


	public void Toggle()
	{
		foreach(GameObject gam in ToToggleOnOff)
		{
			gam.SetActive(tog.isOn);
		}
		refesh = true;
	}
}
