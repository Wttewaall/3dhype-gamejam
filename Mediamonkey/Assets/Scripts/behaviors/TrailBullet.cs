using UnityEngine;
using System.Collections;

public class TrailBullet : MonoBehaviour {
	
	protected TrailRenderer trail;
	
	void Awake() {
		trail = GetComponent<TrailRenderer>();
	}
	
	void OnEnable() {
		trail.enabled = true;
	}
	
	void OnDisable() {
		trail.enabled = false;
	}
	
}
