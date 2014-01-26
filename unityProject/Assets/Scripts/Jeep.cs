using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class Jeep : MonoBehaviour
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

		float measuredDistanceFromCamera = (camera.transform.position - transform.position).magnitude;
		float desiredDistanceFromCamera = timePassed > 40.0f ? 10.0f : 100.0f; // 40
		float speed = measuredDistanceFromCamera - desiredDistanceFromCamera; // 40

		Rigidbody2D rBody = GetComponent<Rigidbody2D>();
		rBody.velocity = new Vector2(speed, rBody.velocity.y);
		//transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z * 0.9f + rBody.velocity.y/10 * 0.1f);
		transform.eulerAngles = new Vector3(0, 0, rBody.velocity.y/10);
	}
}
