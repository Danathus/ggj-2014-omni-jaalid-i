using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class Obstacle : MonoBehaviour
{
	public enum Type
	{
		Floating,
		Crawling
	}
	public Type type = Type.Crawling;

	void Start()
	{
		switch (type)
		{
		case Type.Floating:
			transform.position = new Vector2(transform.position.x, transform.position.y + 5);
			break;
		default:
			break;
		}
	}

	void Update()
	{
		var rBody = GetComponent<Rigidbody2D>();
		switch (type)
		{
		case Type.Floating:
			rBody.velocity = new Vector2(rBody.velocity.y, 10);
			break;
		case Type.Crawling:
			rBody.velocity = new Vector2(-10, rBody.velocity.y);
			break;
		default:
			break;
		}
	}
}
