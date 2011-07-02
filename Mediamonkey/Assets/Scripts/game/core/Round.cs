using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A Round consists of multiple waves
 * The round handles wave initializing, timing and scoring
 */

public class Round {
	
	public string name;
	public List<Wave> waves;
	public float startTime;
	public float stopTime;
	public RoundState state;
	
	protected List<Enemy> liveEnemies;
	
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
		liveEnemies = new List<Enemy>();
		state = RoundState.IDLE;
	}
	
	// ---- public methods ----
	
	public void start() {
		Utils.trace(name, "start");
		startTime = Time.time;
		state = RoundState.RUNNING;
	}
	
	public void pause() {
		Utils.trace(name, "pause");
		stopTime = Time.time;
		state = RoundState.PAUSED;
	}
	
	public void stop() {
		Utils.trace(name, "stop");
		stopTime = Time.time;
		state = RoundState.IDLE;
	}
	
	public void restart() {
		Utils.trace(name, "restart");
		stop();
		reset();
		start();
	}
	
	public void reset() {
		//..
	}
	
	public void nextWave() {
		if (currentWaveIndex + 1 < waves.Count) {
			currentWaveIndex++;
			Utils.trace("next wave:", currentWaveIndex);
			currentWave.spawnTime = Time.time; // immediately
			//currentWave.spawnTime = Time.time + currentWave.spawnInterval; // after interval
			
		} else {
			Utils.trace("all waves complete");
			stop();
			Game.instance.nextRound();
		}
	}
	
	public void update() {
		
		if (state == RoundState.RUNNING) {
			
			// trigger first wave
			if (currentWaveIndex < 0) nextWave();
			
			if (Time.time >= currentWave.spawnTime) {
				currentWave.spawnTime = Time.time + currentWave.spawnInterval;
				
				/*if (currentWave.numEnemies == 0) {
					nextWave();
					
				} else {
					Utils.trace("spawn @", currentWave.spawnTime, "("+(currentWave.spawnTime - Time.time)+")");
					
					var enemy:Enemy = currentWave.spawn();
					enemy.x = Math.random() * Game.width;
					enemy.y = -20;
					liveEnemies.push(enemy);
					PlayScreen(Game.instance.screen).addChild( enemy );
				}*/
			}
			
			//foreach (Enemy e in liveEnemies) e.update();
		}
	}
	
}

public enum RoundState {
	IDLE, RUNNING, PAUSED
}