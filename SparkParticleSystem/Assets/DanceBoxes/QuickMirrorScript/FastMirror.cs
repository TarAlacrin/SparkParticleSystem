using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class NewBehaviourScript : MonoBehaviour
{
	public float YHeightForMirror = 0f;
	Camera originalCamera;
	Camera newCamera;
	GameObject newObject;
	Transform newTrans;

	private void Awake()
	{
		originalCamera = this.gameObject.GetComponent<Camera>();
		newObject = new GameObject();
		newTrans = newObject.transform;
		newCamera = newObject.AddComponent<Camera>();
		newCamera.CopyFrom(originalCamera);
	}


	private void LateUpdate()
	{
		Vector3 newPos = this.transform.position;
		newPos.y = YHeightForMirror -( this.transform.position.y - YHeightForMirror);
		newTrans.position = newPos;
	}
}
