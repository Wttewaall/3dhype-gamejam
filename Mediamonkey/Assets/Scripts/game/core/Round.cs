using UnityEngine;
using System.Collections.Generic;

/**
 * A Round consists of multiple waves
 * The round handles wave initializing, timing and scoring
 */

public class Round {
	
	public delegate void RoundEvent(Round target);
	public event RoundEvent roundStart;
	//public event RoundEvent roundPaused;
	//public event RoundEvent roundUnpaused;
	//public event RoundEvent roundFailed;
	public event RoundEvent roundComplete;
	
	public string name;
	public ISpawner spawner;
	public Quaternion spawnDirection;
	
	public List<Wave> waves;
	public List<Wave> liveWaves;
	public List<Enemy> liveEnemies;
	
	public float startTime;
	public float stopTime;
	public RoundState state;
	
	// ---- getters & setters ----
	
	private int _currentWaveIndex = -1;
	private Wave _currentWave;
	
	public int currentWaveIndex {
		get { return _currentWaveIndex; }
		set {
			if (_currentWaveIndex != value) {
				_currentWaveIndex = value;
				
				_currentWave = (value >= 0 && value < waves.Count)
					? waves[value]
					: null;
			}
		}
	}
	
	public Wave currentWave {
		get { return _currentWave; }
		set {
			int index = waves.IndexOf(value);
			if (value != null && index > -1) currentWaveIndex = index;
			else throw new UnityException("value not found in collection");
		}
	}
	
	// ---- constructor ----
	
	public Round(string name) {
		this.name = name;
		
		waves = new List<Wave>();
		liveWaves = new List<Wave>();
		liveEnemies = new List<Enemy>();
		
		state = RoundState.IDLE;
	}

	// ---- public methods ----
	
	public void CreateWave(GameObjectPool pool, int amount, float spawnInterval, float waitTime) {
		Wave wave = new Wave(pool, amount, spawnInterval, waitTime);
		AddWave(wave);
	}
	
	public void AddWave(Wave wave) {
		waves.Add(wave);
	}
	
	public void Start() {
		startTime = Time.time;
		state = RoundState.RUNNING;
		DispatchRoundEvent(roundStart);
	}
	
	public void Stop() {
		stopTime = Time.time;
		state = RoundState.IDLE;
	}
	
	public void Update() {
		if (state == RoundState.RUNNING) {
			
			// trigger first wave
			if (currentWaveIndex < 0) NextWave();
			
			if (Time.time >= currentWave.spawnTime) {
				currentWave.spawnTime = Time.time + currentWave.spawnInterval;
				
				// only start a next wave after all enemies are cleared
				if (!currentWave.hasEnemies || currentWave.timeUp) {
					NextWave();
					
				} else {
					currentWave.Spawn(spawner.GetSpawnPosition(), spawnDirection);
				}
			}
		}
	}
	
	public void NextWave() {
		if (currentWaveIndex + 1 < waves.Count) {
			currentWaveIndex++;
			//Utils.trace("next wave:", currentWaveIndex);
			
			SetWaveHandlers(currentWave, true);
			currentWave.startTime = Time.time;
			liveWaves.Add(currentWave);
			
		} else if (liveWaves.Count == 0) {
			DispatchRoundEvent(roundComplete);
			Stop();
		}
	}
	
	// ---- protected methods ----
	
	protected void SetWaveHandlers(Wave target, bool adding) {
		if (adding) {
			target.waveDepleted += waveDepletedHandler;
			target.waveCleared += waveClearedHandler;
			target.enemySpawned += enemySpawnedHandler;
			target.enemyDestroyed += enemyDestroyedHandler;
			
		} else {
			target.waveDepleted -= waveDepletedHandler;
			target.waveCleared -= waveClearedHandler;
			target.enemySpawned -= enemySpawnedHandler;
			target.enemyDestroyed -= enemyDestroyedHandler;
		}
	}
	
	protected void DispatchRoundEvent(RoundEvent evt) {
		if (evt != null) evt(this);
	}
	
	// ---- event handlers ----
	
	private void waveDepletedHandler(Wave target) {
		//Utils.trace("waveDepleted", waves.IndexOf(target));
	}
	
	private void waveClearedHandler(Wave target) {
		//Utils.trace("waveCleared", waves.IndexOf(target));
		liveWaves.Remove(target);
		SetWaveHandlers(target, false);
		
		if (liveWaves.Count == 0) DispatchRoundEvent(roundComplete);
	}
	
	private void enemySpawnedHandler(Wave target, Enemy enemy) {
		liveEnemies.Add(enemy);
	}
	
	private void enemyDestroyedHandler(Wave target, Enemy enemy) {
		liveEnemies.Remove(enemy);
	}
	
}

public enum RoundState {
	IDLE, RUNNING, PAUSED
}