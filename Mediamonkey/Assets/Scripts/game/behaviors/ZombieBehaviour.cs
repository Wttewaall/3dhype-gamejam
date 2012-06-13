using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Behaviors/ZombieBehaviour")]

public class ZombieBehaviour : Enemy {
	
	public bool raisedArms = true;
	public float walkSpeed = 0.01f;
	public List<Texture2D> textures;
	
	protected Transform zombie;
	protected Transform head;
	protected Transform armLeft;
	protected Transform armRight;
	protected Transform legLeft;
	protected Transform legRight;
	
	float offset1;
	float offset2;
	
	override public void Initialize() {
		base.Initialize();
		
		// get references
		zombie		= tf.FindChild("Zombie");
		head		= tf.FindChild("Zombie/head");
		armLeft		= tf.FindChild("Zombie/armLeft");
		armRight	= tf.FindChild("Zombie/armRight");
		legLeft		= tf.FindChild("Zombie/legLeft");
		legRight	= tf.FindChild("Zombie/legRight");
		
		// randomize zombie
		offset1 = Random.value;
		offset2 = Random.value;
		SetRandomTexture(zombie.gameObject);
	}
	
	void Update() {
		float angle;
		
		if (move) {
			
			/*if (move && goal != null) {
				agent.SetDestination(goal.position);
			}*/
			
			tf.Translate(tf.InverseTransformDirection(tf.forward) * walkSpeed);
			
			if (tf.position.z < -20) {
				Die();
				return;
			}
			
			angle = Oscillate(45 * Mathf.Deg2Rad, walkSpeed * 20);
			legLeft.RotateAroundLocal(Vector3.forward, angle);
			legRight.RotateAroundLocal(Vector3.forward, -angle);
			
		}/* else {
			legLeft.rotation = Quaternion.identity;
			legRight.rotation = Quaternion.identity;
			//iTween.RotateTo(legLeft.gameObject, iTween.Hash("x", 0, "time", 1));
			//iTween.RotateTo(legRight.gameObject, iTween.Hash("x", 0, "time", 1));
		}*/
		
		if (raisedArms) {
			float rotation = Oscillate(90 * Mathf.Deg2Rad, 0.5f, offset1) * Mathf.Rad2Deg;
			armLeft.rotation = Quaternion.Euler(90 + rotation, 0, 0);
			armRight.rotation = Quaternion.Euler(90 + rotation, 0, 0);
		}
		
		angle = Oscillate(10 * Mathf.Deg2Rad, 0.6f, offset1);
		head.RotateAroundLocal(Vector3.right, angle);
		angle = Oscillate(10 * Mathf.Deg2Rad, 0.4f, offset2);
		head.RotateAroundLocal(Vector3.forward, angle);
	}
	
	protected float Oscillate(float amplitude, float frequency) {
		return Oscillate(amplitude, frequency, 0);
	}
	
	protected float Oscillate(float amplitude, float frequency, float offset) {
		float PI2Freq = 2 * Mathf.PI * frequency + offset;
		return amplitude * (Mathf.Sin(PI2Freq * Time.time) - Mathf.Sin(PI2Freq * (Time.time - Time.deltaTime)));
	}
	
	public virtual void SetRandomTexture(GameObject child) {
		if (textures.Count == 0) {
			Debug.LogWarning("No textures available");
			return;
		}
		
		int index = Random.Range(0, textures.Count);
		
		var renderers = child.GetComponentsInChildren<Renderer>();
		foreach (Renderer r in renderers) {
			r.material.SetTexture("_MainTex", textures[index]);
		}
		
	}
	
}