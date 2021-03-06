﻿using UnityEngine;
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
	public bool thinkingEnabled = true;
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
		public Thought(bool jump, float run, float duck, int talk)
		{
			this.jump = jump;
			this.run = run;
			this.duck = duck;
			this.talk = talk;
		}
		public Thought() : this(false, 0, 0, 0) {}
		public bool thoughtless()
		{
			return !jump && (run < 0.2) && (duck < 0.1f);
		}
	}

	abstract class Brain
	{
		public int isTalking;
		public float timeToTalk;
		public float timeDontTalk = UnityEngine.Random.Range(5.0f, 15.0f);
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
			if(state.X && isTalking == 0)
			{
				thought.talk = UnityEngine.Random.Range(0, racerJokes.Length);
			}
			return thought;
		}
	}

	class AIBrain : Brain
	{
		public override Thought Think()
		{
			Thought thought = new Thought();
			thought.duck = 0;
			thought.jump = false;
			thought.run = 0.25f;
			if(isTalking == 0 && Time.realtimeSinceStartup - timeToTalk > timeDontTalk)
			{
				thought.talk = UnityEngine.Random.Range(1, 19);
				timeToTalk = Time.realtimeSinceStartup;
				timeDontTalk = UnityEngine.Random.Range(10.0f, 15.0f);
			}
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

	Animator anim;
	// Use this for initialization

	AudioClip racerHit;
	AudioClip racerJump1, racerJump2, racerJump3, racerJump4;
	
	void Start()
	{
		brain = CreateBrain();
		myTransform = transform;
		myTransform.position = myTransform.position + new Vector3(0, 100, 0);
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
			jumpSprite = Resources.Load<Sprite> (filepath + color + "_jump2");
			landSprite = Resources.Load<Sprite> (filepath + color + "_land1");
			slideSprite = Resources.Load<Sprite> (filepath + color + "_slide");
			SpriteRenderer sprRenderer = GetComponent<SpriteRenderer> ();
			sprRenderer.sprite = standSprite;
			sprRenderer.sortingOrder = 1;
		}
		anim = GetComponent<Animator> ();

		racerHit = Resources.Load<AudioClip>("Soundfx/RacerHit");
		racerJump1 = Resources.Load<AudioClip>("Soundfx/RacerJump");
		racerJump2 = Resources.Load<AudioClip>("Soundfx/RacerJump2");
		racerJump3 = Resources.Load<AudioClip>("Soundfx/RacerJump3");
		racerJump4 = Resources.Load<AudioClip>("Soundfx/RacerJump4");
	}

	float Height()
	{
		return myTransform.localScale.y;
	}
	private bool _onGround = false;

	public bool onGround
	{
		get
		{
			return _onGround;
		}

		set
		{
			if (value && !_onGround)
			{
				anim.SetTrigger ("HitGround");
				if(landSprite)
				{
					SpriteRenderer sprRenderer = GetComponent<SpriteRenderer> ();
					sprRenderer.sprite = landSprite;
				}
			}
			_onGround = value;
		}
	}

	bool OnGround()
	{
		return onGround;
	}

	// Update is called once per frame
	void Update()
	{
		Thought thought = IsStunned() || !thinkingEnabled ? new Thought(false, -1.0f, 0.0f, 0) : brain.Think();

		Duck(thought);
		Run(thought);
		if (OnGround())
		{
			Jump(thought);
		}
		Talk(thought);
		if (thought.thoughtless()) 
		{
			if (standSprite) 
			{
				SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();

				if (IsStunned())
				{
					sprRenderer.sprite = (int)(mStunCountdown * 10) % 2 == 0 ? standSprite : null;
				}
				else
				{
					sprRenderer.sprite = standSprite;
				}
			}
		}
		UpdateStunEffect();
#if STUN_TEST
		if (brainType == BrainType.Player1 && GamePad.GetState(GamePad.Index.One).Y)
		{
			Stun();
		}
#endif
	}

	float Duck(Thought thought)
	{
		if (slideSprite && thought.duck > 0.5f) 
		{
			SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();
			
			sprRenderer.sprite = slideSprite; 	
		}
		//myTransform.localScale = new Vector3(startScale.x * (1.0f + thought.duck), startScale.y * (1.0f - thought.duck/2), startScale.z);

		Rigidbody2D rbody = GetComponent<Rigidbody2D>();
		rbody.velocity = new Vector2(rbody.velocity.x, rbody.velocity.y + Physics2D.gravity.y*thought.duck);

		return thought.duck;
	}

	void Jump(Thought thought)
	{
		if (thought.jump)
		{
			if (jumpSprite)
			{
				SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();
				sprRenderer.sprite = jumpSprite;  
			}

			Rigidbody2D rbody = GetComponent<Rigidbody2D>();
			rbody.velocity = new Vector2(rbody.velocity.x, jumpSpeed);
			anim.SetTrigger ("Jump");

			// choose random jump sound
			//
			int jumpSound = (int)UnityEngine.Random.Range(0.0f, 3.9f);
			Vector3 camPos = GameObject.Find("Main Camera").transform.position;
			switch (jumpSound)
			{
			case 0: AudioSource.PlayClipAtPoint(racerJump1, camPos); break;
			case 1: AudioSource.PlayClipAtPoint(racerJump2, camPos); break;
			case 2: AudioSource.PlayClipAtPoint(racerJump3, camPos); break;
			case 3: AudioSource.PlayClipAtPoint(racerJump4, camPos); break;
			}
			//
			;
		}
		onGround = false;
	}

	void Run(Thought thought)
	{
		Rigidbody2D rbody = GetComponent<Rigidbody2D>();

		float currVelX = thought.run;
		if(runSprite && OnGround())
		{
			SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();
			sprRenderer.sprite = runSprite;  
		}
		rbody.velocity = new Vector2((currVelX + 1.0f) * maxRunSpeed, rbody.velocity.y);
		myTransform.eulerAngles = new Vector3(0, 0, -rbody.velocity.x/3);
	}	
	//int talking = 0;
	GameObject talkBubble;
	GameObject talkText;
	const float talkStayTime = 5.0f;
	float talkStartTime;
	void Talk(Thought thought)
	{
		if (thought.talk > 0 && thought.talk != brain.isTalking)
		{
			talkBubble = new GameObject ();
			Sprite talkSprite = Resources.Load<Sprite> ("Art/Effects/speechbubble");
			
			SpriteRenderer spriteRenderer = talkBubble.AddComponent<SpriteRenderer> ();
			spriteRenderer.sprite = talkSprite;
			brain.isTalking = thought.talk;
			talkBubble.name = "talkBubble";
			Destroy(talkBubble, talkStayTime);
			talkBubble.transform.position = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y + 20);
			talkBubble.transform.localScale = new Vector2(1.1f, 1.1f);
			
			talkText = new GameObject ();
			talkText.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y /*+ 20*/, -5.0f);
			Destroy(talkText, talkStayTime);
			//talkText.renderer.material.color = Color.black;
			//talkText.transform.localScale = new Vector2(10, 10);
			
			MeshRenderer meshRenderer = talkText.AddComponent<MeshRenderer>();
			TextMesh talkLayer = talkText.AddComponent<TextMesh> ();
			//meshRenderer.material = (Resources.Load("Livingst") as Material);
			
			talkText.renderer.material = Resources.Load("arialbd", typeof(Material)) as Material; 
			talkLayer.text = GenerateTalkText();
			talkLayer.anchor = TextAnchor.LowerCenter;
			talkLayer.fontSize = 16; //20;
			talkLayer.characterSize = 2.0f;
			talkLayer.renderer.material.color = Color.black;
			Font myFont = Resources.Load("arialbd", typeof(Font)) as Font;
			talkLayer.font = myFont;
		} 
		if (talkBubble) 
		{
			talkBubble.transform.position = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y + 20);
			talkText.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y +13/*+ 20*/, -5.0f);
		}
		else
		{
			brain.isTalking = 0;
		}
	}
	
	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.name == "tile")
		{
			onGround = true;
		}
		else if (collision.gameObject.name == "obstacle")
		{
			Stun();
			GameObject.Destroy(collision.gameObject);
		}
    }

	float mStunCountdown = 0;
	const float kStunDuation = 1.0f;
	void Stun()
	{
		if (!IsStunned())
		{
			mStunCountdown = kStunDuation;
			AudioSource.PlayClipAtPoint(racerHit, GameObject.Find("Main Camera").transform.position); //  new Vector3(0, 0, 0)
		}
	}
	bool IsStunned()
	{
		return mStunCountdown > 0;
	}
	void UpdateStunEffect()
	{
		mStunCountdown = Mathf.Max(mStunCountdown - Time.deltaTime, 0);
	}

	static string[] racerJokes = new string[]{
		"Am I racing\nwith myself?",
		"Who puts obstacles\nin the middle\nof a desert?",
		"Was it dash then\njump or jump\nthen dash...",
		"Did the race come\nbefore the obstacles,\nor the obstacles\nbefore the race?",
		"God I hope\nI don't die",
		"Please don't\ncatch up..\nPlease don't\ncatch up ...",
		"I'm tired",
		"Am I wearing\nshoes? I\ncan't tell",
		"This race is race-ist",
		"Was running\nalways\nthis hard?",
		"My cousin died\nonce when he\njumped instead\nof dashing",
		"Is that guy wearing\nroller skates?",
		"I think I'll jump\nthis time",
		"What if there\nwere no hypo-\nthetical questions?",
		"How can you\nguys talk\nso much?!",
		"They Don’t\nThink It Be\nLike It Is,\nBut It Do",
		"Now this is the\nstory all about, how\nmy life got flip-\nturned upside-down...",
		"We're no stran-\ngers to love. You\nknow the rules\nand so do I..."
	};

	string GenerateTalkText()
	{
		//return "line " + brain.isTalking;
		return racerJokes [brain.isTalking % racerJokes.Length];
	}
}
