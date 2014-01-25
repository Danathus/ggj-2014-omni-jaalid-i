using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {
	Transform bulletPrefab;
	const float bulletSpeed = 200.0f;
	const float strayFactor = 90.0f;
	bool fired;

	// Use this for initialization
	void Start () {
		bulletPrefab = transform;
		bulletPrefab.position = new Vector2 (-3, -3);
		fired = false;


	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space") && !fired)
			Fire ();
	}

	void Fire()
	{
		fired = true;

		//get random angle and rotate
		var randomNumberZ = Random.Range(-strayFactor, strayFactor);
		Vector3 angle = new Vector3 (0.0f, 0.0f, randomNumberZ);
		bulletPrefab.transform.Rotate (angle);

		//let bullet move
		bulletPrefab.rigidbody.AddForce (bulletPrefab.transform.up * bulletSpeed);
		
		if (bulletPrefab.position.y > 8) {
			DestroyObject(this.gameObject);
		}
	}
}
