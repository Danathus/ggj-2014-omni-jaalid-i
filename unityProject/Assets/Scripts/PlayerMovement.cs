using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

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

	const float jumpSpeed = 100.0f; //50.0f;
	const float maxRunSpeed = 60;
	Vector3 startScale;

	class Thought
	{
		public bool jump;
		public float run;
		public float duck; // [0, 1] -- how much you are ducking (0 is not ducking at all)
		public int talk; // the index of the image want to show, 0 nothing to talk
		public bool thoughtless()
		{
			if (!jump && (run < 0.2) && (duck < 0.1f))
				return true;
			else
				return false;
		}
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

	Sprite standSprite;
	Sprite runSprite;
	Sprite jumpSprite;
	Sprite landSprite;
	Sprite slideSprite;

	// Use this for initialization
	void Start()
	{
		brain = CreateBrain();
		myTransform = transform;
		startScale = new Vector3(myTransform.localScale.x, myTransform.localScale.y, myTransform.localScale.z);
		//Resources.Load<Sprite> ("/Art/Basketballer/basketballer_blue_land1.png");

		string filepath = "Art/Basketballer/basketballer_";
		string color;
		switch (brainType) 
		{
		case BrainType.Player1: color = "blue"; break;
		case BrainType.Player2: color = "green"; break;
		case BrainType.Player3: color = "red"; break;
		case BrainType.Player4: color = "yellow"; break;
		default:                color = null; break;
		}
		if (color != null) 
		{
			standSprite = Resources.Load<Sprite> (filepath + color + "_stand1");
			runSprite = Resources.Load<Sprite> (filepath + color + "_run1");
			jumpSprite = Resources.Load<Sprite> (filepath + color + "_jump1");
			landSprite = Resources.Load<Sprite> (filepath + color + "_land1");
			slideSprite = Resources.Load<Sprite> (filepath + color + "_slide");
			SpriteRenderer sprRenderer = GetComponent<SpriteRenderer> ();
			sprRenderer.sprite = standSprite;
			sprRenderer.sortingOrder = 1;
		}
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
		Thought thought = brain.Think();

		Duck(thought);
		Run(thought);
		if (OnGround())
		{
			Jump(thought);
		}
		Talk (thought);
		if (thought.thoughtless ()) 
		{
			if (standSprite) 
			{
				SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();
				
				sprRenderer.sprite = standSprite; 	
			}
		}
	}

	float Duck(Thought thought)
	{
		if (slideSprite && thought.duck > 0.5f) 
		{
			SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();
			
			sprRenderer.sprite = slideSprite; 	
		}
		myTransform.localScale = new Vector3(startScale.x * (1.0f + thought.duck), startScale.y * (1.0f - thought.duck/2), startScale.z);

		Rigidbody2D rbody = GetComponent<Rigidbody2D>();
		rbody.velocity = new Vector2(rbody.velocity.x, rbody.velocity.y + Physics2D.gravity.y*thought.duck);

		return thought.duck;
	}

	void Jump(Thought thought)
	{
		if (thought.jump)
		{
			if(jumpSprite)
			{
				SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();
				sprRenderer.sprite = jumpSprite;  
			}

			Rigidbody2D rbody = GetComponent<Rigidbody2D>();
			rbody.velocity = new Vector2(rbody.velocity.x, jumpSpeed);
		}
		onGround = false;
	}

	void Run(Thought thought)
	{

		Rigidbody2D rbody = GetComponent<Rigidbody2D>();

		float currVelX = thought.run;
		if (Math.Abs(currVelX) > 0.1f)
		{
			if(runSprite && OnGround())
			{
				SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();
				sprRenderer.sprite = runSprite;  
			}
			rbody.velocity = new Vector2(currVelX * maxRunSpeed, rbody.velocity.y);
		}
		else
		{
			rbody.velocity = new Vector2(rbody.velocity.x * 0.1f, rbody.velocity.y);
		}
		myTransform.eulerAngles = new Vector3(0, 0, -rbody.velocity.x/3);
	}
	int talking = 0;
	GameObject talkBubble;
	void Talk(Thought thought)
	{
		
		if (thought.talk > 0 && thought.talk != talking) {
			talkBubble = new GameObject ();
			Sprite talkSprite = Resources.Load<Sprite> ("Art/Basketballer/basketballer_" + "blue" + "_stand1");
			
			SpriteRenderer spriteRenderer = talkBubble.AddComponent<SpriteRenderer> ();
			spriteRenderer.sprite = talkSprite;
			talking = thought.talk;
			talkBubble.name = "talkBubble";
			
		} 
		else if (talking > 0 && talkBubble) 
		{
			talkBubble.transform.position = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y + 20);
		}
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
		if (collision.gameObject.name == "tile")
		{
			onGround = true;
		}
    }
	//#endif

}
