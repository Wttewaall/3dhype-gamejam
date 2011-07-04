using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Core/Enemy")]

public class Enemy : MonoBehaviour {
	
	public delegate void EnemyEvent(Enemy target);
	public event EnemyEvent death;
	
	public Vector3 offset;
	public EnemyStats stats;
	
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
		dispatchEvent(death);
		Destroy(gameObject);
	}
	
	// ---- protected methods ----
	
	protected void dispatchEvent(EnemyEvent evt) {
		if (evt != null) evt(this);
	}
}

[Serializable]
public class EnemyStats {
	
	// standard
	public float health;
	public float healthDamage;
	public float armor;
	public float armorDamage;
	
	// resistance to certain attacks
	public float direct;
	public float splash;
	public float knockdown;
	
	// mental state
	public float courage; // -courage = fear
	public float rage; // -rage = happiness/joy
	
}