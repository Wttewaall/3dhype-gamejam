using UnityEngine;
using System.Collections;

public class KeyboardControl : MonoBehaviour {
	
	public GameObject cannon;
	public float yawSpeed = 1;
	public float pitchSpeed = 1;
	
	protected bool moveUp;
	protected bool moveDown;
	protected bool moveLeft;
	protected bool moveRight;
	protected bool fire;
	
	void Update () {
		moveUp = Input.GetKey(KeyCode.UpArrow);
		moveDown = Input.GetKey(KeyCode.DownArrow);
		moveLeft = Input.GetKey(KeyCode.LeftArrow);
		moveRight = Input.GetKey(KeyCode.RightArrow);
		if (moveUp || moveDown || moveLeft || moveRight) updateMove();
		
		fire = Input.GetKeyDown(KeyCode.Space);
		if (fire) cannon.GetComponent<Cannon>().fire();
		
		if (Input.GetKeyDown(KeyCode.R)) Application.LoadLevel(0);
	}
	
	protected void updateMove() {
		Vector3 euler = cannon.transform.rotation.eulerAngles;
		euler.y += (moveLeft) ? -yawSpeed : (moveRight) ? yawSpeed : 0;
		euler.z += (moveUp) ? -pitchSpeed : (moveDown) ? pitchSpeed : 0;
		
		cannon.transform.rotation = Quaternion.Euler(euler);
	}
	
}
