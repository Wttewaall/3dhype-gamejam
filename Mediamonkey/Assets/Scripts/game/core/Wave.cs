using UnityEngine;
using System;

/**
 * A wave consists of one type of enemy (pool)
 * The wave handles spawning, destruction and the number of enemies
 */

public class Wave {
	
	public delegate void WaveEvent(Wave target);
	public event WaveEvent waveDepleted;
	public event WaveEvent waveCleared;
	
	public delegate void SpawnEvent(Wave target, Enemy enemy);
	public event SpawnEvent enemySpawned;
	public event SpawnEvent enemyDestroyed;
	
	public float startTime = 0;
	public float spawnTime = 0;
	public float waitTime = 0;
	
	protected GameObjectPool pool;
	protected Vector3 offset;
	
	// stats
	public int initialAmount;
	public int spawnAmount;
	public int destroyAmount;
	
	// ---- getters & setters ----
	
	private float _spawnInterval = 0.3f;
	
	public float spawnInterval {
		get { return _spawnInterval; }
		set { _spawnInterval = value; }
	}
	
	public bool timeUp {
		get {
			if (startTime == 0) return false;
			else return (Time.time >= startTime + spawnInterval * initialAmount + waitTime);
		}
	}
	
	public bool hasEnemies {
		get { return (destroyAmount < initialAmount); }
	}
	
	// ---- constructor ----
	
	public Wave(GameObjectPool pool, int amount, float spawnInterval, float waitTime) {
		if (pool == null) throw new UnityException("pool cannot be null");
		this.pool = pool;
		this.initialAmount = amount;
		this.spawnInterval = spawnInterval;
		this.waitTime = waitTime;
		
		offset = pool.prefab.GetComponent<Enemy>().offset;
	}
	
	// ---- public methods ----
	
	public GameObject spawn(ISpawner spawner, Quaternion rotation) {
		return spawn(spawner.GetSpawnPosition(offset), rotation);
	}
	
	public GameObject spawn(Vector3 position, Quaternion rotation) {
		if (spawnAmount == initialAmount) return null;
		if (++spawnAmount == initialAmount) dispatchWaveEvent(waveDepleted);
		
		if (startTime == 0) startTime = Time.time;
		
		GameObject go = pool.spawn(position + offset, rotation);
		Enemy script = go.GetComponent<Enemy>();
		script.death += enemyDeathHandler;
		
		dispatchSpawnEvent(enemySpawned, script);
		
		return go;
	}
	
	// ---- event handlers ----
	
	protected void enemyDeathHandler(Enemy target) {
		dispatchSpawnEvent(enemyDestroyed, target);
		if (++destroyAmount == initialAmount) dispatchWaveEvent(waveCleared);
	}
	
	// ---- protected methods ----
	
	protected void dispatchWaveEvent(WaveEvent evt) {
		if (evt != null) evt(this);
	}
	
	protected void dispatchSpawnEvent(SpawnEvent evt, Enemy enemy) {
		if (evt != null) evt(this, enemy);
	}
	
}