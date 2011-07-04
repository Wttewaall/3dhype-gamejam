using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveTest : MonoBehaviour {
	
	public GameObject[] enemies;
	
	protected SpawnBox spawnBox;
	protected Quaternion spawnDirection;
	protected Dictionary<GameObject, GameObjectPool> pools;
	
	void Awake() {
		spawnBox = GameObject.Find("Spawnbox").GetComponent<SpawnBox>();
		spawnDirection = Quaternion.LookRotation(Vector3.back);
		
		// create pools per enemy prefab and store in a dictionary with the prefab as key
		pools = new Dictionary<GameObject, GameObjectPool>();
		foreach (GameObject go in enemies) {
			pools.Add(go, new GameObjectPool(go, 5, null, true));
		}
	}
	
	protected Round currentRound;
	
	void Start() {
		
		// create a round with multiple waves of enemies
		currentRound = new Round("Round 1");
		currentRound.spawner = spawnBox;
		currentRound.spawnDirection = spawnDirection;
		
		SetRoundHandlers(currentRound, true);
		
		// play round & handle states
		currentRound.CreateWave(pools[enemies[0]], 4, 0.5f, 0);
		currentRound.CreateWave(pools[enemies[1]], 3, 0.5f, 0);
		currentRound.CreateWave(pools[enemies[0]], 2, 0.5f, 0);
		currentRound.CreateWave(pools[enemies[1]], 1, 0.5f, 0);
		
		Invoke("StartRound", 1);
	}
	
	protected int frame = 0;
	protected int interval = 5;
	
	void Update() {
		if (++frame % interval > 0) return;
		else frame = 0;
		
		currentRound.Update();
	}
	
	public void StartRound() {
		currentRound.Start();
	}
	
	// ---- protected methods ----
	
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
		Utils.trace("roundStart", target);
	}
	
	private void roundCompleteHandler(Round target) {
		Utils.trace("roundComplete", target);
		SetRoundHandlers(target, false);
	}
	
}