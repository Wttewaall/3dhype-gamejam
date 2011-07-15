using UnityEngine;

public class Billboard : MonoBehaviour {
	
	protected Transform tf;
	
	void Start() {
		tf = transform;
	}
	
    void Update() {
		tf.LookAt(Camera.main.transform.position, Vector3.up);
    }
}