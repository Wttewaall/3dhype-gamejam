using UnityEngine;
using System.Collections;

public class TrailBullet : MonoBehaviour {
	
	public Material[] materials;
	
	void OnEnable() {
		Utils.trace(name, "enabled");
		
		TrailRenderer trail = GetComponent<TrailRenderer>();
		if (!trail) trail = gameObject.AddComponent<TrailRenderer>();
		
		trail.materials = materials;
		trail.startWidth = 0.05f;
		trail.endWidth = 0.10f;
		trail.time = 3.0f;
		
		trail.enabled = true;
	}
	
	void OnDisable() {
		Utils.trace(name, "disabled");
		
		TrailRenderer trail = GetComponent<TrailRenderer>();
		if (trail) Destroy(trail);
	}
	
}
