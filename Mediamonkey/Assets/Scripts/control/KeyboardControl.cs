using UnityEngine;
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
	protected bool mute;
	protected float volume;
	
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
		
		// mute
		if (Input.GetKeyDown(KeyCode.M)) {
			if (!mute) volume = Camera.current.audio.volume;
			mute = !mute;
			Camera.current.audio.volume = (mute)? 0 : volume;
		}
		
		// restart
		if (Input.GetKeyDown(KeyCode.R)) Application.LoadLevel(0);
	}
	
	protected void updateMove() {
		Vector3 euler = cannonTransform.rotation.eulerAngles;
		euler.y += (moveLeft) ? -yawSpeed : (moveRight) ? yawSpeed : 0;
		euler.z += (moveUp) ? -pitchSpeed : (moveDown) ? pitchSpeed : 0;
		
		cannonTransform.rotation = Quaternion.Euler(euler);
	}
	
}
