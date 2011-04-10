using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class AimPath : MonoBehaviour {
	
	public GameObject start;
	public int iterations = 30;
	public GameObject bullet;
	public float framerate = 60;
	
	protected LineRenderer line;
	protected Rigidbody rb;
	
	// ---- inherited handlers ----
	
	void Awake() {
		line = gameObject.GetComponent<LineRenderer>();
		rb = bullet.GetComponent<Rigidbody>();
	}
	
	void OnEnable() {
		line.enabled = true;
		rb.Sleep();
	}
	
	void OnDisable() {
		line.enabled = false;
		rb.Sleep();
	}
	
	
	protected Vector3 v1;
	protected Vector3 v2;
	protected Color lineColor = Color.blue;
	protected Vector3 gravity = Physics.gravity;
	protected float bulletSpeed = 30;
	
	void Update () {
		calc();
	}
	
	protected void calc() {
		Vector3 direction = start.transform.forward;
		Vector3 velocity = direction * 3;
		float dampening = 0.8f;
		
		v1 = start.transform.position;
		v2 = start.transform.position;
		
		line.SetVertexCount(iterations);
		
		for (int i=0; i<iterations; i++) {
			v1 = v2;
			line.SetPosition(i, v2);
			
			velocity *= dampening;
			velocity += Physics.gravity / framerate;
			
			v2 = v1 + velocity;
			
			Debug.DrawLine(v1, v2, Color.Lerp(Color.white, Color.red, (1/iterations) * i));
		}
		
		
	}
}