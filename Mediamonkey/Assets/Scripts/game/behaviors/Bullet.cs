using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

[AddComponentMenu("King's Ruby/Behaviors/Bullet")]

public class Bullet : MonoBehaviour {
	
	public GameObject explosion;
	public AudioClip explosionSound;
	public HitStats hitStats;
	
	protected SphereCollider splashCollider;
	protected float radius;
	protected bool removeAfterExplosion;
	
	// ---- getters & setters ----
	
	private bool _exploded = false;
	
	public bool exploded {
		get { return _exploded; }
		private set {
			_exploded = value;
		}
	}
	
	// ---- inherited handlers ----
	
	void Start() {
		splashCollider = gameObject.GetComponent<SphereCollider>();
		radius = splashCollider.radius;
	}
	
	/*void OnTriggerEnter(Collider other) {
		Utils.trace("OnTriggerEnter", other.name);
	}*/
	
	void OnCollisionEnter(Collision collision) {
		Explode();
	}
	
	void FixedUpdate() {
		if (removeAfterExplosion && splashCollider.isTrigger) {
			removeAfterExplosion = false;
			
			AutoDestruct ad = GetComponent<AutoDestruct>();
			if (ad != null) ad.DestroyNow();
			else DestroyObject(gameObject);
		}
	}
	
	// ---- public methods ----
	
	public void Explode() {
		if (exploded) return;
		exploded = true;
		
		// hide visible gameobject
		//renderer.enabled = false;
		
		// stop all movement
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.useGravity = false;
		
		// set the splash radius
		splashCollider.radius = hitStats.splashRadius;
		splashCollider.isTrigger = true;
		
		// remove on the next fixedUpdate as not to hit things twice
		removeAfterExplosion = true;
		
		// TODO: add hitStats.duration
		// do not re-explode, but keep colliding and handing out damage.
		
		// show explosion
		GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		
		// add audiosource if missing
		if (!exp.audio) exp.AddComponent<AudioSource>();
		exp.audio.rolloffMode = AudioRolloffMode.Linear;
		exp.audio.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
		exp.audio.PlayOneShot(explosionSound);
		
		AutoDestruct ad = GetComponent<AutoDestruct>();
		ad = ad ?? gameObject.AddComponent<AutoDestruct>();
		ad.DestroyTimed(4);
	}
	
	// do not explode on hit, but deploy as a trap
	// like: mines, bear traps, electricity, poisoned gas or smoke
	public void Deploy() {
		// TODO for traps
	}
	
	// calculate all sorts of damage types
	public float CalculateDamage(Enemy target) {
		
		float damage = 0;
		float dist = Vector3.Distance(transform.position, target.transform.position);
		
		// direct hit?
		if (dist <= target.collider.bounds.extents.magnitude) {
			damage = hitStats.directHitDamage;
		
		// splash damage
		} else if (hitStats.splashDamage > 0) {
			damage = Mathf.Lerp(hitStats.splashDamage, hitStats.minDamage, dist/hitStats.splashRadius);
		}
		
		return damage;
	}
	
	public void Reset() {
		exploded = false;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.useGravity = true;
		splashCollider.radius = radius;
		splashCollider.isTrigger = false;
	}
	
}

[Serializable]
public class HitStats {
	
	public float minDamage			= 0;
	public float directHitDamage	= 150;
	public float splashDamage		= 80;
	public float splashRadius		= 15;
	public float duration			= 0;
	
}