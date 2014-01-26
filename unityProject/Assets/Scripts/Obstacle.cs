using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class Obstacle : MonoBehaviour
{
	void Start()
	{
	}

	void Update()
	{
		var rBody = GetComponent<Rigidbody2D>();
		rBody.velocity = new Vector2(-10, rBody.velocity.y);
	}
}
