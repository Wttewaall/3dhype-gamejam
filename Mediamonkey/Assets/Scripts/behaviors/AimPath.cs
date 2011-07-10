using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]

public class AimPath : MonoBehaviour {
	
	public Transform originTransform;
	public Rigidbody bulletRigidbody;
	public Transform reticleTransform;
	
	public Axis pitchAxis		= Axis.iX; // vertical aim
	public float pitchOffset	= 0;
	public Axis yawAxis			= Axis.Y; // horizontal aim
	public float yawOffset		= 0;
	public float force			= 200;
	public float groundHeight	= 0;
	
	protected int iterations;
	protected LineRenderer line;
	protected float velocity;
	
	// ---- inherited handlers ----
	
	void Awake() {
		if (originTransform == null) originTransform = transform;
		line = gameObject.GetComponent<LineRenderer>();
		velocity = force / bulletRigidbody.mass;
	}
	
	void Update() {
		CalculatePath();
	}
	
	// ---- public methods ----
	
	public float GetPitchAngle() {
		return (GetVector3AxisValue(originTransform.eulerAngles, pitchAxis) + pitchOffset) * Mathf.Deg2Rad;
	}
	
	public float GetYawAngle() {
		return (GetVector3AxisValue(originTransform.eulerAngles, yawAxis) + yawOffset) * Mathf.Deg2Rad;
	}
	
	// ---- protected methods ----
	
	protected void CalculatePath() {
		float anglePitch = GetPitchAngle();
		float angleYaw = GetYawAngle();
		
		float initialHeight = originTransform.position.y - groundHeight;
		float distance = Trajectory.distanceAtAngle(anglePitch, velocity, initialHeight, -Physics.gravity.y);
		
		float x = Mathf.Sin(angleYaw) * distance;
		float y = groundHeight - originTransform.position.y + 5;
		float z = Mathf.Cos(angleYaw) * distance;
		Vector3 impactLocation = new Vector3(x, y, z);
		
		// set target position
		reticleTransform.position = originTransform.position + impactLocation;
		
		// get time of flight
		float flight = Trajectory.timeOfFlight(anglePitch, velocity, distance);
		iterations = (int) Mathf.Round(flight*10);
		
		// draw line positions
		line.SetVertexCount(iterations+1);
		line.SetPosition(0, originTransform.position);
		
		for (int i=1; i<iterations; i++) {
			float t = (float)i / (float)iterations;
			y = Trajectory.heightAtDistance(distance*t, anglePitch, velocity, initialHeight, -Physics.gravity.y);
			
			Vector3 mid = Vector3.Lerp(originTransform.position, originTransform.position + impactLocation, t);
			Vector3 pos = new Vector3(mid.x, y, mid.z);
			
			line.SetPosition(i, pos);
		}
		
		// last position
		line.SetPosition(iterations, originTransform.position + new Vector3(x, groundHeight - originTransform.position.y, z));
	}
	
	protected float GetVector3AxisValue(Vector3 vector, Axis axis) {
		float val = 0;
		switch(axis) {
			case Axis.X: val = vector.x; break;
			case Axis.Y: val = vector.y; break;
			case Axis.Z: val = vector.z; break;
			case Axis.iX: val = -vector.x; break;
			case Axis.iY: val = -vector.y; break;
			case Axis.iZ: val = -vector.z; break;
		}
		return val;
	}
	
}

public enum Axis {
	X, Y, Z,
	iX, iY, iZ
}