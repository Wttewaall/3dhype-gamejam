using UnityEngine;
using System.Collections;

public class SpringTrap : MonoBehaviour {
	
	protected Hashtable springHash;
	protected Hashtable reloadHash;
	protected bool canSpring = true;
	protected Vector3 position;
	
	void Start () {
		position = transform.position;
		
		Vector3 pos = transform.position;
		pos.y += 0.5f;
		
		springHash = iTween.Hash(
			"position", pos,
			"time", 0.2f,
			"delay", 0.2f,
			"easetype", iTween.EaseType.easeOutQuint,
			"oncomplete", "springComplete"
		);
		
		reloadHash = iTween.Hash(
			"position", position,
			"time", 2,
			"delay", 2,
			"easetype", iTween.EaseType.easeInOutSine,
			"oncomplete", "reloadComplete"
		);
	}
	
	void OnCollisionEnter() {
		if (canSpring) {
			canSpring = false;
			iTween.MoveTo(gameObject, springHash);
		}
	}
	
	void springComplete() {
		iTween.MoveTo(gameObject, reloadHash);
	}
	
	void reloadComplete() {
		canSpring = true;
	}
	
}
