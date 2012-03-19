using UnityEngine;

public static class VQMath {
	
	// source: http://forums.create.msdn.com/forums/t/4574.aspx
	public static Vector3 QuaternionToVector3(Quaternion q) {
		Vector3 v = Vector3.zero;
		
		// yaw
		v.x = Mathf.Atan2( 
			2 * q.y * q.w - 2 * q.x * q.z,
			1 - 2 * Mathf.Pow(q.y, 2) - 2 * Mathf.Pow(q.z, 2)
		); 
		
		// roll
		v.z = Mathf.Asin(
			2 * q.x * q.y + 2 * q.z * q.w
		);
		
		// pitch
		v.y = Mathf.Atan2(
			2 * q.x * q.w - 2 * q.y * q.z,
			1 - 2 * Mathf.Pow(q.x, 2) - 2 * Mathf.Pow(q.z, 2)
		);
		
		if (q.x * q.y + q.z * q.w == 0.5f) {
			v.x = 2 * Mathf.Atan2(q.x, q.w);
			v.y = 0;
			
		} else if (q.x * q.y + q.z * q.w == -0.5f) {
			v.x = -2 * Mathf.Atan2(q.x, q.w);
			v.y = 0;
		}
		
		return v;
	}
	
	public static Quaternion Vector3ToQuaternion(Vector3 v) {
		return Quaternion.LookRotation(v, Vector3.up);
	}
	
}
