using UnityEngine;

public interface ISpawner {

	Vector3 GetSpawnPosition();
	Vector3 GetSpawnPosition(Vector3 offset);
	
}