using UnityEngine;
using System;
using System.Collections;

public class KeyboardControl : MonoBehaviour {
	
	public Cannon cannon;
	public float yawSpeed = 1;
	public float pitchSpeed = 1;
	
	protected Transform cannonTransform;
	protected bool moveUp;
	protected bool moveDown;
	protected bool moveLeft;
	protected bool moveRight;
	protected bool fire;
	
	[NonSerializedAttribute]
	public string controlsText = "" +
		"Aim with the arrow buttons\n" +
		"Press SPACE key to fire";
	
	// ---- inherited handlers ----
	
	void Awake() {
		if (cannon) cannonTransform = cannon.transform;
		else throw new UnityException("cannon reference needed");
	}
	
	void Update() {
		// movement
		moveUp = Input.GetKey(KeyCode.UpArrow);
		moveDown = Input.GetKey(KeyCode.DownArrow);
		moveLeft = Input.GetKey(KeyCode.LeftArrow);
		moveRight = Input.GetKey(KeyCode.RightArrow);
		if (moveUp || moveDown || moveLeft || moveRight) updateMove();
		
		// fire
		fire = Input.GetKeyDown(KeyCode.Space);
		if (fire) cannon.Fire();
	}
	
	// ---- protected methods ----
	
	protected void updateMove() {
		Vector3 euler = cannonTransform.rotation.eulerAngles;
		euler.y += (moveLeft) ? -yawSpeed : (moveRight) ? yawSpeed : 0;
		euler.z += (moveUp) ? -pitchSpeed : (moveDown) ? pitchSpeed : 0;
		
		cannonTransform.rotation = Quaternion.Euler(euler);
	}
	
}
