using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
	public float scrollspeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

	float lastframescrolldelta = 0f;

	Vector3 lastFrameMousePos = Vector3.zero;
	Vector3 lastFrameMouseDelta = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
		Pan();
		Zoom();
		Crab();
		ResetPos();
		//this.transform.RotateAround(Vector3.up, scrollToGo * Time.deltaTime*2f);

	}
	byte lastDeltaType = 0;


	


	void ZoomWheel()
	{
		float scrollToGo = Mathf.Lerp(lastframescrolldelta, Input.mouseScrollDelta.y, 0.07f);
		lastframescrolldelta = scrollToGo;

		this.transform.localScale = Vector3.one * Mathf.Abs(this.transform.localScale.x - scrollToGo * scrollspeed * Time.deltaTime);

	}

	float PanSpeed = 7f;

	void Pan()
	{
		if (Input.GetMouseButtonDown(0))
		{
			lastFrameMouseDelta = Vector3.zero;
			lastFrameMousePos = Input.mousePosition;
			lastDeltaType = 0;
		}

		Vector3 mouseDelta = Vector3.zero;
		if (Input.GetMouseButton(0))
		{
			mouseDelta = (Input.mousePosition - lastFrameMousePos);
		}
		if (lastDeltaType == 0)
		{
			mouseDelta = Vector3.Lerp(lastFrameMouseDelta, mouseDelta, 0.07f);
			lastFrameMouseDelta = mouseDelta;


			lastFrameMousePos = Input.mousePosition;
			this.transform.Rotate(Vector3.up, mouseDelta.x* Time.deltaTime * PanSpeed);
			this.transform.Rotate(-mouseDelta.y * Time.deltaTime * PanSpeed,0,0,Space.Self);
			this.transform.localEulerAngles = Vector3.Scale(this.transform.localEulerAngles, Vector3.up + Vector3.right);
		}


	}


	float ZoomSpeed = 0.1f;

	void Zoom()
	{
		if (Input.GetMouseButtonDown(1))
		{
			lastFrameMouseDelta = Vector3.zero;
			lastFrameMousePos = Input.mousePosition;
			lastDeltaType = 1;
		}

		Vector3 mouseDelta = Vector3.zero;
		if (Input.GetMouseButton(1))
		{
			mouseDelta = (Input.mousePosition - lastFrameMousePos);
		}
		if (lastDeltaType == 1)
		{
			mouseDelta = Vector3.Lerp(lastFrameMouseDelta, mouseDelta, 0.1f);
			lastFrameMouseDelta = mouseDelta;

			lastFrameMousePos = Input.mousePosition;
			this.transform.localScale = Vector3.one * Mathf.Abs(this.transform.localScale.x - mouseDelta.x * ZoomSpeed * Time.deltaTime);

		}
	}


	float crabSpeed = 0.2f;

	void Crab()
	{
		if(Input.GetMouseButtonDown(2))
		{
			lastFrameMouseDelta = Vector3.zero;
			lastFrameMousePos = Input.mousePosition;
			lastDeltaType = 2;
		}

		Vector3 mouseDelta = Vector3.zero;
		if (Input.GetMouseButton(2))
		{
			mouseDelta = (Input.mousePosition - lastFrameMousePos) ;
		}
		if(lastDeltaType ==2)
		{
			mouseDelta = Vector3.Lerp(lastFrameMouseDelta, mouseDelta, 0.07f);
			lastFrameMouseDelta = mouseDelta;

			
			lastFrameMousePos = Input.mousePosition;
			this.transform.localPosition -= this.transform.up * mouseDelta.y * Time.deltaTime * crabSpeed;
			this.transform.localPosition -= this.transform.right * mouseDelta.x * Time.deltaTime * crabSpeed;

		}

	}

	bool isResetting = false;
	float FlerpSpeed = 0f;
	float maxFlerpSpeed = 2.3f;
	float minFlerpSpeedPercentage = 0.07f;
	float slowDownDistance = .2f;
	float FlerpSpeedAccel = 5f;

	private void ResetPos()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			lastFrameMouseDelta = Vector3.zero;
			isResetting = true;
			FlerpSpeed = maxFlerpSpeed;
		}

		if (isResetting)
		{
			Vector3 goalPos = Vector3.one * 0.5f;
			float goalDist = (this.transform.localPosition - goalPos).magnitude;
			if (this.transform.localPosition == goalPos)
			{
				isResetting = false;
				FlerpSpeed = 0;
				return;
			}
			float tempMaxFlerpSpeed = maxFlerpSpeed;
			if (goalDist < slowDownDistance)
				tempMaxFlerpSpeed = maxFlerpSpeed * (minFlerpSpeedPercentage + Mathf.SmoothStep(0,1,goalDist / slowDownDistance) *(1f-minFlerpSpeedPercentage));

			FlerpSpeed = Mathf.Clamp(FlerpSpeed+Time.deltaTime, 0, tempMaxFlerpSpeed);


			this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, goalPos, FlerpSpeed * Time.deltaTime);
		}

	}
}
