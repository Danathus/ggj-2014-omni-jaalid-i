using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class Jeep : MonoBehaviour
{
	public Camera camera;
	float timePassed;
	public bool shouldGetClose = false;

	public enum MovementMode
	{
		AIControl,
		PlayerControl
	}
	public MovementMode movementMode = MovementMode.AIControl;

	void Start()
	{
		transform.position += new Vector3(0, 20, 0);
		timePassed = 0;
	}

	GameObject isTalking;
	void Update()
	{
		timePassed += Time.deltaTime;

		Rigidbody2D rBody = GetComponent<Rigidbody2D>();

		switch (movementMode)
		{
		case MovementMode.AIControl:
			float measuredDistanceFromCamera = (camera.transform.position - transform.position).magnitude;
			float desiredDistanceFromCamera = shouldGetClose ? 10.0f : 100.0f; // 40
			float speed = measuredDistanceFromCamera - desiredDistanceFromCamera; // 40

			rBody.velocity = new Vector2(speed, rBody.velocity.y);
			break;
		case MovementMode.PlayerControl:
			GamepadState state = GamePad.GetState(GamePad.Index.One);
			float baseSpeed = 30.0f;
			rBody.velocity = new Vector2((state.LeftStickAxis.x + 2) * baseSpeed, rBody.velocity.y);
			if (state.X && !isTalking)
			{
				isTalking = TheManager.Say(gameObject, drivingJokes[UnityEngine.Random.Range(0, drivingJokes.Length) % drivingJokes.Length], 3.0f);
			}
			break;
		}
		//transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z * 0.9f + rBody.velocity.y/10 * 0.1f);
		transform.eulerAngles = new Vector3(0, 0, rBody.velocity.y/10);
	}

	static string[] drivingJokes = new string[]{
		"If the opposite\nof Pro is Con.\nIs the opposite of\nProgress, Congress?",
		"Why do they\nsterilise needles,\nused for\nlethal injections?",
		"Why isn't\nginger red?",
		"If corporations are\npeople, is the\nstock market\nslave trading?",
		"If life is\nunfair to everyone,\nis it fair?",
		"Is this\nstick shift?",
		"Hey! Did you\nremember to fill\nup the gas tank?",
		"*putt* *putt* poof.\nI think our car\nis running on fumes",
		"Sorry that was me,\nI had a burrito",
		"Are we there yet?",
		"And I'd like to\ntake a minute\njust sit right\nthere...",
		"I just wanna tell\nyou how I'm feeling,\nGotta make you\nunderstand..."
	};
}
