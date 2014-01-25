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

	abstract class Brain
	{
		public abstract Thought Think();
	}

	class PlayerBrain : Brain
	{
		public GamePad.Index whichController = GamePad.Index.One;
		public override Thought Think()
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
	}

	Brain brain = new PlayerBrain();

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
		Thought thought = brain.Think();

		float duckAmount = Duck(thought);
		Run(thought);
		if (OnGround())
		{
			Jump(thought);
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

	float Duck(Thought thought)
	{
		myTransform.localScale = new Vector3(startScale.x * (1.0f + thought.duck), startScale.y * (1.0f - thought.duck/2), startScale.z);
		return thought.duck;
	}

	void Jump(Thought thought)
	{
		mVel = new Vector2(mVel.x, thought.jump ? jumpSpeed : mVel.y);
	}

	void Run(Thought thought)
	{
		float currVelX = thought.run;
		if (Math.Abs(currVelX) > 0.1f)
		{
			mVel = new Vector2(currVelX * maxRunSpeed, mVel.y);
		}
		else
		{
			mVel = new Vector2(mVel.x * 0.1f, mVel.y);
		}
		myTransform.eulerAngles = new Vector3(0, 0, -mVel.x/3);
	}
}
