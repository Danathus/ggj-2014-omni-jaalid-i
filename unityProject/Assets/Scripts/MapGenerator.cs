using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class MapGenerator : MonoBehaviour
{
	GameObject mapParent;
	void CreateTile(Vector2 pos)
	{
		var cube = GameObject.CreatePrimitive(PrimitiveType.Quad);
		UnityEngine.Object.DestroyImmediate(cube.GetComponent("MeshCollider"));
		BoxCollider2D boxCollider = cube.AddComponent<BoxCollider2D>();
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
		const float tileWidth = 1;
		const float tileHeight = 1;

		// create some tiles procedurally
		for (int x = 0; x < 10; ++x)
		{
			for (int y = 0; y < x; ++y)
			{
				CreateTile(new Vector2(x * tileWidth, y * tileHeight));
			}
		}
	}

	void Update()
	{
		// per frame update
	}
}
