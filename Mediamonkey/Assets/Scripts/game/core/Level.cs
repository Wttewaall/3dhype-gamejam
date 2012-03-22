using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Core/Level")]

public class Level : MonoBehaviour {
	
	// events
	public event LevelEventHandler OnLoaded;
	public event LevelEventHandler OnPlay;
	public event LevelEventHandler OnFailed;
	public event LevelEventHandler OnComplete;
	
	// members
	public LevelSettings settings;
	public List<Spawner> spawners;
	public List<Transform> goals;
	public List<GameObject> enemies;
	
	public List<Round> rounds;
	public DataProvider<Round> dataProvider;
	
	// settings
	protected int frame = 0;
	protected int interval = 5;
	
	// states
	protected PlayState playState = PlayState.STOPPED;
	
	// ---- inherited handlers ----
	
	void Start() {
		// add levels to our dataprovider
		dataProvider = new DataProvider<Round>(rounds);
		dataProvider.OnIndexChange += indexChangeHandler;
		
		// initialize all rounds
		foreach (Round r in rounds) r.Initialize();
		
		DispatchEvent(OnLoaded);
	}
	
	void Update() {
		if (++frame % interval > 1) return;
		else frame = 0;
		
		switch (playState) {
			
			case PlayState.PLAYING:
				// poll rounds or use events?
				break;
			
			case PlayState.PAUSED:
				//..
				break;
			
			case PlayState.STOPPED:
				//..
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
			dataProvider.selectedIndex++;
			dataProvider.selectedItem.Play();
			playState = PlayState.PLAYING;
			DispatchEvent(OnPlay);
		}
	}
	
	public void Next() {
		//dataProvider.selectedItem.Destroy();
		dataProvider.Next();
		
		/*if (currentRound != null) {
			gameState = GameState.LEVEL_START;
			playState = PlayState.PLAYING;
			currentRound.Start();
			
		} else {
			gameState = GameState.GAMEOVER;
			playState = PlayState.STOPPED;
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
			target.OnFailed += roundFailedHandler;
			target.OnComplete += roundCompleteHandler;
			
		} else {
			target.OnStart -= roundStartHandler;
			target.OnFailed -= roundFailedHandler;
			target.OnComplete -= roundCompleteHandler;
		}
	}
	
	// ---- event handlers ----
	
	private void indexChangeHandler(IndexChangeEventType type, DataProvider<Round> currentTarget, int oldIndex, int newIndex) {
		if (currentTarget.selectedItem == null) {
			Debug.LogWarning("Cannot start: selected round is null");
			
		} else {
			SetEventHandlers(dataProvider.previousItem, false);
			Debug.Log("Starting round: "+currentTarget.selectedItem);
			SetEventHandlers(currentTarget.selectedItem, true);
		}
	}
	
	private void roundStartHandler(Round target) {
		Utils.trace(target, "roundStart");
	}
	
	private void roundFailedHandler(Round target) {
		//..
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
	
	// ---- delegates ----
	
	public delegate void LevelEventHandler(Level target);
	
}

[Serializable]
public class LevelSettings {
	
	public int parTime = 12000;
	// add many more settings
	
}