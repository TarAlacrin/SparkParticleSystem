using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyMiror : MonoBehaviour
{

	public Transform tomirror;



	private void LateUpdate()
	{
		Vector3 newpos = tomirror.position;
		newpos.y *= -1f;
		this.transform.position = newpos;
	}
}
