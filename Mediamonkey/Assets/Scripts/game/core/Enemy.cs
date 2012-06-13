using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Behaviors/Enemy")]

public class Enemy : MonoBehaviour {
	
	public event EnemyEventHandler OnEnemieSpawn;
	public event EnemyEventHandler OnEnemieDeath;
	
	public EnemyType type;
	public bool move = true;
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
		Initialize();
	}
	
	void Start() {
		DispatchEnemyEvent(OnEnemieSpawn);
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
	
	public virtual void Initialize() {
		tf = transform;
		
		healthbar = GetComponentInChildren<Healthbar>();
		
		//agent = GetComponent<NavMeshAgent>();
		//if (!agent) agent = gameObject.AddComponent<NavMeshAgent>();
	}
	
	public virtual void Die() {
		DispatchEnemyEvent(OnEnemieDeath);
		
		// TODO: return to pool
		DestroyImmediate(gameObject);
	}
	
	// ---- protected methods ----
	
	protected void DispatchEnemyEvent(EnemyEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	// ---- delegates ----
	
	public delegate void EnemyEventHandler(Enemy target);
}