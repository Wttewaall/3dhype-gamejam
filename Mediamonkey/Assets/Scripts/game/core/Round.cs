using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

/**
 * A Round consists of multiple waves
 * The round handles wave initializing, timing and scoring
 */

[Serializable]
public class Round {
	
	// events
	public event RoundEventHandler OnStart;
	public event RoundEventHandler OnFailed;
	public event RoundEventHandler OnComplete;
	
	// public members
	public RoundSettings settings;
	public List<Wave> waves;
	public DataProvider<Wave> dataProvider;
	
	[NonSerialized]
	public int index;
	
	protected List<Wave> liveWaves;
	protected List<Enemy> liveEnemies;
	protected RoundState state;
	
	protected Timer timer;
	
	// ---- constructor ----
	
	public Round() {
		state = RoundState.IDLE;
	}
	
	public void Initialize() {
		if (waves == null) waves = new List<Wave>();
		if (liveWaves == null) liveWaves = new List<Wave>();
		if (liveEnemies == null) liveEnemies = new List<Enemy>();
		
		dataProvider = new DataProvider<Wave>(waves);
		dataProvider.OnIndexChange += indexChangeHandler;
		
		timer = new Timer(1000);
		timer.Elapsed += elapsedEventHandler;
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
		
		DispatchEvent(OnStart);
		
		if (dataProvider.length > 0) {
			NextWave();
			
			timer.Start();
			state = RoundState.RUNNING;
			
		} else {
			timer.Stop();
			state = RoundState.IDLE;
			Debug.LogError("No waves to start");
			return;
		}
	}
	
	public void Pause() {
		if (state != RoundState.PAUSED) {
			state = RoundState.PAUSED;
			
		} else {
			state = RoundState.RUNNING;
		}
		
		timer.Enabled = !timer.Enabled;
	}
	
	public void Stop() {
		timer.Stop();
		state = RoundState.IDLE;
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
				liveWaves.Add(dataProvider.selectedItem);
			}
		}
	}
	
	private void elapsedEventHandler (object sender, ElapsedEventArgs e) {
		// check waves and round state
		Utils.trace("timer event");
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