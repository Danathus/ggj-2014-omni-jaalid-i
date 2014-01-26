using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	GameObject _targetA, _targetB, _targetC, _targetD;
	public GameObject targetA { set { _targetA = value; targets = null; } get { return _targetA; } }
	public GameObject targetB { set { _targetB = value; targets = null; } get { return _targetB; } }
	public GameObject targetC { set { _targetC = value; targets = null; } get { return _targetC; } }
	public GameObject targetD { set { _targetD = value; targets = null; } get { return _targetD; } }
	public Camera camera;
	float zPos;
	Transform cameraTransform;
	const float minFOV = 25.0f;

	GameObject[] targets = null;

	// Use this for initialization
	void Start () {
		zPos = -10.0f;
		//cameraTransform.position = new Vector3 (-17.8f, 1.1f, -10.0f);
		//target = GameObject.FindWithTag("Player");
	}

	float FindMaxTargetDistance()
	{
		float maxDist = 0;
		for (int targetIdx = 0; targetIdx < targets.Length-1; ++targetIdx)
		{
			GameObject targetA = targets[targetIdx];
			GameObject targetB = targets[targetIdx+1];
			maxDist = Mathf.Max(maxDist, (targetA.transform.position - targetB.transform.position).magnitude);
		}
		return maxDist;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (targets == null)
		{
			int numTargets = (targetA ? 1 : 0) + (targetB ? 1 : 0) + (targetC ? 1 : 0) + (targetD ? 1 : 0);
			targets = new GameObject[numTargets];
			int index = 0;
			if (targetA) { targets[index++] = targetA; }
			if (targetB) { targets[index++] = targetB; }
			if (targetC) { targets[index++] = targetC; }
			if (targetD) { targets[index++] = targetD; }
		}

		Vector2 cameraPos = new Vector2 ();
		for (int targetIdx = 0; targetIdx < targets.Length; ++targetIdx)
		{
			GameObject target = targets[targetIdx];
			cameraPos += new Vector2(target.transform.position.x, target.transform.position.y);
		}
		cameraPos /= targets.Length;
		Vector3 targetPos = new Vector3(cameraPos.x, cameraPos.y, zPos);
		float posLerpTime = Time.deltaTime * 10;
		//gameObject.transform.position = gameObject.transform.position * 0.9f + new Vector3(cameraPos.x, cameraPos.y, zPos) * 0.1f;
		gameObject.transform.position = new Vector3 (
			Mathf.Lerp (gameObject.transform.position.x, targetPos.x, posLerpTime),
			Mathf.Lerp (gameObject.transform.position.y, targetPos.y, posLerpTime),
			//Mathf.Lerp (gameObject.transform.position.z, targetPos.z, posLerpTime));
			zPos);
		/*if (Camera.main.fieldOfView < (maxXPos - minXPos)) 
		{
			Camera.main.fieldOfView += 50.0f; //Mathf.Lerp(camera.fieldOfView,60,Time.deltaTime*5);

		}*/
		float maxTargetDistance = FindMaxTargetDistance();
		float targetOrthographicSize = Mathf.Min(Mathf.Max(minFOV, (maxTargetDistance +10.0f)/2.2f), Mathf.Infinity);
		camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, targetOrthographicSize, Time.deltaTime * 10);
		//gameObject.camera.orthographicSize
	}
}
