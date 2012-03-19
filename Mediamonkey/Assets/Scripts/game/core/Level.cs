using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LevelSettings {
	
	public int parTime = 12000;
	// add many more settings
	
}

public class Level : MonoBehaviour {
	
	/**
	 * TODO
	 * . pool mangement (losse manager?)
	 * 
	 */
	
	// events
	public event LevelEventHandler OnStart;
	//public event LevelEventHandler OnPaused;
	//public event LevelEventHandler OnUnpaused;
	//public event LevelEventHandler OnFailed;
	public event LevelEventHandler OnComplete;
	
	public delegate void LevelEventHandler(Level target);
	
	public LevelSettings levelSettings = new LevelSettings();
	public List<Spawner> spawners;
	public List<Transform> goals;
	public List<Round> rounds;
	
	// ---- getters & setters ----
	
	private int _currentRoundIndex = -1;
	private Round _currentRound;
	
	public int currentRoundIndex {
		get { return _currentRoundIndex; }
		set {
			if (_currentRoundIndex != value) {
				_currentRoundIndex = value;
				
				_currentRound = (value >= 0 && value < rounds.Count)
					? rounds[value]
					: null;
			}
		}
	}
	
	public Round currentRound {
		get { return _currentRound; }
		set {
			if (value == currentRound) return;
			
			if (value == null) {
				currentRoundIndex = -1;
				return;
			}
			
			int index = rounds.IndexOf(value);
			
			if (index > -1) {
				currentRoundIndex = index;
				
			} else {
				rounds.Add(value);
				currentRoundIndex = rounds.Count-1;
			}
		}
	}
	
	// ---- public methods ----
	
	public virtual void Setup() {
		// programmatically overrides all manual settings in the scene
	}
	
	public Round CreateRound() {
		var round = new Round();
		SetRoundHandlers(round, true);
		
		if (rounds == null) rounds = new List<Round>();
		rounds.Add(round);
		
		round.index = rounds.Count - 1;
		return round;
	}
	
	public void NextRound() {
		currentRoundIndex = (currentRoundIndex + 1 < rounds.Count)
			? currentRoundIndex + 1
			: -1;
		
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
	
	protected void SetRoundHandlers(Round target, bool adding) {
		if (adding) {
			target.OnStart += roundStartHandler;
			target.OnComplete += roundCompleteHandler;
			
		} else {
			target.OnStart -= roundStartHandler;
			target.OnComplete -= roundCompleteHandler;
		}
	}
	
	// ---- event handlers ----
	
	private void roundStartHandler(Round target) {
		Utils.trace(target, "roundStart");
	}
	
	private void roundCompleteHandler(Round target) {
		Utils.trace(target, "roundComplete");
		SetRoundHandlers(target, false);
	}
	
}
