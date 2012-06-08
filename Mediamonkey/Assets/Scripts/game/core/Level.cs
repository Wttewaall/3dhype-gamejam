using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Core/Level")]

public class Level : MonoBehaviour, IPlayable {
	
	// events
	public event PlayableEventHandler OnPlaying;
	public event PlayableEventHandler OnPaused;
	public event PlayableEventHandler OnResumed;
	public event PlayableEventHandler OnStopped;
	public event PlayableEventHandler OnReset;
	public event PlayableEventHandler OnComplete;
	
	public event LevelEventHandler OnLevelLoaded;
	public event LevelEventHandler OnLevelFailed;
	public event LevelEventHandler OnLevelComplete;
	
	// public members
	public LevelSettings settings;
	public List<Round> rounds;
	
	protected PlayableCollection dataProvider;
	protected Round currentRound;
	
	// ---- getters & setters ----
	
	private bool _isPlaying;
	
	public virtual bool isPlaying {
		get { return _isPlaying; }
		protected set { _isPlaying = value; }
	}
	
	private bool _isPaused;
	
	public virtual bool isPaused {
		get { return _isPaused; }
		protected set { _isPaused = value; }
	}
	
	// ---- inherited handlers ----
	
	void Start() {
		DispatchEvent(OnLevelLoaded);
		
		// add levels to our dataprovider
		dataProvider = new PlayableCollection(rounds);
		Utils.trace(this, "- filled rounds:", dataProvider);
		
		dataProvider.OnIndexChange += delegate {
			SetEventHandlers(currentRound, false);
			currentRound = dataProvider.selectedItem as Round;
			SetEventHandlers(currentRound, true);
		};
		
		// initialize all rounds
		foreach (Round r in rounds) r.Initialize();
	}
	
	// ---- public methods ----
	
	public virtual void Setup() {
		// programmatically overrides all manual settings in the scene
	}
	
	public bool Play() {
		if (dataProvider.length == 0) {
			throw new UnityException("no rounds to play");
			
		} else {
			if (dataProvider.selectedIndex < 0) {
				dataProvider.selectedIndex = 0;
			}
			
			Utils.trace(this, "- Play", currentRound);
			currentRound.Play();
			DispatchPlayableEvent(OnPlaying);
		}
		return true;
	}
	
	public bool Pause() {
		Utils.trace(this, "- Pause");
		DispatchPlayableEvent(OnPaused);
		currentRound.Pause();
		return true;
	}
	
	public bool Resume() {
		Utils.trace(this, "- Resume");
		currentRound.Resume();
		DispatchPlayableEvent(OnResumed);
		return true;
	}
	
	public bool Stop() {
		Utils.trace(this, "- Stop");
		currentRound.Stop();
		DispatchPlayableEvent(OnStopped);
		return true;
	}
	
	public void Reset() {
		Utils.trace(this, "- Reset");
		DispatchPlayableEvent(OnReset);
	}
	
	public ScoreDetails CalculateScore() {
		// TODO return Score objects with detailed statistical information
		var score = new ScoreDetails();
		return score;
	}
	
	public void DispatchEvent(LevelEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	protected void DispatchPlayableEvent(PlayableEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	// ---- protected methods ----
	
	protected void SetEventHandlers(Round target, bool adding) {
		if (target == null) return;
		
		if (adding) {
			target.OnRoundStart += roundStartHandler;
			target.OnRoundComplete += roundCompleteHandler;
			target.OnRoundFailed += roundFailedHandler;
			
		} else {
			target.OnRoundStart -= roundStartHandler;
			target.OnRoundComplete -= roundCompleteHandler;
			target.OnRoundFailed -= roundFailedHandler;
		}
	}
	
	// ---- event handlers ----
	
	private void roundStartHandler(Round target) {
		Utils.trace("\t", target, "- roundStart");
	}
	
	private void roundCompleteHandler(Round target) {
		Utils.trace("\t", target, "- roundComplete");
		SetEventHandlers(target, false);
		
		if (dataProvider.hasNext) {
			dataProvider.Next().Play();
			
		} else {
			DispatchEvent(OnLevelComplete);
		}
	}
	
	private void roundFailedHandler(Round target) {
		Utils.trace("\t", target, "- roundFailed");
	}
	
	// ---- delegates ----
	
	public delegate void LevelEventHandler(Level target);
	
}

[Serializable]
public class LevelSettings {
	
	public int parTime = 12000;
	// add many more settings
	
}