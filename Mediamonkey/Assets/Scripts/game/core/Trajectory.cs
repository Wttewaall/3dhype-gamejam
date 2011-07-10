using UnityEngine;
using System;

public static class Trajectory {
	
	/*
	public static float m;
	public static float F;
	public static float g;
	
	public static float dx;
	public static float dy;
	public static float a;
	public static float sa;
	public static float ca;
	public static float dist;
	public static float t;
	public static float imp;
	public static Vector3 velocity;
	public static float v;
	public static float d;
	public static float h;
	public static float hmax;
	public static float x0;
	public static float y0;
	public static float ai;
	public static float RmaxAngle;
	public static float Rmax;
	
	public static void SetVariables(float angle, float force, float mass, Vector3 position, float gravity) {
		a = angle;
		sa = Mathf.Sin(a);
		ca = Mathf.Cos(a);
		
		imp = force/mass; // impulse force
		velocity = new Vector3(ca*imp, -sa*imp, 0);
		v = velocity.magnitude;
		
		x0 = position.x;
		y0 = position.y;
		g = gravity;
	}*/
	
	/*public static void updateForward() {
		
		// collision distance
		d = distanceAtAngle(a, v, y0, g);
		drawBorderAtDistance(d, 0x00FF00, true);
		enemy.x = cannon.x + d;
		enemy.y = ground.y;
		
		h = peakHeight(v, y0, a, g);
		if (a < 0) h = y0;
		drawBorderAtDistance(h, 0x00FF00, false);
		
		hmax = maxHeight(v, y0, g);
		drawBorderAtDistance(hmax, 0xFF0000, false);
		
		// time of flight in seconds
		t = d/(v*ca);
		//t = (v*sa + Mathf.Sqrt( (v*sa)*(v*sa) + 2*g*y0 )) / g;
		
		RmaxAngle = maxDistanceAngle(v, y0, g);
		Rmax = distanceAtAngle(RmaxAngle, v, y0, g);
		drawBorderAtDistance(Rmax, 0xFF0000, true);
		
		ai = angleOfImpact(a, v, 0, g);
		impactArrow.x = cannon.x + d;
		impactArrow.y = ground.y;
		impactArrow.rotation = 90-(ai*RAD_TO_DEG);
		
		// draw knots
		int knots = Mathf.round(t*2);
		for (int i=1; i<knots+1; i++) {
			float x = d*(i/(knots+1));
			float y = heightAtDistance(x);
			float vd = velocityAtDistance(x);
			drawPoint(x, y, 0x000000);
		}
		
		drawTrajectoryForward(0x0000FF);
	}*/
	
	/*protected function updateEnemy(event:Event=null):void {
		p = F/m; // impact force
		velocity = new Point(ca*p, -sa*p);
		v = velocity.magnitude;
		
		d = enemy.x - cannon.x;
		a = angleOfReach(d, v, g);
		
		cannon.rotation = -a * RAD_TO_DEG;
	}*/
	
	// formule is alleen voor y0 = 0
	public static float angleOfReach(float distance, float velocity, float gravity) {
		return 0.5f * Mathf.Asin((gravity*distance)/(velocity*velocity));
	}
	
	// incorrect?
	public static float angleOfImpact(float angle, float velocity, float initialHeight, float gravity) {
		float sa = Mathf.Sin(angle);
		float ca = Mathf.Cos(angle);
		return Mathf.Atan( Mathf.Sqrt((velocity * velocity * sa * sa) + 2 * gravity * initialHeight) / (velocity * ca));
	}
	
	public static float maxDistanceAngle(float velocity, float initialHeight, float gravity) {
		return Mathf.Acos( Mathf.Sqrt((2 * gravity * initialHeight + velocity * velocity) / (2 * gravity * initialHeight + velocity * velocity * 2)) );
	}
	
	public static float distanceAtAngle(float angle, float velocity, float initialHeight, float gravity) {
		var sa = Mathf.Sin(angle);
		var ca = Mathf.Cos(angle);
		var vsa = velocity*sa;
		return velocity * ca / gravity * ( vsa + Mathf.Sqrt( vsa * vsa + (2 * gravity * initialHeight) ) );
	}
	
	public static float timeOfFlight(float angle, float velocity, float initialHeight, float gravity) {
		float part = velocity * Mathf.Sin(angle);
		return (part + Mathf.Sqrt(part * part + 2 * gravity * initialHeight) ) / gravity;
	}
	
	public static float timeOfFlight(float angle, float velocity, float distance) {
		return distance / (velocity*Mathf.Cos(angle));
	}
	
	public static float peakHeight(float velocity, float initialHeight, float angle, float gravity) {
		var sa = Mathf.Sin(angle);
		return initialHeight + (velocity*velocity)*(sa*sa)/(2*gravity);
	}
	
	public static float maxHeight(float velocity, float initialHeight, float gravity) {
		return initialHeight + (velocity*velocity)/(2*gravity);
	}
	
	public static Vector3 positionAtTime(float time, float angle, float velocity, float initialHeight, float gravity) {
		float x = velocity * Mathf.Cos(angle) * time;
		float y = initialHeight + (velocity * Mathf.Sin(angle) * time) - (0.5f * gravity * time * time);
		return new Vector3(x, y, 0);
	}
	
	public static float heightAtDistance(float distance, float angle, float velocity, float initialHeight, float gravity) {
		float ca = Mathf.Cos(angle);
		return initialHeight + distance * Mathf.Tan(angle) - ((gravity * distance * distance) / (2 * velocity * ca * velocity * ca));
	}
	
	public static float velocityAtDistance(float distance, float angle, float velocity, float gravity) {
		float part = gravity * distance / (velocity * Mathf.Cos(angle));
		return Mathf.Sqrt( (velocity*velocity) - (2 * gravity * distance * Mathf.Tan(angle)) + part * part );
	}
	
	/*public static void drawTrajectoryForward(Color color) {
		Vector3 p;
		int numLines = 30;
		
		graphics.lineStyle(0, color, 0.5f);
		p = positionAtTime(0);
		graphics.moveTo(x0+p.x, ground.y-p.y);
		
		for (int i=1; i<=numLines; i++) {
			p = positionAtTime(i/numLines*t);
			graphics.lineTo(x0+p.x, ground.y-p.y);
		}
	}*/
	
	/*public static void drawBorderAtDistance(float distance, Color color, bool vertical) {
		graphics.lineStyle(0, color, 0.5f);
		if (vertical) {
			graphics.moveTo(x0+distance, 0);
			graphics.lineTo(x0+distance, 400);
		} else {
			graphics.moveTo(0, ground.y-distance);
			graphics.lineTo(550, ground.y-distance);
		}
	}*/
	
	/*public static void drawPoint(float x, float y, Color color) {
		graphics.lineStyle(1, color, 0.5f);
		graphics.drawCircle(x0+x, ground.y-y, 2);
	}*/
	
	/*public static void updateBackward(event:Event=null):void {
		dx = enemy.x - cannon.x;
		dy = enemy.y - cannon.y;
		dist = Mathf.Sqrt(dx*dx + dy*dy);
		t = dist/force;
		
		velocity = new Point(dx/t, dy/t);
		velocity.y -= (t * g) / 2;
		
		a = Mathf.Atan2(velocity.y, velocity.x);
		cannon.rotation = a * RAD_TO_DEG;
		
		graphics.clear();
		drawTrajectory(new Point(cannon.x, cannon.y), velocity, 0x00FF00);
	}*/
	
	/*
	http://wiki.secondlife.com/wiki/PhysicsLib
	public function pAngleOfReachHeight(distance:Number,velocity:Number, gravity:Number, angle:Number):Number {
		var Theta:Number;
		var V2:Number = velocity*velocity;
		var V4:Number = velocity*velocity*velocity*velocity;
		var D2:Number = distance*distance;
		var Y:Number = tPos.z - cPos.z;
		
		var Thingy:Number = V4 - ( gravity * (gravity*D2 + 2*Y*V2 ));
		if (Thingy <= 0) return null; // no good result
		
		float Sqrt = Mathf.Sqrt(Thingy);
		float x;
		if( angle ) x = V2 + Sqrt;
		else x = V2 - Sqrt;
		float y = gravity*distance;
		Theta = llAtan2(x,y);
		return <0, Mathf.Sin( Theta/2 ), 0, -Mathf.Cos( Theta/2 )>;
	}
	*/

}