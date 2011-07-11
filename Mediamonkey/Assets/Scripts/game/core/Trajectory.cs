using UnityEngine;
using System;

public static class Trajectory {
	
	public static float DistanceAtAngle(float angle, float velocity, float initialHeight, float gravity) {
		var sa = Mathf.Sin(angle);
		var ca = Mathf.Cos(angle);
		var vsa = velocity * sa;
		return velocity * ca / gravity * ( vsa + Mathf.Sqrt( vsa * vsa + (2 * gravity * initialHeight) ) );
	}
	
	public static float TimeOfFlight(float angle, float velocity, float initialHeight, float gravity) {
		float part = velocity * Mathf.Sin(angle);
		return (part + Mathf.Sqrt(part * part + 2 * gravity * initialHeight) ) / gravity;
	}
	
	public static float TimeOfFlight(float angle, float velocity, float distance) {
		return distance / (velocity * Mathf.Cos(angle));
	}
	
	public static float MaxDistanceAngle(float velocity, float initialHeight, float gravity) {
		float part = 2 * gravity * initialHeight + velocity * velocity;
		return Mathf.Acos( Mathf.Sqrt(part / (part * 2)));
	}
	
	public static float PeakHeight(float velocity, float initialHeight, float angle, float gravity) {
		var sa = Mathf.Sin(angle);
		return initialHeight + (velocity * velocity * sa * sa) / (2 * gravity);
	}
	
	public static float MaxHeight(float velocity, float initialHeight, float gravity) {
		return initialHeight + (velocity * velocity) / (2 * gravity);
	}
	
	public static Vector3 PositionAtTime(float time, float angle, float velocity, float initialHeight, float gravity) {
		float x = velocity * Mathf.Cos(angle) * time;
		float y = initialHeight + (velocity * Mathf.Sin(angle) * time) - (0.5f * gravity * time * time);
		return new Vector3(x, y, 0);
	}
	
	public static float HeightAtDistance(float distance, float angle, float velocity, float initialHeight, float gravity) {
		float ca = Mathf.Cos(angle);
		return initialHeight + distance * Mathf.Tan(angle) - ((gravity * distance * distance) / (2 * velocity * ca * velocity * ca));
	}
	
	public static float VelocityAtDistance(float distance, float angle, float velocity, float gravity) {
		float part = gravity * distance / (velocity * Mathf.Cos(angle));
		return Mathf.Sqrt( (velocity * velocity) - (2 * gravity * distance * Mathf.Tan(angle)) + part * part );
	}
	
}