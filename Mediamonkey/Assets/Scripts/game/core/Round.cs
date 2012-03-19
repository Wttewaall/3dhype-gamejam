using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * A Round consists of multiple waves
 * The round handles wave initializing, timing and scoring
 */

[Serializable]
public class Round {
	
	// events
	public event RoundEventHandler OnStart;
	//public event RoundEventHandler OnPaused;
	//public event RoundEventHandler OnUnpaused;
	//public event RoundEventHandler OnFailed;
	public event RoundEventHandler OnComplete;
	
	// public members
	public List<Wave> waves;
	public DataProvider<Wave> dataProvider;
	
	[NonSerialized]
	public int index;
	
	protected List<Wave> liveWaves;
	protected List<Enemy> liveEnemies;
	protected float startTime;
	protected float stopTime;	
	protected RoundState state;
	
	// ---- constructor ----
	
	public Round() {
		waves = new List<Wave>();
		liveWaves = new List<Wave>();
		liveEnemies = new List<Enemy>();
		
		state = RoundState.IDLE;
		
		dataProvider = new DataProvider<Wave>();
		dataProvider.OnIndexChange += indexChangeHandler;
	}

	// ---- public methods ----
	
	public Wave CreateWave(Spawner spawner, Transform goal) {
		// lookup pool by type
		Wave wave = new Wave(spawner, goal);
		waves.Add(wave);
		wave.index = waves.Count - 1;
		return wave;
	}
	
	public void Start() {
		startTime = Time.time;
		state = RoundState.RUNNING;
		DispatchEvent(OnStart);
	}
	
	public void Pause() {
		stopTime = Time.time;
	}
	
	public void Stop() {
		stopTime = Time.time;
		state = RoundState.IDLE;
	}
	
	public void Update() {
		/*if (state == RoundState.RUNNING) {
			
			// trigger first wave
			if (currentWaveIndex < 0) NextWave();
			
			if (Time.time >= currentWave.spawnTime) {
				currentWave.spawnTime = Time.time + currentWave.spawnInterval;
				
				// only start a next wave after all enemies are cleared
				if (!currentWave.hasEnemies || currentWave.timeUp) {
					NextWave();
					
				} else {
					currentWave.Spawn();
				}
			}
		}*/
	}
	
	public void NextWave() {
		/*
		if (currentWaveIndex + 1 < waves.Count) {
			currentWaveIndex++;
			//Utils.trace("next wave:", currentWaveIndex);
			
			SetWaveHandlers(currentWave, true);
			currentWave.startTime = Time.time;
			liveWaves.Add(currentWave);
			
		} else if (liveWaves.Count == 0) {
			DispatchEvent(OnComplete);
			Stop();
		}
		*/
	}
	
	override public string ToString() {
		return "Round "+index;
	}
	
	// ---- protected methods ----
	
	protected void SetWaveHandlers(Wave target, bool adding) {
		if (adding) {
			target.OnDepleted += waveDepletedHandler;
			target.OnCleared += waveClearedHandler;
			target.OnEnemySpawned += enemySpawnedHandler;
			target.OnEnemyDestroyed += enemyDestroyedHandler;
			
		} else {
			target.OnDepleted -= waveDepletedHandler;
			target.OnCleared -= waveClearedHandler;
			target.OnEnemySpawned -= enemySpawnedHandler;
			target.OnEnemyDestroyed -= enemyDestroyedHandler;
		}
	}
	
	protected void DispatchEvent(RoundEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	// ---- event handlers ----
	
	private void indexChangeHandler (IndexChangeEventType type, DataProvider<Wave> currentTarget, int oldIndex, int newIndex) {
		//..
	}
	
	private void waveDepletedHandler(Wave target) {
		//Utils.trace("waveDepleted", waves.IndexOf(target));
	}
	
	private void waveClearedHandler(Wave target) {
		//Utils.trace("waveCleared", waves.IndexOf(target));
		liveWaves.Remove(target);
		SetWaveHandlers(target, false);
		
		if (liveWaves.Count == 0) DispatchEvent(OnComplete);
	}
	
	private void enemySpawnedHandler(Wave target, Enemy enemy) {
		liveEnemies.Add(enemy);
	}
	
	private void enemyDestroyedHandler(Wave target, Enemy enemy) {
		liveEnemies.Remove(enemy);
	}
	
	// ---- delegates ----
	
	public delegate void RoundEventHandler(Round target);
	
}