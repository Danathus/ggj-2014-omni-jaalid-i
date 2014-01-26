using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class Godzilla : MonoBehaviour
{
	public Camera camera;
	float timePassed;
	void Start()
	{
		transform.position += new Vector3(0, 20, 0);
		timePassed = 0;
	}

	void Update()
	{
		timePassed += Time.deltaTime;

		float measuredDistanceFromCamera = (camera.transform.position.x - transform.position.x);
		float desiredDistanceFromCamera = timePassed > 66.0f ? 20.0f : 200.0f; // 40
		float speed = measuredDistanceFromCamera - desiredDistanceFromCamera; // 40

		Rigidbody2D rBody = GetComponent<Rigidbody2D>();
		rBody.velocity = new Vector2(speed, rBody.velocity.y);
		//transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z * 0.9f + rBody.velocity.y/10 * 0.1f);
		transform.eulerAngles = new Vector3(0, 0, rBody.velocity.y/10);
	}
}
