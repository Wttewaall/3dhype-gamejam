using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour {
	
	public Cannon cannon;
	public GameObject ground;
	
	protected Transform cannonTransform;
	protected Vector3 aimPosition;
	
	void Awake() {
		if (cannon) cannonTransform = cannon.transform;
		else throw new UnityException("cannon reference needed");
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
	
	void Update() {
		if (Raycaster.instance != null) {
			RaycastHit[] hits = Raycaster.instance.lastHits;
			
			if (hits.Length > 0) {
				float dist = Vector3.Distance(aimPosition, hits[0].point);
				aimPosition = Vector3.Lerp(aimPosition, hits[0].point, dist/50); // ease
				cannon.GetComponent<CannonAim>().AimAtPosition(aimPosition);
			}
		}
	}
	
	// ---- event handlers ----
	
	protected void mouseClickHandler(int buttonID) {
		if (buttonID == MouseButton.LEFT) {
			
			if (cannon) cannon.Fire();
			
			/*RaycastHit[] hits = Raycaster.instance.lastHits;
			foreach (RaycastHit hit in hits) {
				if (hit.transform.gameObject == ground)
					GameObject.Instantiate(explosion, hit.point, Quaternion.identity);
			}*/
		}
	}
}
