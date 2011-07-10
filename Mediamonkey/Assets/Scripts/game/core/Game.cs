using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Core/Game")]

public class Game : MonoBehaviour {
	
	public float timeScale = 1;
	public GameObject[] enemies;
	
	protected PlayState playState = PlayState.STOPPED;
	protected GameState gameState = GameState.MENU;
	
	protected Dictionary<GameObject, GameObjectPool> pools;
	protected List<Round> rounds = new List<Round>();
	
	protected SpawnBox spawnBox;
	protected Quaternion spawnDirection;
	protected int frame = 0;
	protected int interval = 5;
	
	// ---- getters & setters ----
	
	private static Game _instance;
	private int _currentRoundIndex = -1;
	private Round _currentRound;
	
	public static Game instance {
		get { return _instance; }
	}
	
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
	
	// ---- inherited handlers ----
	
	void Awake() {
		_instance = this;
		
		spawnBox = GameObject.Find("Spawnbox").GetComponent<SpawnBox>();
		spawnDirection = Quaternion.LookRotation(Vector3.back);
		
		FillPools();
		CreateRounds();
	}
	
	void Start() {
		Invoke("NextRound", 1);
	}
	
	void Update() {
		if (++frame % interval > 1) return;
		else frame = 0;
		
		if (playState == PlayState.PLAYING) {
			currentRound.Update();
		}
	}
	
	// ---- public methods ----
	
	public void NextRound() {
		currentRoundIndex = (currentRoundIndex + 1 < rounds.Count)
			? currentRoundIndex + 1
			: -1;
		
		if (currentRound != null) {
			gameState = GameState.LEVEL_START;
			playState = PlayState.PLAYING;
			currentRound.Start();
			
		} else {
			gameState = GameState.GAMEOVER;
			playState = PlayState.STOPPED;
		}
	}
	
	// ---- protected methods ----
	
	protected void FillPools() {
		// create pools per enemy prefab and store in a dictionary with the prefab as key
		pools = new Dictionary<GameObject, GameObjectPool>();
		
		foreach (GameObject enemy in enemies) {
			pools.Add(enemy, new GameObjectPool(enemy, 5, null, true));
		}
	}
	
	protected void CreateRounds() {
		// create a round with multiple waves of enemies
		Round round = new Round("Round 1");
		round.spawner = spawnBox;
		round.spawnDirection = spawnDirection;
		
		SetRoundHandlers(round, true);
		
		// play round & handle states
		round.CreateWave(pools[enemies[0]], 4, 0.5f, 0);
		round.CreateWave(pools[enemies[1]], 3, 0.5f, 0);
		round.CreateWave(pools[enemies[2]], 4, 0.5f, 0);
		round.CreateWave(pools[enemies[3]], 3, 0.5f, 0);
		
		rounds.Add(round);
	}
	
	protected void SetRoundHandlers(Round target, bool adding) {
		if (adding) {
			target.roundStart += roundStartHandler;
			target.roundComplete += roundCompleteHandler;
			
		} else {
			target.roundStart -= roundStartHandler;
			target.roundComplete -= roundCompleteHandler;
		}
	}
	
	// ---- event handlers ----
	
	private void roundStartHandler(Round target) {
		//Utils.trace("roundStart", target);
	}
	
	private void roundCompleteHandler(Round target) {
		//Utils.trace("roundComplete", target);
		SetRoundHandlers(target, false);
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