using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class VectorTest : MonoBehaviour {
	
	public List<Transform> targets;
	
	void Update () {
		
		var t1 = targets[0];
		var t2 = targets[1];
		
		foreach (Transform t in targets) {
			Debug.DrawLine(t.position, t.position + Vector3.forward * 2, Color.blue);
			Debug.DrawLine(t.position, t.position + Vector3.right * 2, Color.red);
			Debug.DrawLine(t.position, t.position + Vector3.up * 2, Color.green);
			
			Debug.DrawLine(t.position, t.position + t.forward * 1.5f, Color.cyan);
		}
		
		//var dot = Vector3.Dot(t2.position, t1.position);
		//var cross = Vector3.Cross(t2.position, t1.position);
		
		Debug.DrawLine(t1.position + t1.right * 10, t1.position - t1.right * 10, Color.yellow);
		
	}
}
