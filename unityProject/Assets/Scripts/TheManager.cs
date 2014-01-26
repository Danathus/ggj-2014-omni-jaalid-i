using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class TheManager : MonoBehaviour
{
	public Camera camera;
	public GameObject playerA;
	public GameObject playerB;
	public GameObject playerC;
	public GameObject playerD;
	public GameObject jeep;

	float timePassed;
	void Start()
	{
		timePassed = 0;

		CameraMovement camMovement = camera.GetComponent<CameraMovement>();
		camMovement.targetA = playerA;
		camMovement.targetB = playerB;
		camMovement.targetC = playerC;
		camMovement.targetD = playerD;
	}

	void DisablePlayer(PlayerMovement player)
	{
		player.thinkingEnabled = false;
		//player.enabled = false;
		//player.GetComponent<Rigidbody2D>().isKinematic = true;
	}

	public static GameObject Say(GameObject gObj, string text, float talkStayTime)
	{
		// give a word bubble for the jeep
		var talkBubble = new GameObject();
		Sprite talkSprite = Resources.Load<Sprite>("Art/Effects/speechbubble");
		
		SpriteRenderer spriteRenderer = talkBubble.AddComponent<SpriteRenderer> ();
		spriteRenderer.sprite = talkSprite;
		talkBubble.name = "talkBubble";
		Destroy(talkBubble, talkStayTime);
		talkBubble.transform.parent = gObj.transform;
		talkBubble.transform.position = new Vector2 (gObj.transform.position.x, gObj.transform.position.y + 20);
		talkBubble.transform.localScale = new Vector2(1.1f, 1.1f);
		
		var talkText = new GameObject ();
		talkText.transform.parent = gObj.transform;
		talkText.transform.position = new Vector3 (gObj.transform.position.x, gObj.transform.position.y +13, -5.0f);
		Destroy(talkText, talkStayTime);
		//talkText.renderer.material.color = Color.black;
		//talkText.transform.localScale = new Vector2(10, 10);
		
		MeshRenderer meshRenderer = talkText.AddComponent<MeshRenderer>();
		TextMesh talkLayer = talkText.AddComponent<TextMesh> ();
		//meshRenderer.material = (Resources.Load("Livingst") as Material);
		
		talkText.renderer.material = Resources.Load("arialbd", typeof(Material)) as Material; 
		talkLayer.text = text;
		talkLayer.anchor = TextAnchor.LowerCenter;
		talkLayer.fontSize = 16; //20;
		talkLayer.characterSize = 2.0f;
		talkLayer.renderer.material.color = Color.black;
		Font myFont = Resources.Load("arialbd", typeof(Font)) as Font;
		talkLayer.font = myFont;

		return talkBubble;
	}

	GameObject talkBubble;
	GameObject talkText;
	void Update()
	{
		timePassed += Time.deltaTime;

		if (timePassed > 70)
		{
		}
		else if (timePassed > 68)
		{
			CameraMovement camMovement = camera.GetComponent<CameraMovement>();
			if (camMovement.targetB == null)
			{
				var godzilla = GameObject.Find("Godzilla");
				camMovement.targetB = godzilla;
				var clip = Resources.Load<AudioClip>("Soundfx/GodzillaRoar");
				AudioSource.PlayClipAtPoint(clip, GameObject.Find("Main Camera").transform.position); //  new Vector3(0, 0, 0)

				godzilla.GetComponent<Godzilla>().shouldGetClose = true;
			}

			// tell Jeep to start moving
			jeep.GetComponent<Jeep>().movementMode = Jeep.MovementMode.PlayerControl;
		}
		else if (timePassed > 67)
		{
			// have the players fade out and disappear
			Destroy(playerA);
			Destroy(playerB);
			Destroy(playerC);
			Destroy(playerD);
		}
		else if (timePassed > 64.5f && !talkBubble) // 65
		{
			//
			talkBubble = Say(jeep, "Come with me\nIf you want\nto live!", 3.0f);
		}
		else if (timePassed > 62 && !talkBubble) // 65
		{
			//
			talkBubble = Say(jeep, "Don't look now,\nwe're being chased\nby Godzilla!", 3.0f);
		}
		else if (timePassed > 61)
		{
			CameraMovement camMovement = camera.GetComponent<CameraMovement>();
			camMovement.targetA = jeep;
			camMovement.targetB = null;
			camMovement.targetC = null;
			camMovement.targetD = null;
		}
		else if (timePassed > 60) //60)
		{
			// turn all the players and the jeep off
			if (playerA) { DisablePlayer(playerA.GetComponent<PlayerMovement>()); }
			if (playerB) { DisablePlayer(playerB.GetComponent<PlayerMovement>()); }
			if (playerC) { DisablePlayer(playerC.GetComponent<PlayerMovement>()); }
			if (playerD) { DisablePlayer(playerD.GetComponent<PlayerMovement>()); }
		}
		else if (timePassed > 40)
		{
			GameObject.Find("Jeep").GetComponent<Jeep>().shouldGetClose = true;
		}
		else if (timePassed >= 0 && !racerMusic)
		{
			racerMusic = Resources.Load<AudioClip>("Music/Racer/Decktonic_-_08_-_Fair_Game");
			AudioSource.PlayClipAtPoint(racerMusic, camera.transform.position);
		}
	}

	public static GameObject PlayClipAt(AudioClip clip, Vector3 pos)
	{
		var tempGO = new GameObject("TempAudio"); // create the temp object
		tempGO.transform.position = pos; // set its position
		tempGO.AddComponent<AudioSource>(); // add an audio source
		tempGO.audio.clip = clip; // define the clip
		tempGO.audio.Play(); // start the sound
		Destroy(tempGO, clip.length); // destroy object after clip duration
		return tempGO; // return reference to the temporary GameObject
	}

	AudioClip racerMusic;
}
