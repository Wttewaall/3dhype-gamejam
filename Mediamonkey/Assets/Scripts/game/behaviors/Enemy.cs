using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Behaviors/Enemy")]

public class Enemy : MonoBehaviour {
	
	public delegate void EnemyEvent(Enemy target);
	public event EnemyEvent death;
	
	public string type;
	public bool move = true;
	public Vector3 offset;
	public EnemyStats charStats;
	
	protected Transform tf;
	protected Healthbar healthbar;
	
	// ---- getters & setters ----
	
	public float health {
		get {
			return charStats.health;
		}
		set {
			// if already out of health, return
			if (charStats.health <=0) return;
			
			charStats.health = value;
			if (healthbar != null) healthbar.health = charStats.health / charStats.startHealth;
			if (charStats.startHealth > 0 && charStats.health <= 0) Die();
		}
	}
	
	// ---- inherited handlers ----
	
	void Awake() {
		tf = transform;
		healthbar = GetComponentInChildren<Healthbar>();
	}
	
	void Update() {
		// simplest behavior: move forward
		if (move) {
			tf.Translate(tf.TransformDirection(tf.forward)*0.05f);
			if (tf.position.z < -20) Die();
		}
	}
	
	void OnTriggerEnter(Collider other) {
		Bullet bullet = other.GetComponent<Bullet>();
		if (bullet != null) {
			health -= bullet.CalculateDamage(this);
			Statistics.targetsHit++;
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Bullet>() != null) {
			Statistics.targetsHit++;
		}
	}
	
	// ---- public methods ----
	
	public void Die() {
		DispatchEnemyEvent(death);
		
		// TODO: return to pool
		Destroy(gameObject);
	}
	
	// ---- protected methods ----
	
	protected void DispatchEnemyEvent(EnemyEvent evt) {
		if (evt != null) evt(this);
	}
}

[Serializable]
public class EnemyStats {
	
	// standard
	public float health			= 200;
	public float armor			= 200;
	
	[NonSerializedAttribute]
	public float startHealth;
	
	[NonSerializedAttribute]
	public float startArmor;
	
	// scalar resistance to certain attacks
	public float resistDirectHit;
	public float resistSplashDamage;
	public float resistKnockdown;
	
	// mental state
	public float courage		= 50; // -courage = fear
	public float rage			= 0; // -rage = happiness/joy
	
	public EnemyStats() {
		startHealth = health;
		startArmor = armor;
	}
	
}