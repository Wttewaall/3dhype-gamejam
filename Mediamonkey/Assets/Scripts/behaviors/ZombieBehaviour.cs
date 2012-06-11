using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Behaviors/ZombieBehaviour")]

public class ZombieBehaviour : MonoBehaviour {
	
	protected Transform tf;
	public bool move = true;
	//public TransformOffset offset = new TransformOffset();
	public float speed = 0.01f;
	public List<Texture2D> textures;
	
	void Start() {
		tf = transform;
		
		int index = Random.Range(0, textures.Count);
		Debug.Log("Using texture "+index+": "+textures[index]);
		
		var renderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer r in renderers) {
			r.material.SetTexture("_MainTex", textures[index]);
		}
	}
	
	void Update() {
		
		if (move) {
			tf.Translate(tf.TransformDirection(tf.forward) * speed);
			if (tf.position.z < -20) Die();
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