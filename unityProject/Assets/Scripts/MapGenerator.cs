using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class MapGenerator : MonoBehaviour
{
	Sprite[] textures;
	string[] names;
	Sprite tileSprite;

	GameObject mapParent;
	const float tileWidth  = 4;
	const float tileHeight = 4;

	void CreateTile(Vector2 pos)
	{
		var cube = new GameObject();
		BoxCollider2D boxCollider = cube.AddComponent<BoxCollider2D>();
		boxCollider.size = new Vector2(tileWidth, tileHeight);
		SpriteRenderer spriteRenderer = cube.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = tileSprite;
		cube.transform.position = pos;
		cube.transform.parent = mapParent.transform;
		cube.name = "tile";
	}

	void Start()
	{
		// create parent for map
		mapParent = new GameObject();
		mapParent.name = "map";

		// initialization
		//tileSprite = Resources.Load<Sprite>("Art/Basketballer/basketballer_blue_jump1");
		tileSprite = Resources.Load<Sprite>("Art/Tiles/track_flat3");

		Vector2 offset = new Vector2(-75, -75);

		// create some tiles procedurally
		for (int x = 0; x < 100; ++x)
		{
			int elevation = (int)(10.0f*Mathf.Sin(x / 100.0f * 2.0f * (float)Math.PI)) + 10;
			for (int y = 0; y < elevation; ++y)
			{
				CreateTile(new Vector2(x * tileWidth, y * tileHeight) + offset);
			}
		}
	}

	void Update()
	{
		// per frame update
	}
}
