using UnityEngine;
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
	private PlayState playState = PlayState.STOPPED;
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
		
		// setting the index to 0 will trigger te startup
		dataProvider.selectedIndex = 0;
	}

	void Update() {
		if (++frame % interval > 1) return;
		else frame = 0;
		
		if (playState == PlayState.PLAYING) {
			//dataProvider.selectedItem.Update();
		}
	}
	
	// ---- protected methods ----
	
	/*
	protected Level CreateLevel() {
	
		var spawner = GameObject.Find("Spawnbox").GetComponent<SpawnBox>();
		spawners.Add(spawner);
		
		var level = new Level();
		SetLevelHandlers(level, true);
		level.spawners = spawners;
		level.goals = goals;
		
		var round1 = level.CreateRound();
		
		var wave1 = round1.CreateWave(spawners[0], goals[0]);
		wave1.CreateGroup(EnemyType.RUNNER, 4, 0);
		wave1.CreateGroup(EnemyType.ORC, 4, 0);
		wave1.CreateGroup(EnemyType.ARCHER, 4, 0);
		
		var wave2 = round1.CreateWave(spawners[1], goals[1]);
		wave2.CreateGroup(EnemyType.ORC, 4, 0);
		wave2.CreateGroup(EnemyType.ARCHER, 4, 0);
		wave2.CreateGroup(EnemyType.TROLL, 4, 0);
		wave2.CreateGroup(EnemyType.OGRE, 4, 0);
		
		return level;
	}
	*/
	
	// ---- protected methods ----
	
	protected void SetLevelHandlers(Level target, bool adding) {
		if (adding) {
			target.OnStart += levelStartHandler;
			target.OnComplete += levelCompleteHandler;
			
		} else {
			target.OnStart -= levelStartHandler;
			target.OnComplete -= levelCompleteHandler;
		}
	}
	
	// ---- event handlers ----
	
	private void indexChangeHandler(IndexChangeEventType type, DataProvider<Level> currentTarget, int oldIndex, int newIndex) {
		if (currentTarget.selectedItem == null) {
			Debug.LogWarning("Cannot start: selected level is null");
			
		} else {
			Debug.Log("Starting level: "+currentTarget.selectedItem);
			SetLevelHandlers(currentTarget.selectedItem, true);
			playState = PlayState.PLAYING;
		}
	}
	
	private void levelStartHandler(Level target) {
		Utils.trace(target, "levelStart");
	}
	
	private void levelCompleteHandler(Level target) {
		Utils.trace(target, "levelComplete");
		SetLevelHandlers(target, false);
	}
	
}

public enum PlayState {
	PLAYING,
	PAUSED,
	STOPPED
}

public enum GameState {
	MENU,
	LEVEL_START,
	LEVEL_PAUSE,
	LEVEL_WIN,
	LEVEL_LOSE,
	GAMEOVER
}