using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class AimPath : MonoBehaviour {
	
	public GameObject start;
	public GameObject bullet;
	public GameObject reticle;
	
	public float force;
	public float groundLevel = 0;
	public int maxIterations = 30;
	public float framerate = 60;
	
	protected LineRenderer line;
	protected Color lineColor = Color.blue;
	protected Rigidbody rb;
	protected Vector3 v1;
	protected Vector3 v2;
	
	
	// ---- inherited handlers ----
	
	void Awake() {
		line = gameObject.GetComponent<LineRenderer>();
		rb = bullet.GetComponent<Rigidbody>();
	}
	
	
	void Update () {
		calc();
	}
	
	protected void calc() {
		Vector3 velocity = start.transform.forward * force / 3;//rb.mass;
		
		v1 = start.transform.position;
		v2 = start.transform.position;
		
		line.SetVertexCount(maxIterations);
		
		for (int i=0; i<maxIterations; i++) {
			v1 = v2;
			line.SetPosition(i, v2);
			
			//velocity /= rb.drag;
			velocity += Physics.gravity / framerate;
			
			v2 = v1 + velocity;
			
			Debug.DrawLine(v1, v2, Color.Lerp(Color.white, Color.red, (1/maxIterations) * i));
			
			if (v2.y <= groundLevel) {
				line.SetVertexCount(i+1);
				
				if (reticle) {
					var pos = reticle.transform.position;
					pos.x = v2.x;
					pos.z = v2.z;
					reticle.transform.position = pos;
				}
				
				break;
			}
		}
		
		
	}
}