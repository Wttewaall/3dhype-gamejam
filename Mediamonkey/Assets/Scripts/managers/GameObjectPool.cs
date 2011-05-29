using UnityEngine;
using System;
using System.Collections;

/**
 * A general pool object for reusable game objects.
 * 
 * It supports spawning and unspawning game objects that are
 * instantiated from a common prefab. Can be used preallocate
 * objects to avoid calls to Instantiate during gameplay. Can
 * also create objects on demand (which it does if no objects
 * are available in the pool).
 * 
 * Converted JScript version to C#, original here:
 * http://vonlehecreative.com/video-games/unity-resource-gameobjectpool/
 * 
 * C# version by Bart Wttewaall, www.Mediamonkey.nl
 */

public class GameObjectPool {

	private GameObject _prefab;
	private Stack available;
	private ArrayList all;
	
	private Action<GameObject> initAction;
	private bool setActiveRecursively;
	
	// ---- getters & setters ----
	#region getters & setters
	
	// returns the prefab being used by the pool.
	public GameObject prefab {
		get { return _prefab; }
	}
	
	// returns the number of active objects.
	public int numActive {
		get { return all.Count - available.Count; }
	}
	
	// returns the number of available objects.
	public int numAvailable {
		get { return available.Count; }
	}
	
	#endregion
	// ---- constructor ----
	#region constructor
	
	public GameObjectPool(GameObject prefab, uint initialCapacity, Action<GameObject> initAction, bool setActiveRecursively) {
		this._prefab = prefab;
		this.initAction = initAction;
		this.setActiveRecursively = setActiveRecursively;
		
		available =	(initialCapacity > 0)	? new Stack((int) initialCapacity)		: new Stack();
		all =		(initialCapacity > 0)	? new ArrayList((int) initialCapacity)	: new ArrayList();
	}
	
	#endregion
	// ---- public methods ----
	#region public methods
	
	public GameObject spawn(Vector3 position, Quaternion rotation) {
		GameObject result;
		
		if (available.Count == 0){
			
			// create an object and initialize it.
			result = GameObject.Instantiate(prefab, position, rotation) as GameObject;
			
			// run optional initialization method on the object
			if (initAction != null) initAction(result);
			
			all.Add(result);
			
		} else {
			result = available.Pop() as GameObject;
			
			// get the result's transform and reuse for efficiency.
			// calling gameObject.transform is expensive.
			Transform resultTrans = result.transform;
			resultTrans.position = position;
			resultTrans.rotation = rotation;
			
			setActive(result, true);
		}
		
		return result;
	}
	
	public bool destroy(GameObject target) {
		if (!available.Contains(target)) {
			available.Push(target);
			
			setActive(target, false);
			return true;
		}
		
		return false;
	}
	
	// Unspawns all the game objects created by the pool.
	public void destroyAll() {
		for (int i=0; i<all.Count; i++) {
			GameObject target = all[i] as GameObject;
			
			if (target.active) destroy(target);
		}
	}
	
	// Unspawns all the game objects and clears the pool.
	public void clear() {
		destroyAll();
		available.Clear();
		all.Clear();
	}
	
	// Applies the provided function to some or all of the pool's game objects.
	public void forEach(Action<GameObject> action, bool activeOnly) {
		for (int i=0; i<all.Count; i++){
			GameObject target  = all[i] as GameObject;
			
			if (!activeOnly || target.active) action(target);
		}
	}
	
	#endregion
	// ---- protected methods ----
	#region protected methods
	
	// Activates or deactivates the provided game object using the method
	// specified by the setActiveRecursively flag.
	protected void setActive(GameObject target, bool value) {
		if (setActiveRecursively) target.SetActiveRecursively(value);
		else target.active = value;
	}
	
	#endregion
}
