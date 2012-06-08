using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * A Round consists of multiple waves
 * The round handles wave initializing, timing and scoring
 */

[Serializable]
public class Round : PlayableCollection {
	
	// events
	public event RoundEventHandler OnRoundStart;
	public event RoundEventHandler OnRoundFailed;
	public event RoundEventHandler OnRoundComplete;
	
	// public members
	public RoundSettings settings;
	public List<Wave> waves;
	
	[NonSerialized]
	public int index;
	
	protected List<Wave> liveWaves;
	protected List<Enemy> liveEnemies;
	protected Wave currentWave;
	
	// ---- getters & setters ----
	
	// ---- constructor ----
	
	public Round() {
		this.OnIndexChange += delegate {
			SetEventHandlers(currentWave, false);
			currentWave = selectedItem as Wave;
			SetEventHandlers(currentWave, true);
		};
	}
	
	public void Initialize() {
		if (waves == null) waves = new List<Wave>();
		if (liveWaves == null) liveWaves = new List<Wave>();
		if (liveEnemies == null) liveEnemies = new List<Enemy>();
		
		source = waves;
		
		// initialize all rounds
		for (int i = 0; i<waves.Count; i++) {
			waves[i].Initialize();
			waves[i].index = i;
		}
	}
	
	// ---- public methods ----
	
	override public string ToString() {
		return "Round \""+settings.name+"\" ["+OutputToString()+"]";
	}
	
	// ---- protected methods ----
	
	protected void SetEventHandlers(Wave target, bool adding) {
		if (target == null) return;
		
		if (adding) {
			target.OnWaveDepleted += waveDepletedHandler;
			target.OnWaveCleared += waveClearedHandler;
			target.OnEnemySpawned += enemySpawnedHandler;
			target.OnEnemyDestroyed += enemyDestroyedHandler;
			
		} else {
			target.OnWaveDepleted -= waveDepletedHandler;
			target.OnWaveCleared -= waveClearedHandler;
			target.OnEnemySpawned -= enemySpawnedHandler;
			target.OnEnemyDestroyed -= enemyDestroyedHandler;
		}
	}
	
	protected void DispatchEvent(RoundEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	// ---- event handlers ----
	
	private void waveStartedHandler(Wave target) {
		Utils.trace("\t\t", target, "- waveStarted", waves.IndexOf(target));
		liveWaves.Add(target);
	}
	
	private void waveDepletedHandler(Wave target) {
		Utils.trace("\t\t", target, "- waveDepleted", waves.IndexOf(target));
	}
	
	private void waveClearedHandler(Wave target) {
		Utils.trace("\t\t", target, "- waveCleared", waves.IndexOf(target));
		liveWaves.Remove(target);
		SetEventHandlers(target, false);
		
		if (hasNext) Next().Play();
		else if (liveWaves.Count == 0) DispatchEvent(OnRoundComplete);
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