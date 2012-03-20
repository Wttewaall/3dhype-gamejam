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
	public event RoundEventHandler OnPlay;
	//public event RoundEventHandler OnPaused;
	//public event RoundEventHandler OnUnpaused;
	//public event RoundEventHandler OnFailed;
	public event RoundEventHandler OnComplete;
	
	// public members
	public RoundSettings settings;
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
		state = RoundState.IDLE;
	}
	
	public void Start() {
		if (waves == null) waves = new List<Wave>();
		if (liveWaves == null) liveWaves = new List<Wave>();
		if (liveEnemies == null) liveEnemies = new List<Enemy>();
		
		dataProvider = new DataProvider<Wave>(waves);
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
	
	public void Play() {
		startTime = Time.time;
		
		if (dataProvider.length > 0) {
			NextWave();
			
			state = RoundState.RUNNING;
			DispatchEvent(OnPlay);
			
		} else {
			state = RoundState.IDLE;
			Debug.LogError("No waves to start");
			return;
		}
	}
	
	public void Pause() {
		stopTime = Time.time;
	}
	
	public void Stop() {
		stopTime = Time.time;
		state = RoundState.IDLE;
	}
	
	public void Update() {
		if (state == RoundState.RUNNING) {
			
			int currentWaveIndex = dataProvider.selectedIndex;
			Wave currentWave = dataProvider.selectedItem;
			
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
		}
	}
	
	public void NextWave() {
		Utils.trace("next wave:", dataProvider.selectedIndex + 1);
		dataProvider.Next();
	}
	
	override public string ToString() {
		return "Round "+index;
	}
	
	// ---- protected methods ----
	
	protected void SetEventHandlers(Wave target, bool adding) {
		if (target == null) return;
		
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
		if (currentTarget.selectedItem == null) {
			Debug.LogWarning("Cannot start: selected round is null");
			
		} else {
			SetEventHandlers(dataProvider.previousItem, false);
			
			if (dataProvider.previousItem == currentTarget.selectedItem) {
				DispatchEvent(OnComplete);
				Stop();
				
			} else {
				Debug.Log("Starting round: "+currentTarget.selectedItem);
				SetEventHandlers(currentTarget.selectedItem, true);
				dataProvider.selectedItem.startTime = Time.time;
				liveWaves.Add(dataProvider.selectedItem);
			}
		}
	}
	
	private void waveDepletedHandler(Wave target) {
		Utils.trace("waveDepleted", waves.IndexOf(target));
	}
	
	private void waveClearedHandler(Wave target) {
		Utils.trace("waveCleared", waves.IndexOf(target));
		liveWaves.Remove(target);
		SetEventHandlers(target, false);
		
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

[Serializable]
public class RoundSettings {
	
	public string name = "Round";
	// add many more settings
	
}