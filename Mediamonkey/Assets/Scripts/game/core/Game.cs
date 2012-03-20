using UnityEngine;
using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Core/Game")]

public class Game : MonoBehaviour {
	
	// visible public members
	public List<Level> levels;
	
	// invisible public members
	public DataProvider<Level> dataProvider;
	public PoolManager poolManager = new PoolManager();
	
	// settings
	protected int frame = 0;
	protected int interval = 5;
	
	// states
	private GameState gameState = GameState.MENU;
	
	// depricated
	protected List<Spawner> spawners;
	protected List<Transform> goals;
	
	// ---- getters & setters ----
	
	private static Game _instance;
	
	public static Game instance {
		get { return _instance; }
	}
	
	// ---- inherited handlers ----
	
	void Awake() {
		_instance = this;
	}
	
	void Start() {
		// add levels to our dataprovider
		dataProvider = new DataProvider<Level>(levels);
		dataProvider.OnIndexChange += indexChangeHandler;
		
		if (dataProvider.length == 0) {
			var level = CreateLevel();
			dataProvider.AddItem(level);
		}
	}
	
	void OnGUI() {
		if (++frame % interval > 1) return;
		else frame = 0;
		
		// handle gamestates and UI
		if (gameState == GameState.MENU) {
			if (GUI.Button(new Rect(0, 0, 150, 30), "Start")) {
				// setting the index to 0 will trigger te startup
				dataProvider.selectedIndex = 0;
				gameState = GameState.LEVEL_START;
			}
		}
	}
	
	// ---- public methods ----
	
	public Level CreateLevel() {
	
		var spawner = GameObject.Find("Spawnbox").GetComponent<SpawnBox>();
		spawners.Add(spawner);
		
		var level = new Level();
		SetEventHandlers(level, true);
		level.spawners = spawners;
		level.goals = goals;
		
		var round1 = level.CreateRound();
		
		var wave11 = round1.CreateWave(spawners[0], goals[0]);
		wave11.CreateGroup(EnemyType.RUNNER, 4, 0);
		wave11.CreateGroup(EnemyType.ORC, 4, 0);
		wave11.CreateGroup(EnemyType.ARCHER, 4, 0);
		
		var wave12 = round1.CreateWave(spawners[1], goals[1]);
		wave12.CreateGroup(EnemyType.ORC, 4, 0);
		wave12.CreateGroup(EnemyType.ARCHER, 4, 0);
		wave12.CreateGroup(EnemyType.TROLL, 4, 0);
		wave12.CreateGroup(EnemyType.OGRE, 4, 0);
		
		var round2 = level.CreateRound();
		
		var wave21 = round2.CreateWave(spawners[0], goals[0]);
		wave21.CreateGroup(EnemyType.OGRE, 4, 0);
		
		return level;
	}
	
	// ---- protected methods ----
	
	protected void SetEventHandlers(Level target, bool adding) {
		if (target == null) return;
		
		if (adding) {
			target.OnLoaded += levelLoadedHandler;
			target.OnPlay += levelStartHandler;
			target.OnPaused += levelPausedHandler;
			target.OnUnpaused += levelPausedHandler;
			target.OnFailed += levelFailedHandler;
			target.OnComplete += levelCompleteHandler;
			
		} else {
			target.OnLoaded -= levelLoadedHandler;
			target.OnPlay -= levelStartHandler;
			target.OnPaused -= levelPausedHandler;
			target.OnUnpaused -= levelPausedHandler;
			target.OnFailed -= levelFailedHandler;
			target.OnComplete -= levelCompleteHandler;
		}
	}
	
	// ---- event handlers ----
	
	private void indexChangeHandler(IndexChangeEventType type, DataProvider<Level> currentTarget, int oldIndex, int newIndex) {
		if (currentTarget.selectedItem == null) {
			Debug.LogWarning("Cannot start: selected level is null");
			
		} else {
			SetEventHandlers(currentTarget.previousItem, false);
			Debug.Log("Starting level: "+currentTarget.selectedItem);
			SetEventHandlers(currentTarget.selectedItem, true);
		}
	}
	
	private void levelLoadedHandler(Level target) {
		target.Play();
	}
	
	private void levelStartHandler(Level target) {
		Utils.trace(target, "levelStart");
	}
	
	private void levelPausedHandler(Level target) {
		Utils.trace(target, "levelPaused");
	}
	
	private void levelFailedHandler(Level target) {
		Utils.trace(target, "levelFailed");
	}
	
	private void levelCompleteHandler(Level target) {
		Utils.trace(target, "levelComplete");
		SetEventHandlers(target, false);
	}
	
}