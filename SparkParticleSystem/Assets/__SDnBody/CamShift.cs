using UnityEngine;
using System.Collections;

public class CamShift : MonoBehaviour {

	Camera thisCam;
	float defaov = 0f;
	float lastShiftUp = 0f;

	Transform orbitTrans;
	// Use this for initialization
	void Start () 
	{
		thisCam = gameObject.GetComponent<Camera>();
		defaov = thisCam.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(Input.GetKeyUp(KeyCode.LeftControl))
		{
			lastShiftUp = Time.time;
		}
		if(Input.GetKeyDown(KeyCode.LeftControl) && Time.time - lastShiftUp < 0.5f)
		{
			thisCam.fieldOfView = defaov;
		}


		if(Input.GetKey(KeyCode.LeftControl))
		{
			thisCam.fieldOfView =Mathf.Clamp(thisCam.fieldOfView + Input.mouseScrollDelta.y*3f, 1,179);
		}
		else if(!Input.GetKey(KeyCode.LeftShift))
		{
			//transform.position += transform.forward*Input.mouseScrollDelta.y;
		}


		if(Input.GetKeyDown(KeyCode.F))
		{
			if(this.orbitTrans == null)
			{
				this.orbitTrans = GameObject.Find("Capsule").transform;
			}

			if(this.orbitTrans != null)
			{
				Vector3 camFwd = thisCam.transform.forward; 


				float newDist = Mathf.Clamp(BufferHandlerPS1.inst.singularityStickDistance, 5f, 20f);
				Vector3 newPos = BufferHandlerPS1.inst.gravityObject.position - camFwd*newDist;
				float curDist = (Camera.main.transform.position - BufferHandlerPS1.inst.gravityObject.position).magnitude;

				Vector3 orbitPos = BufferHandlerPS1.inst.gravityObject.position;

                //THIS IS WHAT HANDLES THE SNAP BACK AND FORTH FROM EMITTER TO GRAVITY
				if(Camera.main.transform.position == newPos && BufferHandlerPS1.inst.singularityStickDistance == curDist && (curDist >= 4.9f && curDist <= 20.1f))
				{
					Vector3 emitPos = new Vector3(MiscPSystemControls.inst.emitPosBase.x,MiscPSystemControls.inst.emitPosBase.y,MiscPSystemControls.inst.emitPosBase.z);
					newPos = emitPos - camFwd*newDist;
					orbitPos= emitPos;
				}


				BufferHandlerPS1.inst.singularityStickDistance = newDist;

				thisCam.transform.position = newPos;

				this.orbitTrans.position = orbitPos;
			}
		}	
	}
}
