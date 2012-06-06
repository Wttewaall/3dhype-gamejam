using UnityEngine;
using System.Collections.Generic;

public class PoolManager {
	
	protected Dictionary<GameObject, GameObjectPool> pools;
	
	public PoolManager() {
		pools = new Dictionary<GameObject, GameObjectPool>();
	}
	
	public GameObjectPool AddPool(GameObjectPool pool) {
		return pools[pool.prefab] = pool;
	}
	
	public GameObjectPool GetPoolByGameObject(GameObject go) {
		return pools[go];
	}
	
	public GameObjectPool GetPoolByEnemyType(EnemyType type) {
		var enumerator = pools.GetEnumerator();
		
		while (enumerator.MoveNext()) {
			if (enumerator.Current.Key.GetComponent<Enemy>().type == type)
				return enumerator.Current.Value;
		}
		return null;
	}
	
}
