using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveTest : MonoBehaviour {
	
	public GameObject prefab;
	public Transform ground;
	
	void OnEnable() {
		MouseManager.mouseClick += mouseClickHandler;
	}
	
	void OnDisable() {
		MouseManager.mouseClick -= mouseClickHandler;
	}

	void mouseClickHandler(int buttonID) {
		if (buttonID == MouseButton.LEFT) {
			RaycastHit hit = Raycaster.getHitByTransform(ground);
			if (hit.transform != null) Spawn(hit.point);
		}
	}
	
	void Spawn(Vector3 point) {
		Instantiate(prefab, point + Vector3.up * 5, Quaternion.identity);
	}
	
}