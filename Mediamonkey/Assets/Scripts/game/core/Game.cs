using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	
	private static Game _instance;
	
	public static Game instance {
		get { return _instance; }
	}
	
	public static GameObjectPool poolManager;
	
	public float timeScale = 1;
	
	protected GameState gameState;
	
	protected GameObjectPool pool1;
	protected GameObjectPool pool2;
	protected GameObjectPool pool3;
	
	protected List<Round> rounds;
	
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
			int index = rounds.IndexOf(value);
			
			// set currentRound through setting currentRoundIndex
			if (value != null && index > -1) currentRoundIndex = index;
			else throw new UnityException("value not found in collection");
		}
	}
	
	// ---- inherited handlers ----
	
	void Awake() {
		_instance = this;
		
		//poolManager = new ObjectPoolManager();
		fillPools();
		
		rounds = new List<Round>();
		//rounds.Add( createRound("Round 1", [Enemy1Clip, Enemy2Clip]) );
		//rounds.Add( createRound("Round 2", [Enemy2Clip, Enemy3Clip]) );
		//rounds.Add( createRound("Round 3", [Enemy1Clip, Enemy3Clip]) );
		
		setGameState(GameState.MENU);
		//addEventListener(Event.ENTER_FRAME, enterFrameHandler);
	}
	
	protected void fillPools() {
		/*pool1 = new ObjectPool(Enemy1Clip, 5, true);
		pool2 = new ObjectPool(Enemy2Clip, 5, true);
		pool3 = new ObjectPool(Enemy3Clip, 5, true);
		
		poolManager.register(pool1);
		poolManager.register(pool2);
		poolManager.register(pool3);*/
	}
	
	protected Round createRound(string name, object[] types) {
		Round round = new Round(name);
		
		/*for (int i=0; i<types.Length; i++) {
			round.waves.Add( new Wave(types[i], 2) );
		}*/
		
		return round;
	}
	
	public void nextRound() {
		Utils.trace("currentRoundIndex:", currentRoundIndex);
		currentRoundIndex = (currentRoundIndex + 1 < rounds.Count) ? currentRoundIndex + 1 : -1;
		Utils.trace("next currentRoundIndex:", currentRoundIndex);
		
		if (currentRound != null) {
			//var playScreen:PlayScreen = screen as PlayScreen;
			//if (!playScreen) throw new Error("cannot play next round: you're not in a game");
			
			//playScreen.reset();
			//playScreen.round = currentRound;
			//playScreen.start();
			
		} else {
			setGameState(GameState.GAMEOVER);
		}
	}
	
	public void setGameState(GameState state) {
		if (gameState == state) return;
		
		/*if (gameState != null) {
			screen.destroy();
			removeChild(screen);
		}*/
		
		gameState = state;
		
		/*switch (gameState) {
			case GameState.MENU: {
				screen = addChild(new MenuScreenClip()) as MenuScreen;
				break;
			}
			case GameState.LEVEL_START: {
				var playScreen:PlayScreen;
				screen = playScreen = addChild(new PlayScreenClip()) as PlayScreen;
				playScreen.start();
				break;
			}
			case GameState.LEVEL_PAUSE: {
				playScreen = screen as PlayScreen;
				if (!playScreen) throw new Error("cannot switch to this state");
				playScreen.pause();
				break;
			}
			case GameState.LEVEL_WIN: {
				playScreen = screen as PlayScreen;
				if (!playScreen) throw new Error("cannot switch to this state");
				playScreen.win();
				break;
			}
			case GameState.LEVEL_LOSE: {
				playScreen = screen as PlayScreen;
				if (!playScreen) throw new Error("cannot switch to this state");
				playScreen.lose();
				break;
			}
			case GameState.GAMEOVER: {
				screen = addChild(new ScoreScreenClip()) as ScoreScreen;
				break;
			}
		}*/
	}
	
	/*protected void enterFrameHandler(event:Event) {
		if (screen) screen.update();
		else throw new Error("nothing to update");
	}*/
	
}

public enum GameState {
	MENU,
	LEVEL_START,
	LEVEL_PAUSE,
	LEVEL_WIN,
	LEVEL_LOSE,
	GAMEOVER
}