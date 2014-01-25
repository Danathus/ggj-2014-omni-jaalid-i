using UnityEngine;
using System.Collections;
using GamepadInput;
using System;

public class PlayerMovement : MonoBehaviour
{
	public enum BrainType
	{
		Player1,
		Player2,
		Player3,
		Player4,
		AI
	}
	public BrainType brainType = BrainType.Player1;

	private Transform myTransform;

	Vector2 mPos;
	Vector2 mVel;
	const float jumpSpeed = 50.0f;
	const float gravity = 200.0f;
	const float maxRunSpeed = 30;
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
		public GamePad.Index whichController; // = GamePad.Index.One;
		public PlayerBrain(GamePad.Index controllerIdx)
		{
			whichController = controllerIdx;
		}
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

	class AIBrain : Brain
	{
		public override Thought Think()
		{
			Thought thought = new Thought();
			thought.duck = 0;
			thought.jump = true;
			thought.run = 0.25f;
			return thought;
		}
	}

	Brain brain;

	Brain CreateBrain()
	{
		switch (brainType)
		{
		case BrainType.Player1: return new PlayerBrain(GamePad.Index.One);
		case BrainType.Player2: return new PlayerBrain(GamePad.Index.Two);
		case BrainType.Player3: return new PlayerBrain(GamePad.Index.Three);
		case BrainType.Player4: return new PlayerBrain(GamePad.Index.Four);
		case BrainType.AI:      return new AIBrain();
		default:                return null;
		}
	}

	// Use this for initialization
	void Start()
	{
		brain = CreateBrain();
		mVel = new Vector2(0, 0);
		myTransform = transform;
		mPos = new Vector2(myTransform.position.x, myTransform.position.y);
		startScale = new Vector3(myTransform.localScale.x, myTransform.localScale.y, myTransform.localScale.z);
	}

	float Height()
	{
		return myTransform.localScale.y;
	}
	private bool onGround = false;
	bool OnGround()
	{
		return onGround;
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.Log(rigidbody2d
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

		myTransform.position = new Vector2(mPos.x, myTransform.position.y);
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
		if (thought.jump)
		{
			Rigidbody2D rbody = GetComponent<Rigidbody2D>();
			rbody.velocity = new Vector2(rbody.velocity.x, jumpSpeed);
		}
		onGround = false;
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

	//#if IGNORE
	void OnCollisionEnter2D(Collision2D collision)
	{
		/*
		float minDist = 2;
		mPos = (mPos - new Vector2(collision.transform.position.x, collision.transform.position.y)).normalized * minDist;
        foreach (ContactPoint2D contact in collision.contacts)
		{
            Debug.DrawRay(contact.point, contact.normal, Color.green);
        }
		//*/
		if (collision.gameObject.name == "Big Flat Ground")
		{
			onGround = true;
		}
    }
	//#endif
}
