using UnityEngine;

[ExecuteInEditMode]
public class Billboard : MonoBehaviour {
	
	protected Transform tf;
	//public bool perpendicular = true;
	
	void Start() {
		tf = transform;
	}
	
    void Update() {
		/*if (perpendicular) {
			var heading = tf.position - Camera.main.transform.position;
			heading.y = 0;
			
			var direction = heading / heading.magnitude;
			
			var perp = Vector3.Cross(tf.position, Camera.main.transform.position);
			Debug.Log("perp: "+perp);
			
			Vector3 project = Vector3.Project(tf.position, Camera.main.transform.right);
			Debug.Log("project: "+project);
			
			tf.rotation = Quaternion.Euler(perp);
			
		} else {*/
			tf.LookAt(Camera.main.transform.position, Vector3.up);
		//}
    }
}