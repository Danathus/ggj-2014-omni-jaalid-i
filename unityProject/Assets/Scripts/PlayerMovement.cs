using UnityEngine;
using System.Collections;
using GamepadInput;
using System;

public class PlayerMovement : MonoBehaviour
{
	public GamePad.Index whichController = GamePad.Index.One;
	private Transform myTransform;

	Vector2 mPos;
	Vector2 mVel;
	const float jumpSpeed = 50.0f;
	const float gravity = 200.0f;
	const float maxRunSpeed = 30;
	const float groundLevel = -3;
	Vector3 startScale;

	class Thought
	{
		public bool jump;
		public float run;
		public float duck; // [0, 1] -- how much you are ducking (0 is not ducking at all)
	}

	Thought Think()
	{
		Thought thought = new Thought();
		GamepadState state = GamePad.GetState(whichController);
		thought.duck = state.LeftStickAxis.y < -0.1f
			? -state.LeftStickAxis.y
			: 0;
		thought.jump = state.A;
		thought.run = Math.Min(Math.Max(state.LeftStickAxis.x + state.dPadAxis.x, -1.0f), 1.0f);
		return thought;
	}

	// Use this for initialization
	void Start()
	{
		mVel = new Vector2(0, 0);
		myTransform = transform;
		mPos = new Vector2(-10, -3);
		startScale = new Vector3(myTransform.localScale.x, myTransform.localScale.y, myTransform.localScale.z);
	}

	float Height()
	{
		return myTransform.localScale.y;
	}
	bool OnGround()
	{
		return mPos.y - Height()/2 == groundLevel;
	}

	// Update is called once per frame
	void Update()
	{
		GamepadState state = GamePad.GetState(whichController);
		Thought thought = Think();

		float duckAmount = Duck(state);
		Run(state);
		if (OnGround())
		{
			Jump(state);
		}

		// apply gravity
		mVel = new Vector2(mVel.x, mVel.y - gravity*(1+duckAmount) * Time.deltaTime);
		mPos += mVel * Time.deltaTime;

		// clamp vertical position
		mPos = new Vector2(mPos.x, System.Math.Max(mPos.y - Height()/2, groundLevel) + Height()/2);
		myTransform.position = mPos;
		if (OnGround())
		{
			mVel = new Vector2(mVel.x, 0);
		}
	}

	float Duck(GamepadState state)
	{
		float duckAmount = 0;
		if (state.LeftStickAxis.y < -0.1f)
		{
			duckAmount = -state.LeftStickAxis.y;
		}
		myTransform.localScale = new Vector3(startScale.x * (1.0f + duckAmount), startScale.y * (1.0f - duckAmount/2), startScale.z);
		return duckAmount;
	}

	void Jump(GamepadState state)
	{
		mVel = new Vector2(mVel.x, state.A ? jumpSpeed : mVel.y);
	}

	void Run(GamepadState state)
	{
		float currVelX = Math.Min(Math.Max(state.LeftStickAxis.x + state.dPadAxis.x, -1.0f), 1.0f);
		if (Math.Abs(currVelX) > 0.1f)
		{
			mVel = new Vector2(currVelX * maxRunSpeed, mVel.y);
		}
		else
		{
			if (state.X)
			{
				mVel += new Vector2(Time.deltaTime * maxRunSpeed, 0);
			} 
			else
			{
				mVel = new Vector2(mVel.x * 0.1f, mVel.y);
			}
		}
		myTransform.eulerAngles = new Vector3(0, 0, -mVel.x/3);
	}
}
