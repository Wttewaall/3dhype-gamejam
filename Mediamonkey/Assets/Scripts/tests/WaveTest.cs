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
	
	protected Wave currentWave;
	
	void Start() {
		currentWave = new Wave(pools[enemies[0]], 4, 1);
		setWaveHandlers(currentWave, true);
		
		InvokeRepeating("Spawn", 1, currentWave.spawnInterval);
		
		// create a round with multiple waves of enemies
		// play round & handle states
	}
	
	// ---- public methods ----
	
	public void Spawn() {
		currentWave.spawn(spawnBox.GetSpawnPosition(), spawnDirection);
	}
	
	// ---- protected methods ----
	
	protected void setWaveHandlers(Wave target, bool adding) {
		if (adding) {
			target.waveDepleted += waveDepletedHandler;
			target.waveCleared += waveClearedHandler;
			
		} else {
			target.waveDepleted -= waveDepletedHandler;
			target.waveCleared -= waveClearedHandler;
		}
	}
	
	// ---- event handlers ----
	
	private void waveDepletedHandler(Wave target) {
		Utils.trace("waveDepleted", target);
		CancelInvoke("Spawn");
	}
	
	private void waveClearedHandler(Wave target) {
		Utils.trace("waveCleared", target);
		setWaveHandlers(target, false);
	}
	
}