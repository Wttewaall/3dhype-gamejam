using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Core/Game")]

public class Game : MonoBehaviour {
	
	// visible public members
	public List<Level> levels;
	public Level currentLevel;
	
	// invisible public members
	public DataProvider<Level> dataProvider;
	public PoolManager poolManager = new PoolManager();
	
	// settings
	protected int frame = 0;
	protected int interval = 5;
	
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
		
		dataProvider.OnIndexChange += delegate {
			SetEventHandlers(currentLevel, false);
			currentLevel = dataProvider.selectedItem as Level;
			SetEventHandlers(currentLevel, true);
		};
		
		/*
		// if level is not declared, build one as a test case
		if (dataProvider.length == 0) {
			var level = CreateLevel();
			dataProvider.AddItem(level);
		}
		*/
	}
	
	void OnGUI() {
		if (++frame % interval > 1) return;
		else frame = 0;
		
		if (GUI.Button(new Rect(0, 0, 150, 30), "Start")) {
			// setting the index to 0 will trigger te startup
			dataProvider.selectedIndex = 0;
			currentLevel.Play();
		}
	}
				
	// ---- public methods ----
	
	/*
	public Level CreateLevel() {
		Utils.trace(this, "CreateLevel");
		
		var spawner = GameObject.Find("Spawnbox").GetComponent<SpawnBox>();
		spawners.Add(spawner);
		
		var level = new Level();
		SetEventHandlers(level, true);
		
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
	}*/
	
	// ---- protected methods ----
	
	protected void SetEventHandlers(Level target, bool adding) {
		if (target == null) return;
		
		if (adding) {
			target.OnLevelLoaded += levelLoadedHandler;
			target.OnLevelFailed += levelFailedHandler;
			target.OnLevelComplete += levelCompleteHandler;
			
		} else {
			target.OnLevelLoaded -= levelLoadedHandler;
			target.OnLevelFailed -= levelFailedHandler;
			target.OnLevelComplete -= levelCompleteHandler;
		}
	}
	
	// ---- event handlers ----
	
	private void levelLoadedHandler(Level target) {
		Utils.trace(target, "levelLoaded");
		target.Play();
	}
	
	private void levelFailedHandler(Level target) {
		Utils.trace(target, "levelFailed");
	}
	
	private void levelCompleteHandler(Level target) {
		Utils.trace(target, "levelComplete");
		//SetEventHandlers(target, false);
		target.Reset();
	}
	
}