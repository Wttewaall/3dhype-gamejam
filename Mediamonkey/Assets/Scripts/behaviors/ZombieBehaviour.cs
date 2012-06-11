using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Behaviors/ZombieBehaviour")]

public class ZombieBehaviour : MonoBehaviour {
	
	protected Transform tf;
	public bool move = true;
	public bool raisedArms = true;
	//public TransformOffset offset = new TransformOffset();
	public float speed = 0.01f;
	public List<Texture2D> textures;
	
	private Vector3 initialRotation;
	
	void Start() {
		tf = transform;
		initialRotation = tf.localRotation.eulerAngles;
		
		/*int index = Random.Range(0, textures.Count);
		Debug.Log("Using texture "+index+": "+textures[index]);
		
		var renderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer r in renderers) {
			r.material.SetTexture("_MainTex", textures[index]);
		}*/
	}
	
	void Update() {
		
		if (move) {
			tf.Translate(tf.TransformDirection(tf.forward) * speed);
			if (tf.position.z < -20) Die();
		}
		
		if (raisedArms) {
			
			//var range = 5.0f;
			//var speed = 0.05f;
			
			//var rot = tf.localRotation.eulerAngles;
			//rot.x = initialRotation.x - 90 - range/2 + Mathf.Sin(Time.frameCount * speed) * range;
			//rot.x = initialRotation.x - 90 + Mathf.PingPong(Time.time, 10) - 5;
			
			//tf.localRotation = Quaternion.Euler(rot);
			//tf.RotateAround(tf.right, Mathf.PingPong(Time.time*0.01f, 0.005f) - 0.0025f);
			//tf.RotateAroundLocal(tf.right, -90 + Mathf.PingPong(Time.time, 0.05f) - 0.025f);
			
			float amplitude = 45 * Mathf.Deg2Rad;
			float frequency = 0.1f;
			float PI2Freq = 2 * Mathf.PI * frequency;
			var val = amplitude * (Mathf.Sin(PI2Freq * Time.time) - Mathf.Sin(PI2Freq * (Time.time - Time.deltaTime)));
			//transform.position += val * transform.up;
			tf.RotateAround(tf.right, val);
		}
	}
	
	void Die() {
		DestroyImmediate(gameObject);
	}
	
}

/*
[Serializable]
public class TransformOffset {
	
	public Vector3 position;
	public Vector3 rotation;
	public Vector3 scale;
	
}*/