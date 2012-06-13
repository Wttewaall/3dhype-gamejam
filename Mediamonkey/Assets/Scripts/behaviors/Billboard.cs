using UnityEngine;

[ExecuteInEditMode]
public class Billboard : MonoBehaviour {
	
	protected Transform tf;
	public bool parallel = true;
	
	void Start() {
		tf = transform;
	}
	
    void Update() {
		if (parallel) {
			Vector3 flipped = Camera.main.transform.eulerAngles;
			flipped.x *= -1;
			flipped.y += 180;
			flipped.z *= -1;
			
			tf.rotation = Quaternion.Euler(flipped);
			
		} else {
			tf.LookAt(Camera.main.transform.position, Vector3.up);
		}
    }
}