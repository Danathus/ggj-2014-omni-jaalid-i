using UnityEngine;
using System.Collections;
using GamepadInput;
using System;
using CBX.TileMapping.Unity;

public class Godzilla : MonoBehaviour
{
	public Camera camera;
	float timePassed;
	public bool shouldGetClose = false;
	AudioClip GodzillaFootstep, GodzillaFootstep2, GodzillaFootstep3;

	void Start()
	{
		transform.position += new Vector3(0, 20, 0);
		timePassed = 0;
		// do not collide with obstacles
		Physics2D.IgnoreLayerCollision(8, 9, false);

		GodzillaFootstep = Resources.Load<AudioClip>("Soundfx/GodzillaFootstep");
		GodzillaFootstep2 = Resources.Load<AudioClip>("Soundfx/GodzillaFootstep2");
		GodzillaFootstep3 = Resources.Load<AudioClip>("Soundfx/GodzillaFootstep3");
	}

	GameObject PlayClipAt(AudioClip clip, Vector3 pos)
	{
		var tempGO = new GameObject("TempAudio"); // create the temp object
		tempGO.transform.position = pos; // set its position
		tempGO.AddComponent<AudioSource>(); // add an audio source
		tempGO.audio.clip = clip; // define the clip
		tempGO.audio.Play(); // start the sound
		Destroy(tempGO, clip.length); // destroy object after clip duration
		return tempGO; // return reference to the temporary GameObject
	}

	GameObject isPlaying = null;
	void Update()
	{
		timePassed += Time.deltaTime;

		float measuredDistanceFromCamera = (camera.transform.position.x - transform.position.x);
		float desiredDistanceFromCamera = shouldGetClose ? 20.0f : 200.0f; // 40
		float speed = measuredDistanceFromCamera - desiredDistanceFromCamera; // 40

		Rigidbody2D rBody = GetComponent<Rigidbody2D>();
		//rBody.velocity = new Vector2(speed, shouldGetClose ? rBody.velocity.y - Time.deltaTime * 1000 : rBody.velocity.y);
		rBody.velocity = new Vector2(speed, rBody.velocity.y);
		//transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z * 0.9f + rBody.velocity.y/10 * 0.1f);
		transform.eulerAngles = new Vector3(0, 0, rBody.velocity.y/10);

		rBody.WakeUp();
		Debug.Log (rBody.velocity);

		if (shouldGetClose && rBody.velocity.x > 10)
		{
			if (!isPlaying)
			{
				isPlaying = PlayClipAt(GodzillaFootstep, camera.transform.position);
				//AudioSource.PlayClipAtPoint(GodzillaFootstep, camera.transform.position);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.name == "obstacle")
		{
			GameObject.Destroy(collision.gameObject);
		}
    }
}
