using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {
	Transform bulletPrefab;
	const float bulletSpeed = 100.0f;

	// Use this for initialization
	void Start () {
		bulletPrefab = transform;
		bulletPrefab.position = new Vector2 (-3, -3);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space"))
			Fire ();
	}

	void Fire()
	{
		//bulletPrefab.Translate (Vector3.up * bulletSpeed * 0.5f);
		bulletPrefab.rigidbody.AddForce (bulletPrefab.transform.up * bulletSpeed);
		
		if (bulletPrefab.position.y > 8) {
			DestroyObject(this.gameObject);
		}
	}
}
