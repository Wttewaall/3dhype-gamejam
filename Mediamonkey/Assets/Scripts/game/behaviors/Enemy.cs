using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Behaviors/Enemy")]

public class Enemy : MonoBehaviour {
	
	public delegate void EnemyEvent(Enemy target);
	public event EnemyEvent death;
	
	public Vector3 offset;
	public EnemyStats characterStats;
	
	protected Transform tf;
	
	// ---- inherited handlers ----
	
	void Awake() {
		tf = transform;
	}
	
	void Update() {
		// simplest behavior: move forward
		tf.Translate(tf.TransformDirection(tf.forward)*0.05f);
		
		if (tf.position.z < -20) Die();
	}
	
	void OnMouseOver() {
		Die();
	}
	
	// ---- public methods ----
	
	public void Die() {
		DispatchEnemyEvent(death);
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
	public float healthDamage	= 0;
	
	[NonSerializedAttribute]
	public float armorDamage	= 0;
	
	// resistance to certain attacks
	public float resistDirectHit;
	public float resistSplashDamage;
	public float resistKnockdown;
	
	// mental state
	public float courage		= 50; // -courage = fear
	public float rage			= 0; // -rage = happiness/joy
	
}