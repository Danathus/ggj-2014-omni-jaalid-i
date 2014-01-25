using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	private Transform myTransform;
	const float jumpSpeed = 10.0f;
	const float gravity = 3.0f;
	float runSpeed = 0.0f;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		myTransform.position = new Vector2 (-10, -3);
	}
	
	// Update is called once per frame
	void Update () {
		Running ();


		Jump ();

		if (myTransform.position.y > -3) 
		{
			myTransform.Translate(Vector3.down * gravity * Time.deltaTime);
		}
		if (Input.GetKey ("space")) {
			runSpeed += 0.5f;
		} 
		else if(runSpeed > 0.0f){
			runSpeed -= 0.5f;
		}
	}

	void Jump() {
		myTransform.Translate(Vector3.up * Input.GetAxis ("Vertical") * jumpSpeed * 0.5f);
	}

	void Running() {
		myTransform.Translate(Vector3.right * runSpeed * Time.deltaTime);
	}
}
