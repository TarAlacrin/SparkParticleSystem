using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSAverager : MonoBehaviour
{
	List<float> framerates = new List<float>();
	List<float> smoothframes = new List<float>();
	public bool record = false;

	float timeStartRec;

	bool lastFrameRecord = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(record && !lastFrameRecord)
		{
			List<float> framerates = new List<float>();
			List<float> smoothframes = new List<float>();

			timeStartRec = Time.time;
		}

		if(record)
		{
			framerates.Add(1f / Time.deltaTime);
			smoothframes.Add(1f/Time.smoothDeltaTime);
		}


		if(lastFrameRecord && !record)
		{

			float average = 0f;
			foreach (float f in framerates)
				average += f;

			average /= (float)framerates.Count;

			string todebug = "FRAME RATE OVER TIME : " + (Time.time - timeStartRec) + " starting at: " + timeStartRec + " EQUALS!== " + average;
			Debug.Log(todebug);
		}


		lastFrameRecord = record;
    }
}
