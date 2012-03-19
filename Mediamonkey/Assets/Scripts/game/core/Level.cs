using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Core/Level")]

public class Level : MonoBehaviour {
	
	// events
	public event LevelEventHandler OnLoaded;
	public event LevelEventHandler OnStart;
	public event LevelEventHandler OnPaused;
	public event LevelEventHandler OnUnpaused;
	public event LevelEventHandler OnFailed;
	public event LevelEventHandler OnComplete;
	
	// members
	public LevelSettings settings = new LevelSettings();
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
		
		if (OnLoaded != null) OnLoaded(this);
	}
	
	void Update() {
		if (++frame % interval > 1) return;
		else frame = 0;
		
		switch (playState) {
			
			case PlayState.PLAYING:
				dataProvider.selectedItem.Update();
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
			playState = PlayState.PLAYING;
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
	
	// ---- protected methods ----
	
	protected void SetEventHandlers(Round target, bool adding) {
		if (target == null) return;
		
		if (adding) {
			target.OnStart += roundStartHandler;
			target.OnComplete += roundCompleteHandler;
			
		} else {
			target.OnStart -= roundStartHandler;
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
	
	private void roundCompleteHandler(Round target) {
		Utils.trace(target, "roundComplete");
		SetEventHandlers(target, false);
	}
	
	// ---- delegates ----
	
	public delegate void LevelEventHandler(Level target);
	
}

[Serializable]
public class LevelSettings {
	
	public int parTime = 12000;
	// add many more settings
	
}