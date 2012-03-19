using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
	
	protected Bounds bounds;
	protected Transform tf;
	
	void Awake() {
		bounds = collider.bounds;
		tf = transform;
	}
	
	public virtual Vector3 GetSpawnPosition() {
		return new Vector3();
	}
	
	public virtual Vector3 GetSpawnPosition(Vector3 offset) {
		return GetSpawnPosition() + offset;
	}
	
	public virtual Vector3 GetSpawnDirection() {
		return new Vector3();
	}
	
}
