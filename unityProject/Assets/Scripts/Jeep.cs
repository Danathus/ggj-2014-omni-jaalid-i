using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class Jeep : MonoBehaviour
{
	void Start()
	{
		transform.position += new Vector3(0, 20, 0);
	}

	void Update()
	{
		Rigidbody2D rBody = GetComponent<Rigidbody2D>();
		rBody.velocity = new Vector2(40, rBody.velocity.y);
		//transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z * 0.9f + rBody.velocity.y/10 * 0.1f);
		transform.eulerAngles = new Vector3(0, 0, rBody.velocity.y/10);
	}
}
