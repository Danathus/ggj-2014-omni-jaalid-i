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

	GameObject talkBubble;
	GameObject talkText;
	void Update()
	{
		timePassed += Time.deltaTime;

		if (timePassed > 69)
		{
		}
		else if (timePassed > 63 && !talkBubble) // 65
		{
			float talkStayTime = 5.0f;

			// give a word bubble for the jeep
			talkBubble = new GameObject();
			Sprite talkSprite = Resources.Load<Sprite>("Art/Effects/speechbubble");
			
			SpriteRenderer spriteRenderer = talkBubble.AddComponent<SpriteRenderer> ();
			spriteRenderer.sprite = talkSprite;
			talkBubble.name = "talkBubble";
			Destroy(talkBubble, talkStayTime);
			talkBubble.transform.parent = jeep.transform;
			talkBubble.transform.position = new Vector2 (jeep.transform.position.x, jeep.transform.position.y + 20);
			talkBubble.transform.localScale = new Vector2(1.1f, 1.1f);
			
			talkText = new GameObject ();
			talkText.transform.parent = jeep.transform;
			talkText.transform.position = new Vector3 (jeep.transform.position.x, jeep.transform.position.y +13, -5.0f);
			Destroy(talkText, talkStayTime);
			//talkText.renderer.material.color = Color.black;
			//talkText.transform.localScale = new Vector2(10, 10);
			
			MeshRenderer meshRenderer = talkText.AddComponent<MeshRenderer>();
			TextMesh talkLayer = talkText.AddComponent<TextMesh> ();
			//meshRenderer.material = (Resources.Load("Livingst") as Material);
			
			talkText.renderer.material = Resources.Load("arialbd", typeof(Material)) as Material; 
			talkLayer.text = "Don't look now,\nwe're being chased\nby Godzilla!";
			talkLayer.anchor = TextAnchor.LowerCenter;
			talkLayer.fontSize = 16; //20;
			talkLayer.characterSize = 2.0f;
			talkLayer.renderer.material.color = Color.black;
			Font myFont = Resources.Load("arialbd", typeof(Font)) as Font;
			talkLayer.font = myFont;
		}
		else if (timePassed > 62)
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
	}
}
