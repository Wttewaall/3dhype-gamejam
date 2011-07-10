using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

[AddComponentMenu("King's Ruby/Behaviors/Bullet")]

public class Bullet : MonoBehaviour {
	
	public float force		= 100;
	public HitStats hitStats;
	
	protected Rigidbody rb;
	
	void Start() {
		rb = rigidbody;
	}
	
}
