using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * A wave consists of one type of enemy (pool)
 * The wave handles spawning, destruction and the number of enemies
 */

[Serializable]
public class Wave : PlayableItem {
	
	public event WaveEventHandler	OnWaveStarted;
	public event WaveEventHandler	OnWaveDepleted;
	public event WaveEventHandler	OnWaveCleared;
	public event SpawnEventHandler	OnEnemySpawned;
	public event SpawnEventHandler	OnEnemyDestroyed;
	
	public Spawner spawner;
	public Transform goal;
	public List<Group> groups;
	
	[NonSerialized] public int index;
	[NonSerialized] public float startTime = 0;
	[NonSerialized] public float spawnTime = 0;
	[NonSerialized] public float delay = 0;
	
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
	public Wave(int index) {
		this.index = index;
	}
	
	override public void Initialize() {
		base.Initialize();
		timer.repeatCount = 0;
		// TODO: get pools, setup spawning
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
		if (++spawnAmount == initialAmount) DispatchEvent(OnWaveDepleted);
		
		if (startTime == 0) startTime = Time.time;
		
		// TODO - find pool by Game.
		Game.instance.poolManager.GetPoolByEnemyType(EnemyType.ARCHER);
		
		Quaternion q = Quaternion.LookRotation(spawner.GetSpawnDirection());
		GameObject go = pool.Spawn(spawner.GetSpawnPosition() + offset, q);
		var enemy = go.GetComponent<Enemy>();
		enemy.OnDeath += enemyDeathHandler;
		enemy.goal = goal;
		
		DispatchEvent(OnEnemySpawned, enemy);
		
		return go;
	}
	
	override public bool Play() {
		DispatchEvent(OnWaveStarted);
		return base.Play();
	}
	
	override public string ToString() {
		return "Wave "+index;
	}
	
	// ---- event handlers ----
	
	override protected void timerTickHandler(Timer target) {
		
		if (target.currentCount < groups.Count-1) {
			Utils.trace("\t\t", this, "- spawn", groups[target.currentCount]);
		}
		
		// spawn 3 enemies
		if (target.currentCount == groups.Count-1) {
			DispatchEvent(OnWaveDepleted);
		
		// enemies are defeated after 5 seconds
		} else if (target.currentCount == 5) {
			DispatchEvent(OnWaveCleared);
			Stop();
		}
	}
	
	override protected void timerCompleteHandler(Timer target) {
		DispatchCompleteEvent();
	}
	
	protected void enemyDeathHandler(Enemy target) {
		DispatchEvent(OnEnemyDestroyed, target);
		if (++destroyAmount == initialAmount) DispatchEvent(OnWaveCleared);
	}
	
	// ---- protected methods ----
	
	protected void DispatchEvent(WaveEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	protected void DispatchEvent(SpawnEventHandler evt, Enemy enemy) {
		if (evt != null) evt(this, enemy);
	}
	
	// ---- delegates ----
	
	public delegate void WaveEventHandler(Wave target);
	public delegate void SpawnEventHandler(Wave target, Enemy enemy);
	
}