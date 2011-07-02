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
	
	public float spawnInterval = 0.3f;
	public float spawnTime; // depricate?
	
	protected GameObjectPool pool;
	protected Vector3 offset;
	
	// stats
	protected int initialAmount;
	protected int spawnAmount;
	protected int destroyAmount;
	
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
	
	public GameObject spawn(SpawnBox spawner, Quaternion rotation) {
		return spawn(spawner.GetSpawnPosition(offset), rotation);
	}
	
	public GameObject spawn(Vector3 position, Quaternion rotation) {
		if (spawnAmount == initialAmount) return null;
		if (++spawnAmount == initialAmount) dispatchEvent(waveDepleted);
		
		GameObject go = pool.spawn(position + offset, rotation);
		Enemy script = go.GetComponent<Enemy>();
		script.death += enemyDeathHandler;
		
		return go;
	}
	
	// ---- event handlers ----
	
	protected void enemyDeathHandler(Enemy target) {
		if (++destroyAmount == initialAmount) dispatchEvent(waveCleared);
	}
	
	// ---- protected methods ----
	
	protected void dispatchEvent(WaveEvent evt) {
		if (evt != null) evt(this);
	}
	
}