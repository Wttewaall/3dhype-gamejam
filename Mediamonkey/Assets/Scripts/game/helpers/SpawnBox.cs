using UnityEngine;

[AddComponentMenu("King's Ruby/Helpers/SpawnBox")]

public class SpawnBox : MonoBehaviour, ISpawner {
	
	protected Bounds bounds;
	protected Transform tf;
	
	void Awake() {
		bounds = collider.bounds;
		tf = transform;
	}
	
	public Vector3 GetSpawnPosition() {
		var x = Random.Range(bounds.min.x, bounds.max.x);
		var z = Random.Range(bounds.min.z, bounds.max.z);
		return new Vector3(x, bounds.min.y, z);
	}
	
	public Vector3 GetSpawnPosition(Vector3 offset) {
		return GetSpawnPosition() + offset;
	}
}
