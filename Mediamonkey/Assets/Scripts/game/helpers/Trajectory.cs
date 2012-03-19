using UnityEngine;
using System;

public static class Trajectory {
	
	/// <summary>
	/// Returns the angle needed to reach the given distance.
	/// </summary>
	public static float AngleOfReach(float distance, float velocity, float initialHeight, float targetHeight, float gravity) {
		float v2 = velocity * velocity;
		float v4 = v2 * v2;
		
		float discriminant = v4 - gravity * (gravity*distance*distance + 2*(targetHeight-initialHeight)*v2);
		float theta1 = Mathf.Atan((v2 + Mathf.Sqrt(discriminant)) / (gravity*distance));
		float theta2 = Mathf.Atan((v2 - Mathf.Sqrt(discriminant)) / (gravity*distance));
		
		return Mathf.Min(theta1, theta2);
	}
	
	/// <summary>
	/// Returns the calculated distance to the point of impact.
	/// Calculate the initialHeight to account for the difference in height
	/// </summary>
	public static float DistanceAtAngle(float angle, float velocity, float initialHeight, float gravity) {
		var sa = Mathf.Sin(angle);
		var ca = Mathf.Cos(angle);
		var vsa = velocity * sa;
		return velocity * ca / gravity * ( vsa + Mathf.Sqrt( vsa * vsa + (2 * gravity * initialHeight) ) );
	}
	
	/// <summary>
	/// returns the optimal angle to achieve the maximum distance
	/// </summary>
	public static float MaxDistanceAngle(float velocity, float initialHeight, float gravity) {
		float part = 2 * gravity * initialHeight + velocity * velocity;
		return Mathf.Acos( Mathf.Sqrt(part / (part * 2)));
	}
	
	/// <summary>
	/// return the potential maximum distance under the optimal angle
	/// </summary>
	public static float MaxDistance(float velocity, float initialHeight, float gravity) {
		float angle = MaxDistanceAngle(velocity, initialHeight, gravity);
		return DistanceAtAngle(angle, velocity, initialHeight, gravity);
	}
	
	/// <summary>
	/// returns the maximum height of the trajectory
	/// </summary>
	public static float PeakHeight(float velocity, float initialHeight, float angle, float gravity) {
		var sa = Mathf.Sin(angle);
		return initialHeight + (velocity * velocity * sa * sa) / (2 * gravity);
	}
	
	/// <summary>
	/// returns the potential maximum height (straight upwards)
	/// </summary>
	public static float MaxHeight(float velocity, float initialHeight, float gravity) {
		return initialHeight + (velocity * velocity) / (2 * gravity);
	}
	
	/// <summary>
	/// returns the time it will take to traverse the parabole path
	/// </summary>
	public static float TimeOfFlight(float angle, float velocity, float initialHeight, float gravity) {
		float part = velocity * Mathf.Sin(angle);
		return (part + Mathf.Sqrt(part * part + 2 * gravity * initialHeight) ) / gravity;
	}
	
	/// <summary>
	/// Shorter version where the distance is given as a parameter
	/// </summary>
	public static float TimeOfFlight(float angle, float velocity, float distance) {
		return distance / (velocity * Mathf.Cos(angle));
	}
	
	/// <summary>
	/// Returns the position at a given time.
	/// </summary>
	public static Vector3 PositionAtTime(float time, float angle, float velocity, float initialHeight, float gravity) {
		float x = velocity * Mathf.Cos(angle) * time;
		float y = initialHeight + (velocity * Mathf.Sin(angle) * time) - (0.5f * gravity * time * time);
		return new Vector3(x, y, 0);
	}
	
	/// <summary>
	/// Returns the position at a range, where value is between 0 and 1.
	/// </summary>
	public static Vector3 PositionAtValue(float value, float angle, float velocity, float initialHeight, float gravity) {
		float time = TimeOfFlight(angle, velocity, initialHeight, gravity);
		return PositionAtTime(time*value, angle, velocity, initialHeight, gravity);
	}
	
	/// <summary>
	/// Returns the height of a point on the parabole at a given distance.
	/// </summary>
	public static float HeightAtDistance(float distance, float angle, float velocity, float initialHeight, float gravity) {
		float ca = Mathf.Cos(angle);
		return initialHeight + distance * Mathf.Tan(angle) - ((gravity * distance * distance) / (2 * velocity * ca * velocity * ca));
	}
	
	/// <summary>
	/// Returns the velocity of a point on the parabole at a given distance.
	/// </summary>
	public static float VelocityAtDistance(float distance, float angle, float velocity, float gravity) {
		float part = gravity * distance / (velocity * Mathf.Cos(angle));
		return Mathf.Sqrt( (velocity * velocity) - (2 * gravity * distance * Mathf.Tan(angle)) + part * part );
	}
	
}