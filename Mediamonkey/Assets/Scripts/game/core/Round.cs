using UnityEngine;
using System.Collections;
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
	
	public void CreateWave(GameObjectPool pool, int amount) {
		Wave wave = new Wave(pool, amount);
		AddWave(wave);
	}
	
	public void CreateWave(GameObjectPool pool, int amount, float spawnInterval) {
		Wave wave = new Wave(pool, amount, spawnInterval);
		AddWave(wave);
	}
	
	public void AddWave(Wave wave) {
		waves.Add(wave);
	}
	
	public void Start() {
		startTime = Time.time;
		state = RoundState.RUNNING;
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
				if (!currentWave.hasEnemies) {
					NextWave();
					
				} else {
					currentWave.spawn(spawner.GetSpawnPosition(), spawnDirection);
				}
			}
			
			// update all live waves
			//foreach (Wave w in liveWaves) w.Update();
		}
	}
	
	public void NextWave() {
		if (currentWaveIndex + 1 < waves.Count) {
			currentWaveIndex++;
			Utils.trace("next wave:", currentWaveIndex);
			
			currentWave.spawnTime = Time.time; // immediately
			SetWaveHandlers(currentWave, true);
			liveWaves.Add(currentWave);
			
		} else {
			Utils.trace("all waves complete");
			Stop();
			dispatchRoundEvent(roundComplete);
		}
	}
	
	/*public void update() {
		
		if (state == RoundState.RUNNING) {
			
			// trigger first wave
			if (currentWaveIndex < 0) nextWave();
			
			if (Time.time >= currentWave.spawnTime) {
				currentWave.spawnTime = Time.time + currentWave.spawnInterval;
				
				if (currentWave.numEnemies == 0) {
					nextWave();
					
				} else {
					Utils.trace("spawn @", currentWave.spawnTime, "("+(currentWave.spawnTime - Time.time)+")");
					
					var enemy:Enemy = currentWave.spawn();
					enemy.x = Math.random() * Game.width;
					enemy.y = -20;
					liveEnemies.push(enemy);
					PlayScreen(Game.instance.screen).addChild( enemy );
				}
			}
			
			//foreach (Enemy e in liveEnemies) e.update();
		}
	}*/
	
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
	
	protected void dispatchRoundEvent(RoundEvent evt) {
		if (evt != null) evt(this);
	}
	
	// ---- event handlers ----
	
	private void waveDepletedHandler(Wave target) {
		Utils.trace("waveDepleted", target);
	}
	
	private void waveClearedHandler(Wave target) {
		Utils.trace("waveCleared", target);
		liveWaves.Remove(target);
		SetWaveHandlers(target, false);
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