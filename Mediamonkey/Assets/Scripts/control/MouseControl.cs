using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour {
	
	public GameObject ground;
	public GameObject explosion;
	
	void Awake() {
		ground = GameObject.Find("Grass");
	}
	
	void Start() {
		Raycaster.instance.castType = RaycastType.Mouse;
	}
	
	void OnEnable() {
		MouseManager.mouseClick += mouseClickHandler;
	}
	
	void OnDisable() {
		MouseManager.mouseClick -= mouseClickHandler;
	}
	
	// ---- event handlers ----
	
	protected void mouseClickHandler(int buttonID) {
		if (buttonID == MouseButton.LEFT) {
			var hits = Raycaster.instance.lastHits;
			
			foreach (RaycastHit hit in hits) {
				
				if (hit.transform.gameObject == ground) {
					GameObject.Instantiate(explosion, hit.point, Quaternion.identity);
				}
			}
		}
	}
}
