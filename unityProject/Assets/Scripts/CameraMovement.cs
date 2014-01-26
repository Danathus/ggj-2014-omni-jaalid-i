using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	public GameObject targetA;
	public GameObject targetB;
	public GameObject targetC;
	public GameObject targetD;
	public Camera camera;
	float zPos;
	Transform cameraTransform;
	const float minFOV = 20.0f;

	// Use this for initialization
	void Start () {
		zPos = -10.0f;
		//cameraTransform.position = new Vector3 (-17.8f, 1.1f, -10.0f);
		//target = GameObject.FindWithTag("Player");

	}
	
	// Update is called once per frame
	void Update () {
		Vector2 cameraPos = new Vector2 ();
		int count = 0;
		float minXPos = float.MaxValue, maxXPos = -float.MaxValue;
		if (targetA) 
		{
			count ++;
			cameraPos += new Vector2(targetA.transform.position.x, 0.0f);
			if(targetA.transform.position.x < minXPos)
				minXPos = targetA.transform.position.x;
			if(targetA.transform.position.x > maxXPos)
				maxXPos = targetA.transform.position.x;
		}
		if(targetB)
		{
			count ++;
			cameraPos += new Vector2(targetB.transform.position.x, 0.0f);
			if(targetB.transform.position.x < minXPos)
				minXPos = targetB.transform.position.x;
			if(targetB.transform.position.x > maxXPos)
				maxXPos = targetB.transform.position.x;
		}
		if(targetC)
		{
			count ++;
			cameraPos += new Vector2(targetC.transform.position.x, 0.0f);
			if(targetC.transform.position.x < minXPos)
				minXPos = targetC.transform.position.x;
			if(targetC.transform.position.x > maxXPos)
				maxXPos = targetC.transform.position.x;
		}
		if(targetD)
		{
			count ++;
			cameraPos += new Vector2(targetD.transform.position.x, 0.0f);
			if(targetD.transform.position.x < minXPos)
				minXPos = targetD.transform.position.x;
			if(targetD.transform.position.x > maxXPos)
				maxXPos = targetD.transform.position.x;
		}
		cameraPos /= count;
		gameObject.transform.position = new Vector3(cameraPos.x, cameraPos.y, zPos);
		/*if (Camera.main.fieldOfView < (maxXPos - minXPos)) 
		{
			Camera.main.fieldOfView += 50.0f; //Mathf.Lerp(camera.fieldOfView,60,Time.deltaTime*5);

		}*/
		if (camera.orthographicSize < (maxXPos - minXPos)/4) {
			camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, (maxXPos - minXPos)/4, Time.deltaTime * 5);
			
				} else {
			camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, Mathf.Max((maxXPos - minXPos +10.0f)/4, minFOV), Time.deltaTime * 5);
				}
		//gameObject.camera.orthographicSize
	}
}
