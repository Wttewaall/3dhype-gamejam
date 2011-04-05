using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {

	public float rotateSpeed	= 20.0f;
	public float frequency		= 2.0f;
	public float amplitude		= 0.2f;
	
	protected Vector3 position;
	protected float y;
	
	void Start() {
		position = transform.position;
		y = position.y;
	}
	
	void Update() {
		transform.RotateAround(Vector3.zero, Vector3.up, rotateSpeed * Time.deltaTime);
		position.y = y + amplitude + Mathf.Sin(Time.time * frequency) * amplitude;
		transform.position = position;
	}
	
}