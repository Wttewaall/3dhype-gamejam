using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * A wave consists of one type of enemy (pool)
 * The wave handles spawning, destruction and the number of enemies
 */

[Serializable]
public class Wave {
	
	public event WaveEventHandler OnDepleted;
	public event WaveEventHandler OnCleared;
	public event SpawnEventHandler OnEnemySpawned;
	public event SpawnEventHandler OnEnemyDestroyed;
	
	public delegate void WaveEventHandler(Wave target);
	public delegate void SpawnEventHandler(Wave target, Enemy enemy);
	
	public Spawner spawner;
	public Transform goal;
	public List<Group> groups;
	
	[NonSerialized]
	public int index;
	
	protected float startTime = 0;
	protected float spawnTime = 0;
	protected float delay = 0;
	
	protected GameObjectPool pool;
	protected Vector3 offset;
	
	// stats
	protected int initialAmount;
	protected int spawnAmount;
	protected int destroyAmount;
	
	// ---- getters & setters ----
	
	private float _spawnInterval = 0.3f;
	
	public float spawnInterval {
		get { return _spawnInterval; }
		set { _spawnInterval = value; }
	}
	
	public bool timeUp {
		get {
			if (startTime == 0) return false;
			else return (Time.time >= startTime + spawnInterval * initialAmount + delay);
		}
	}
	
	public bool hasEnemies {
		get { return (destroyAmount < initialAmount); }
	}
	
	// ---- constructor ----
	
	public Wave(Spawner spawner, Transform goal) {
		/*if (pool == null) throw new UnityException("pool cannot be null");
		this.pool = pool;
		this.initialAmount = amount;
		this.spawnInterval = spawnInterval;
		this.delay = delay;
		
		offset = pool.prefab.GetComponent<Enemy>().offset;*/
	}
	
	// ---- public methods ----
	
	public Group CreateGroup(EnemyType type, int amount, float delay) {
		// lookup pool by type
		var g = new Group(type, amount);
		groups.Add(g);
		return g;
	}
	
	public GameObject Spawn() {
		if (spawnAmount == initialAmount) return null;
		if (++spawnAmount == initialAmount) DispatchEvent(OnDepleted);
		
		if (startTime == 0) startTime = Time.time;
		
		Quaternion q = Quaternion.LookRotation(spawner.GetSpawnDirection());
		GameObject go = pool.Spawn(spawner.GetSpawnPosition() + offset, q);
		var enemy = go.GetComponent<Enemy>();
		enemy.OnDeath += enemyDeathHandler;
		enemy.goal = goal;
		
		DispatchEvent(OnEnemySpawned, enemy);
		
		return go;
	}
	
	override public string ToString() {
		return "Wave "+index;
	}
	
	// ---- event handlers ----
	
	protected void enemyDeathHandler(Enemy target) {
		DispatchEvent(OnEnemyDestroyed, target);
		if (++destroyAmount == initialAmount) DispatchEvent(OnCleared);
	}
	
	// ---- protected methods ----
	
	protected void DispatchEvent(WaveEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	protected void DispatchEvent(SpawnEventHandler evt, Enemy enemy) {
		if (evt != null) evt(this, enemy);
	}
	
}