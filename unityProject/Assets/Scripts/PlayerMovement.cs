using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	private Transform myTransform;
	const float jumpSpeed = 10.0f;
	const float gravity = 3.0f;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		myTransform.position = new Vector3 (-3, -3, 1);
	}
	
	// Update is called once per frame
	void Update () {

		Jump ();

		if (myTransform.position.y > -3) 
		{
			myTransform.Translate(Vector3.down * gravity * Time.deltaTime);
		}
	}

	void Jump() {
		myTransform.Translate(Vector3.up * Input.GetAxis ("Vertical") * jumpSpeed * 0.5f);

	}
}
