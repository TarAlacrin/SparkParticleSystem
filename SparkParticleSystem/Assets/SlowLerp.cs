using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowLerp : MonoBehaviour {

	public Transform targetTrans;
	public float lerpsped = 1f;
	public float maxDistance = 1f;

	// Use this for initialization
	void Start () {
		 
	}
	
	// Update is called once per frame
	void LateUpdate () {

		
		float distance = (this.transform.position - targetTrans.position).magnitude;

		float numFramesEstimated = Mathf.Ceil(distance / (maxDistance * Time.deltaTime));

		Vector3 targetPos = Vector3.Slerp(this.transform.position, targetTrans.position, lerpsped * Time.deltaTime);

		//this.transform.position = Vector3.MoveTowards(this.transform.position, targetTrans.position, maxDistance * Time.deltaTime);


		//this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetTrans.rotation, 1f/numFramesEstimated);//Quaternion.RotateTowards(this.transform.rotation, targetTrans.rotation, speed * Time.deltaTime);


		this.transform.position = targetTrans.position;

		this.transform.rotation = targetTrans.rotation;

	}
}
