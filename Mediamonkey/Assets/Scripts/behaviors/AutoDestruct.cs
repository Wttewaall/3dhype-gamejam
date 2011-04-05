using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {
	
	public float timeOut = 1.0f;
	public bool detachChildren = false;
	public GameObjectPool pool;
	
	void OnEnable () {
		Invoke("DestroyNow", timeOut);
	}
	
	public void DestroyNow () {
		if (detachChildren) transform.DetachChildren ();
		
		// destroy the gameobject through the pool, if available
		if (pool != null) pool.destroy(gameObject);
		else DestroyObject(gameObject);
	}
}
