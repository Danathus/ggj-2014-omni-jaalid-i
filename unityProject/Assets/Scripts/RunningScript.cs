using UnityEngine;
using System.Collections;

public class RunningScript : MonoBehaviour {
	private Transform myTransform;
	const float runSpeed = 5.0f;



	// Use this for initialization
	void Start () {
		myTransform = transform;
		myTransform.position = new Vector2 (-40, -3);
	}
	
	// Update is called once per frame
	void Update () {
		Running ();
	}

	void Running() {
		myTransform.Translate(Vector3.right * runSpeed * Time.deltaTime);
	}
}
