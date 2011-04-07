using UnityEngine;
using System.Collections;

public class KeyboardControl : MonoBehaviour {
	
	public GameObject cannon;
	public float panSpeed = 1;
	public float yawSpeed = 1;
	
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
	}
	
	protected void updateMove() {
		Vector3 euler = cannon.transform.rotation.eulerAngles;
		euler.y += (moveLeft) ? -panSpeed : (moveRight) ? panSpeed : 0;
		euler.z += (moveUp) ? -yawSpeed : (moveDown) ? yawSpeed : 0;
		
		cannon.transform.rotation = Quaternion.Euler(euler);
	}
	
}
