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

	enum CollisionShape
	{
		None,
		Square,
		Incline,
		Decline
	}

	void CreateTile(Vector2 pos, Sprite tileSprite, CollisionShape collisionShape)
	{
		var cube = new GameObject();
		switch (collisionShape)
		{
		case CollisionShape.Square:
			BoxCollider2D boxCollider = cube.AddComponent<BoxCollider2D>();
			boxCollider.size = new Vector2(tileWidth, tileHeight);
			break;
		case CollisionShape.Incline:
			{
				PolygonCollider2D polyCollider = cube.AddComponent<PolygonCollider2D>();
				Vector2[] points = new Vector2[3];
				points[0] = new Vector2(-tileWidth/2, -tileHeight/2);
				points[1] = new Vector2( tileWidth/2, -tileHeight/2);
				points[2] = new Vector2( tileWidth/2,  tileHeight/2);
				polyCollider.SetPath(0, points);
			}
			break;
		case CollisionShape.Decline:
			{
				PolygonCollider2D polyCollider = cube.AddComponent<PolygonCollider2D>();
				Vector2[] points = new Vector2[3];
				points[0] = new Vector2(-tileWidth/2,  tileHeight/2);
				points[1] = new Vector2(-tileWidth/2, -tileHeight/2);
				points[2] = new Vector2( tileWidth/2, -tileHeight/2);
				polyCollider.SetPath(0, points);
			}
			break;
		}
		SpriteRenderer spriteRenderer = cube.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = tileSprite;
		cube.transform.position = pos;
		cube.transform.parent = mapParent.transform;
		cube.name = "tile";
	}

	float DeterministicRandom(int input) // output in [0, 1]
	{
		/*
		UnityEngine.Random.seed = input;
		float randomValue = UnityEngine.Random.value;
		/*/
		System.Random random = new System.Random((int)(input * 100 ^ 0xdeadbeef));
		float randomValue = (float)random.Next(0, 1000) / 1000.0f;
		//*/
		//Debug.Log ("" + input + " -> " + randomValue);
		return randomValue;
	}

	int GenerateElevation(int x)
	{
		//int elevation = (int)(10.0f*Mathf.Sin(x / 100.0f * 2.0f * (float)Math.PI)) + 10; // sine wave function

		// make an elevation decision every 10 tiles
		int decisionFrequency = 10;
		// round down to the earlier decision point
		int earlierDecisionPoint = (x / decisionFrequency) * decisionFrequency;
		// and find the next decision point
		int laterDecisionPoint = earlierDecisionPoint + decisionFrequency;

		// find the heights at these decision points
		int midLevel = 20;
		float tileAmplitude = 10.0f;
		int earlierElevation = midLevel + (int)(DeterministicRandom(earlierDecisionPoint)*tileAmplitude);
		int laterElevation = midLevel + (int)(DeterministicRandom(laterDecisionPoint)*tileAmplitude);

		// interpolate to find our current elevation
		float interpolationFrac = (float)(x - earlierDecisionPoint) / decisionFrequency;
		float interpolatedElevation = earlierElevation * (1.0f - interpolationFrac) + laterElevation * interpolationFrac;
		int elevation = (int)interpolatedElevation;

		return elevation;
	}

	Sprite ChooseElevationTile(int y, int prevElevation, int currElevation, int nextElevation)
	{
		Sprite tileChoice = track_flat3; // default to underground
		if (y == currElevation-1) // if we're right on the top...
		{
			// default to flat top unless proven otherwise
			tileChoice = track_flat2;
			
			// consider previous and next elevation
			if (prevElevation < currElevation && currElevation <= nextElevation)
			{
				// incline
				tileChoice = track_incline2;
			}
			if (prevElevation >= currElevation && currElevation > nextElevation)
			{
				// decline
				tileChoice = track_decline2;
			}
		}
		return tileChoice;
	}

	Sprite ChooseAboveTile(Sprite elevationTile)
	{
		if (elevationTile == track_decline2)
		{
			return track_decline1;
		}
		if (elevationTile == track_incline2)
		{
			return track_incline1;
		}
		if (elevationTile == track_flat2)
		{
			return track_flat1;
		}
		return null;
	}

	CollisionShape ChooseCollisionShape(Sprite tile)
	{
		if (tile == track_decline2)
		{
			return CollisionShape.Decline;
		}
		if (tile == track_incline2)
		{
			return CollisionShape.Incline;
		}
		if (tile == track_flat2 || tile == track_flat3)
		{
			return CollisionShape.Square;
		}
		return CollisionShape.None;
	}
	
	void Start()
	{
		// create parent for map
		mapParent = new GameObject();
		mapParent.name = "map";

		// initialization
		track_decline1 = Resources.Load<Sprite>("Art/Tiles/track_decline1"); // far decline
		track_decline2 = Resources.Load<Sprite>("Art/Tiles/track_decline2"); // near decline
		track_flat1    = Resources.Load<Sprite>("Art/Tiles/track_flat1"); // far top of ground
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
		int prevElevation, currElevation = 0, nextElevation = GenerateElevation(1);
		for (int x = 0; x < 300; ++x)
		{
			prevElevation = currElevation;
			currElevation = nextElevation;
			nextElevation = GenerateElevation(x+1);
			for (int y = 0; y < currElevation; ++y)
			{
				Sprite tileChoice = ChooseElevationTile(y, prevElevation, currElevation, nextElevation);
				CreateTile(new Vector2(x * tileWidth, y * tileHeight) + offset, tileChoice, ChooseCollisionShape(tileChoice));
				if (y == currElevation-1)
				{
					Sprite aboveTile = ChooseAboveTile(tileChoice);
					CreateTile(new Vector2(x * tileWidth, (y+1) * tileHeight) + offset, aboveTile, ChooseCollisionShape(aboveTile));
				}
			}
		}
	}

	void Update()
	{
		// per frame update
	}
}
