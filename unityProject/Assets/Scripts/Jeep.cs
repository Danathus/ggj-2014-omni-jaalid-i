using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class Jeep : MonoBehaviour
{
	public Camera camera;
	float timePassed;

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

	void Update()
	{
		timePassed += Time.deltaTime;

		Rigidbody2D rBody = GetComponent<Rigidbody2D>();

		switch (movementMode)
		{
		case MovementMode.AIControl:
			float measuredDistanceFromCamera = (camera.transform.position - transform.position).magnitude;
			float desiredDistanceFromCamera = timePassed > 40.0f ? 10.0f : 100.0f; // 40
			float speed = measuredDistanceFromCamera - desiredDistanceFromCamera; // 40

			rBody.velocity = new Vector2(speed, rBody.velocity.y);
			break;
		case MovementMode.PlayerControl:
			GamepadState state = GamePad.GetState(GamePad.Index.One);
			float baseSpeed = 100.0f;
			rBody.velocity = new Vector2((state.LeftStickAxis.x + 2) * baseSpeed, rBody.velocity.y);
			break;
		}
		//transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z * 0.9f + rBody.velocity.y/10 * 0.1f);
		transform.eulerAngles = new Vector3(0, 0, rBody.velocity.y/10);
	}
}
