using UnityEngine;
using System.Collections;
using GamepadInput;

public class PlayerMovement : MonoBehaviour
{
	private Transform myTransform;
	const float jumpSpeed = 10.0f;
	const float gravity = 3.0f;
	float runSpeed = 0.0f;

	// Use this for initialization
	void Start()
	{
		myTransform = transform;
		myTransform.position = new Vector2 (-10, -3);
	}
	
	// Update is called once per frame
	void Update()
	{
		Running ();

		Jump();

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

	void Jump()
	{
	#if IGNORE
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
	#else
		GamepadState state = GamePad.GetState(GamePad.Index.One);
		bool yHeld = state.Y;
		bool dpLHeld = state.dPadAxis.x < -0.1f;
		bool dpRHeld = state.dPadAxis.x >  0.1f;
		float dPadH = state.dPadAxis.x;
	#endif

		float jumpDir = 0; //Input.GetAxis("Vertical");
		if (yHeld)
		{
			jumpDir = 1;
		}
		float walkDir = dPadH;
		myTransform.Translate(Vector3.up * jumpDir * jumpSpeed * Time.deltaTime);
		myTransform.Translate(Vector3.right * walkDir * jumpSpeed * Time.deltaTime);
	}
	void Running() {
		myTransform.Translate(Vector3.right * runSpeed * Time.deltaTime);
	}

}
