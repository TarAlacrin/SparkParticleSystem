using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

	float lastframescrolldelta = 0f;

    // Update is called once per frame
    void Update()
    {
		float scrollToGo = Mathf.Lerp(lastframescrolldelta, Input.mouseScrollDelta.y, 0.15f);
		lastframescrolldelta = scrollToGo;
		this.transform.RotateAround(Vector3.up, scrollToGo * Time.deltaTime*2f);
		
    }
}
