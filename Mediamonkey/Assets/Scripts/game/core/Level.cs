using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Core/Level")]

public class Level : MonoBehaviour {
	
	// events
	public event LevelEventHandler OnLoaded;
	public event LevelEventHandler OnPlay;
	public event LevelEventHandler OnPause;
	public event LevelEventHandler OnStop;
	public event LevelEventHandler OnFailed;
	public event LevelEventHandler OnComplete;
	public event LevelEventHandler OnStateChange;
	
	// members
	public LevelSettings settings;
	public List<Spawner> spawners;
	public List<Transform> goals;
	public List<GameObject> enemies;
	
	public List<Round> rounds;
	public DataProvider<Round> dataProvider;
	public Round currentRound;
	
	// settings
	protected int frame = 0;
	protected int interval = 5;
	
	// ---- getters & setters ----
	
	private PlayState _state = PlayState.STOPPED;
	
	public PlayState state {
		get { return _state; }
		set {
			if (_state == value) return;
			_state = value;
			DispatchEvent(OnStateChange);
		}
	}
	
	// ---- inherited handlers ----
	
	void Start() {
		DispatchEvent(OnLoaded);
		
		// add levels to our dataprovider
		dataProvider = new DataProvider<Round>(rounds);
		dataProvider.OnIndexChange += indexChangeHandler;
		
		// initialize all rounds
		foreach (Round r in rounds) r.Initialize();
	}
	
	void Update() {
		if (++frame % interval > 1) return;
		else frame = 0;
		
		switch (state) {
			
			case PlayState.PLAYING:
				// poll rounds or use events?
				break;
			
			case PlayState.PAUSED:
				//currentRound.Pause();
				break;
			
			case PlayState.STOPPED:
				//Next();
				break;
				
		}
	}
	
	// ---- public methods ----
	
	public virtual void Setup() {
		// programmatically overrides all manual settings in the scene
	}
	
	public Round CreateRound() {
		var round = new Round();
		SetEventHandlers(round, true);
		
		if (rounds == null) rounds = new List<Round>();
		rounds.Add(round);
		
		round.index = rounds.Count - 1;
		return round;
	}
	
	public void Play() {
		if (dataProvider.length == 0) {
			throw new UnityException("no rounds to play");
		
		} else {
			Utils.trace(this, "Play");
			
			dataProvider.selectedIndex++;
			dataProvider.selectedItem.Play();
			state = PlayState.PLAYING;
			DispatchEvent(OnPlay);
		}
	}
	
	public void Pause() {
		Utils.trace(this, "Pause");
		dataProvider.selectedItem.Pause();
		state = PlayState.PAUSED;
		DispatchEvent(OnPause);
	}
	
	public void Stop() {
		Utils.trace(this, "Stop");
		dataProvider.selectedItem.Stop();
		state = PlayState.STOPPED;
		DispatchEvent(OnStop);
	}
	
	public void Next() {
		Utils.trace(this, "Next");
		
		//dataProvider.selectedItem.Destroy();
		dataProvider.Next();
		
		/*if (currentRound != null) {
			gameState = GameState.LEVEL_START;
			state = PlayState.PLAYING;
			currentRound.Start();
			
		} else {
			gameState = GameState.GAMEOVER;
			state = PlayState.STOPPED;
		}*/
	}
	
	public ScoreDetails CalculateScore() {
		// TODO return Score objects with detailed statistical information
		var score = new ScoreDetails();
		return score;
	}
	
	public void DispatchEvent(LevelEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	// ---- protected methods ----
	
	protected void SetEventHandlers(Round target, bool adding) {
		if (target == null) return;
		
		if (adding) {
			target.OnStart += roundStartHandler;
			target.OnComplete += roundCompleteHandler;
			target.OnFailed += roundFailedHandler;
			
		} else {
			target.OnStart -= roundStartHandler;
			target.OnComplete -= roundCompleteHandler;
			target.OnFailed -= roundFailedHandler;
		}
	}
	
	// ---- event handlers ----
	
	private void indexChangeHandler(IndexChangeEventType type, DataProvider<Round> currentTarget, int oldIndex, int newIndex) {
		currentRound = currentTarget.selectedItem;
		
		if (currentRound == null) {
			Debug.LogWarning("Cannot start: selected round is null");
			
		} else {
			SetEventHandlers(dataProvider.previousItem, false);
			Debug.Log("Starting round: "+currentRound);
			SetEventHandlers(currentTarget.selectedItem, true);
		}
	}
	
	private void roundStartHandler(Round target) {
		Utils.trace(target, "roundStart");
	}
	
	private void roundCompleteHandler(Round target) {
		Utils.trace(target, "roundComplete");
		SetEventHandlers(target, false);
		
		if (dataProvider.nextItem != null) {
			dataProvider.Next();
			
		} else {
			DispatchEvent(OnComplete);
		}
	}
	
	private void roundFailedHandler(Round target) {
		Utils.trace(target, "roundFailed");
	}
	
	// ---- delegates ----
	
	public delegate void LevelEventHandler(Level target);
	
}

[Serializable]
public class LevelSettings {
	
	public int parTime = 12000;
	// add many more settings
	
}