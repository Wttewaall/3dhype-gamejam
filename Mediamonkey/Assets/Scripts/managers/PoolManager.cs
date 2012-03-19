using UnityEngine;
using System.Collections.Generic;

public class PoolManager {
	
	protected Dictionary<GameObject, GameObjectPool> pools;
	
	public PoolManager() {
		pools = new Dictionary<GameObject, GameObjectPool>();
	}
	
	public GameObjectPool GetPoolByGameObject(GameObject go) {
		return pools[go];
	}
	
}
