using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	private Transform myTransform;
	const float jumpSpeed = 10.0f;
	const float gravity = 3.0f;

	// Use this for initialization
	void Start()
	{
		myTransform = transform;
		myTransform.position = new Vector3 (-3, -3, 1);
	}
	
	// Update is called once per frame
	void Update()
	{
		Jump();

		if (myTransform.position.y > -3) 
		{
			myTransform.Translate(Vector3.down * gravity * Time.deltaTime);
		}
	}

	void Jump()
	{
		bool aHeld = Input.GetKeyDown("joystick 1 button 0");
		bool bHeld = Input.GetKeyDown("joystick 1 button 1");
		bool xHeld = Input.GetKeyDown("joystick 1 button 2");
		bool yHeld = Input.GetKeyDown("joystick 1 button 3");

		bool LBHeld = Input.GetKeyDown("joystick 1 button 4");
		bool RBHeld = Input.GetKeyDown("joystick 1 button 5");
		bool backHeld = Input.GetKeyDown("joystick 1 button 6");
		bool startHeld = Input.GetKeyDown("joystick 1 button 7");

		bool dpLHeld = Input.GetKeyDown("joystick 1 button 8"); // ?
		bool dpRHeld = Input.GetKeyDown("joystick 1 button 9"); // ?

		float dPadH = Input.GetAxis("RightStickHorizontal");
		//Debug.Log(Input.GetAxis("Left Trigger"));
		//Debug.Log(Input.GetAxis("Left Trigger"));
		//Debug.Log(Input.GetAxis("DPad Horizontal"));
		//Debug.Log(Input.GetAxis("DPad Vertical"));
		//Debug.Log(Input.GetAxis("Right Trigger")); // doesn't work
		//Debug.Log(Input.GetAxis("RightStickHorizontal"));
		//Debug.Log(Input.GetAxis("RightStickVertical"));

		float jumpDir = Input.GetAxis("Vertical");
		if (yHeld)
		{
			jumpDir = 1;
		}
		float walkDir = (dpLHeld ? -1 : 0) + (dpRHeld ? +1 : 0) + dPadH;
		myTransform.Translate(Vector3.up * jumpDir * jumpSpeed * 0.5f);
		myTransform.Translate(Vector3.right * walkDir * jumpSpeed * 0.5f);
	}
}
