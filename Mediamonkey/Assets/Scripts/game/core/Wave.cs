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
	
	public float spawnInterval = 0.3f;
	public float spawnTime; // depricate?
	
	protected GameObjectPool pool;
	protected Vector3 offset;
	
	// stats
	public int initialAmount;
	public int spawnAmount;
	public int destroyAmount;
	
	public bool hasEnemies {
		get { return (destroyAmount < initialAmount); }
	}
	
	// ---- constructor ----
	
	public Wave(GameObjectPool pool, int amount) {
		if (pool == null) throw new UnityException("pool cannot be null");
		this.pool = pool;
		this.initialAmount = amount;
		
		offset = pool.prefab.GetComponent<Enemy>().offset;
	}
	
	public Wave(GameObjectPool pool, int amount, float spawnInterval) : this(pool, amount) {
		this.spawnInterval = spawnInterval;
	}
	
	// ---- public methods ----
	
	public GameObject spawn(ISpawner spawner, Quaternion rotation) {
		return spawn(spawner.GetSpawnPosition(offset), rotation);
	}
	
	public GameObject spawn(Vector3 position, Quaternion rotation) {
		if (spawnAmount == initialAmount) return null;
		if (++spawnAmount == initialAmount) dispatchWaveEvent(waveDepleted);
		
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