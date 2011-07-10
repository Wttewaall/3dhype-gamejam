using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour {
	
	public Cannon cannon;
	public GameObject ground;
	public GameObject explosion;
	
	protected Transform cannonTransform;
	
	void Awake() {
		if (cannon) cannonTransform = cannon.transform;
		//else throw new UnityException("cannon reference needed");
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
			
			if (cannon) cannon.Fire();
			
			var hits = Raycaster.instance.lastHits;
			
			foreach (RaycastHit hit in hits) {
				
				if (hit.transform.gameObject == ground) {
					GameObject.Instantiate(explosion, hit.point, Quaternion.identity);
				}
			}
		}
	}
}
