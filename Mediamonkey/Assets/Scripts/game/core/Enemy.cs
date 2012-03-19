using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Behaviors/Enemy")]

public class Enemy : MonoBehaviour {
	
	public event EnemyEvent OnDeath;
	
	public delegate void EnemyEvent(Enemy target);
	
	public EnemyType type;
	public bool move = true;
	public Vector3 offset;
	public EnemyStatistics stats;
	public Transform goal;
	
	protected Transform tf;
	protected Healthbar healthbar;
	protected NavMeshAgent agent;
	
	// ---- getters & setters ----
	
	public float health {
		get {
			return stats.health;
		}
		set {
			// if already out of health, return
			if (stats.health <= 0) return;
			
			stats.health = value;
			if (healthbar != null) healthbar.health = stats.health / stats.startHealth;
			if (stats.startHealth > 0 && stats.health <= 0) Die();
		}
	}
	
	// ---- inherited handlers ----
	
	void Awake() {
		tf = transform;
		healthbar = GetComponentInChildren<Healthbar>();
		
		agent = GetComponent<NavMeshAgent>();
		if (!agent) agent = gameObject.AddComponent<NavMeshAgent>();
	}
	
	void Update() {
		if (goal != null) agent.SetDestination(goal.position);
		
		/*// simplest behavior: move forward
		if (move) {
			tf.Translate(tf.TransformDirection(tf.forward)*0.05f);
			if (tf.position.z < -20) Die();
		}//*/
	}
	
	void OnTriggerEnter(Collider other) {
		Bullet bullet = other.GetComponent<Bullet>();
		if (bullet != null) {
			health -= bullet.CalculateDamage(this);
			GameStatistics.targetsHit++;
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Bullet>() != null) {
			GameStatistics.targetsHit++;
		}
	}
	
	// ---- public methods ----
	
	public void Die() {
		DispatchEvent(OnDeath);
		
		// TODO: return to pool
		Destroy(gameObject);
	}
	
	// ---- protected methods ----
	
	protected void DispatchEvent(EnemyEvent evt) {
		if (evt != null) evt(this);
	}
}