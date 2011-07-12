using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]

[AddComponentMenu("King's Ruby/Behaviors/Bullet")]

public class Bullet : MonoBehaviour {
	
	public GameObject explosion;
	public AudioClip explosionSound;
	public HitStats hitStats;
	
	[NonSerialized]
	public bool exploded = false;
	
	/*void FixedUpdate() {
		//..
	}*/
	
	/*void OnTriggerEnter(Collider other) {
		Utils.trace("TriggerEnter", other.gameObject.name);
		Explode(transform.position);
	}*/
	
	void OnCollisionEnter(Collision collision) {
		Explode(collision.contacts[0].point);
	}
	
	/*void OnCollisionStay(Collision collision) {
		Utils.trace("CollisionStay", collision.gameObject.name);
	}
	
	void OnCollisionExit(Collision collision) {
		Utils.trace("CollisionExit", collision.gameObject.name);
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit) {
		Utils.trace("ColliderHit", hit.gameObject.name);
		
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic) return;
		
		Explode(hit.point);
	}*/
	
	public void Explode(Vector3 point) {
		if (exploded) return;
		exploded = true;
		
		GameObject.Instantiate(explosion, point, Quaternion.identity);
		
		audio.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
		audio.PlayOneShot(explosionSound);
		
		GetComponent<AutoDestruct>().DestroyTimed(2);
	}
	
}
