using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
[AddComponentMenu("King's Ruby/Behaviors/CannonAim")]

public class CannonAim : MonoBehaviour {
	
	public Transform cannonTransform;
	public Transform originTransform;
	public Rigidbody bulletRigidbody;
	public Transform reticleTransform;
	
	public Axis pitchAxis		= Axis.invX; // vertical aim
	public float pitchOffset	= 0;
	public Axis yawAxis			= Axis.Y; // horizontal aim
	public float yawOffset		= 0;
	public float force			= 200;
	public float groundHeight	= 0;
	
	protected int iterations;
	protected LineRenderer line;
	protected float velocity;
	protected float anglePitch;
	protected float angleYaw;
	
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
	
	public float GetPitchAngle(Transform tf) {
		return (GetVector3AxisValue(tf.eulerAngles, pitchAxis) + pitchOffset) * Mathf.Deg2Rad;
	}
	
	public float GetYawAngle(Transform tf) {
		return (GetVector3AxisValue(tf.eulerAngles, yawAxis) + yawOffset) * Mathf.Deg2Rad;
	}
	
	public void AimAtPosition(Vector3 position) {
		
		reticleTransform.position = position + Vector3.up * 5;
		
		float distance = Vector3.Distance(originTransform.position, position);
		float initialHeight = originTransform.position.y - groundHeight;
		float targetHeight = groundHeight + position.y;
		
		float angle = Trajectory.AngleOfReach(distance, velocity, initialHeight, targetHeight, -Physics.gravity.y);
		if (float.IsNaN(angle)) return;
		
		Vector3 pos = cannonTransform.eulerAngles;
		SetVector3AxisValue(ref pos, angle * Mathf.Rad2Deg - pitchOffset, pitchAxis);
		
		Vector3 diff = position - cannonTransform.position;
		angle = Mathf.Atan2(diff.x, diff.z);
		
		SetVector3AxisValue(ref pos, angle * Mathf.Rad2Deg - yawOffset, yawAxis);
		
		cannonTransform.eulerAngles = pos;
		
		DrawTrajectory(originTransform.position, position, distance);
		
		// and once again by cannon angle
		//CalculatePath();
	}
	
	// ---- protected methods ----
	
	protected void CalculatePath() {
		float anglePitch = GetPitchAngle(originTransform);
		float angleYaw = GetYawAngle(originTransform);
		
		float initialHeight = originTransform.position.y - groundHeight; // FIXME: target height is unknown
		float distance = Trajectory.DistanceAtAngle(anglePitch, velocity, initialHeight, -Physics.gravity.y);
		
		float x = Mathf.Sin(angleYaw) * distance;
		float y = groundHeight - originTransform.position.y + 5;
		float z = Mathf.Cos(angleYaw) * distance;
		Vector3 impactLocation = originTransform.position + new Vector3(x, y, z);
		
		// set target position
		reticleTransform.position = impactLocation;
		DrawTrajectory(originTransform.position, impactLocation, distance);
	}
	
	protected void DrawTrajectory(Vector3 startPosition, Vector3 endPosition, float distance) {
		float anglePitch = GetPitchAngle(originTransform);
		//float angleYaw = GetYawAngle(originTransform); // not used?
		
		float initialHeight = startPosition.y - groundHeight;
		
		// get time of flight
		float flight = Trajectory.TimeOfFlight(anglePitch, velocity, distance);
		iterations = (int) Mathf.Round(flight*10);
		
		// draw line positions
		line.SetVertexCount(iterations+1);
		line.SetPosition(0, startPosition);
		
		for (int i=1; i<iterations; i++) {
			float t = (float)i / (float)iterations;
			float y = Trajectory.HeightAtDistance(distance*t, anglePitch, velocity, initialHeight, -Physics.gravity.y);
			
			Vector3 mid = Vector3.Lerp(startPosition, endPosition, t);
			Vector3 pos = new Vector3(mid.x, y, mid.z);
			
			line.SetPosition(i, pos);
		}
		
		// last position
		line.SetPosition(iterations, endPosition);
	}
	
	protected float GetVector3AxisValue(Vector3 vector, Axis axis) {
		float val = 0;
		switch(axis) {
			case Axis.X: val = vector.x; break;
			case Axis.Y: val = vector.y; break;
			case Axis.Z: val = vector.z; break;
			case Axis.invX: val = -vector.x; break;
			case Axis.invY: val = -vector.y; break;
			case Axis.invZ: val = -vector.z; break;
		}
		return val;
	}
	
	protected void SetVector3AxisValue(ref Vector3 vector, float val, Axis axis) {
		switch(axis) {
			case Axis.X: vector.x = val; break;
			case Axis.Y: vector.y = val; break;
			case Axis.Z: vector.z = val; break;
			case Axis.invX: vector.x = -val; break;
			case Axis.invY: vector.y = -val; break;
			case Axis.invZ: vector.z = -val; break;
		}
	}
	
}

public enum Axis {
	X, Y, Z,
	invX, invY, invZ
}