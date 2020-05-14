using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pauser : MonoBehaviour
{
	Animator anim;
	bool oneframe = false;
    void Start()
    {
		anim = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
			anim.enabled = !anim.enabled;

		if (oneframe)
		{
			anim.enabled = false;
			oneframe = false;
		}

		if (Input.GetKeyDown(KeyCode.RightArrow) && !anim.enabled && !oneframe)
		{
			anim.enabled = true;
			oneframe = true;
		}


    }
}
