using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class MapGenerator : MonoBehaviour
{	
	Sprite track_decline1;
	Sprite track_decline2;
	Sprite track_flat1;
	Sprite track_flat2;
	Sprite track_flat3;
	Sprite track_incline1;
	Sprite track_incline2;

	GameObject mapParent;
	const float tileWidth  = 4;
	const float tileHeight = 4;

	void CreateTile(Vector2 pos, Sprite tileSprite)
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

	int GenerateElevation(int x)
	{
		int elevation = (int)(10.0f*Mathf.Sin(x / 100.0f * 2.0f * (float)Math.PI)) + 10;
		return elevation;
	}

	void Start()
	{
		// create parent for map
		mapParent = new GameObject();
		mapParent.name = "map";

		// initialization
		track_decline1 = Resources.Load<Sprite>("Art/Tiles/track_decline1"); // far incline
		track_decline2 = Resources.Load<Sprite>("Art/Tiles/track_decline2"); // near incline
		track_flat1    = Resources.Load<Sprite>("Art/Tiles/track_flat1");
		track_flat2    = Resources.Load<Sprite>("Art/Tiles/track_flat2"); // top of ground (w/ grass)
		track_flat3    = Resources.Load<Sprite>("Art/Tiles/track_flat3"); // underground
		track_incline1 = Resources.Load<Sprite>("Art/Tiles/track_incline1"); // far incline
		track_incline2 = Resources.Load<Sprite>("Art/Tiles/track_incline2"); // near incline

		Sprite[] tiles = new Sprite[7];
		tiles[0] = track_decline1;
		tiles[1] = track_decline2;
		tiles[2] = track_flat1;
		tiles[3] = track_flat2;
		tiles[4] = track_flat3;
		tiles[5] = track_incline1;
		tiles[6] = track_incline2;

		Vector2 offset = new Vector2(-75, -75);

		// create some tiles procedurally
		//int tileIdx = 0;
		for (int x = 0; x < 1000; ++x)
		{
			int elevation = GenerateElevation(x);
			for (int y = 0; y < elevation; ++y)
			{
				Sprite tileChoice = track_flat3; // default to underground
				if (y == elevation-1) // if we're right on the top...
				{
					// for now just choose flat top
					tileChoice = track_flat2;
				}
				//CreateTile(new Vector2(x * tileWidth, y * tileHeight) + offset, track_flat3);
				//CreateTile(new Vector2(x * tileWidth, y * tileHeight) + offset, tiles[tileIdx]);
				//tileIdx = (tileIdx + 1) % tiles.Length;
				CreateTile(new Vector2(x * tileWidth, y * tileHeight) + offset, tileChoice);
			}
		}
	}

	void Update()
	{
		// per frame update
	}
}
