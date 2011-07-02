/**
 * Copyright (c) 2011 Bart Wttewaall
 * Website: http://www.mediamonkey.nl
 */

using UnityEngine;
using System;
using System.Collections;

/**
 * This class does a raycast from the mouseposition.
 * It fires an event with the sorted hits, from close to far away.
 * You can skip frames for performance by setting the interval property.
 */

[AddComponentMenu("Mediamonkey/Helpers/Raycaster")]

public class Raycaster : MonoBehaviour {
	
	public delegate void HitEventHandler(Ray ray, RaycastHit[] hits);
	
	public static event HitEventHandler hitUpdate;
	
	public static Raycaster instance;
	
	// raycast properties
	public int				interval = 1;
	public float			maxDistance = float.PositiveInfinity;
	public LayerMask		layer = -1; // Everything
	public RaycastType		castType = RaycastType.Mouse;
	
	// temporary properties
	private Ray				ray;
	private RaycastHit[]	hits;
	private RaycastHit		hit;
	
	private RaycastDistanceSorter sorter;
	
	private byte			frame = 0; // max 255
	
	// ---- getters & setters ----
	
	public Ray lastRay {
		get { return ray; }
	}
	
	public RaycastHit[] lastHits {
		get { return hits; }
	}
	
	// ---- inherited handlers ----
	
	void Awake () {
		if (Raycaster.instance != null)
			throw new UnityException("Singleton error: instance already set");
		
		// quick and easy singleton
		Raycaster.instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
		// only continue every n-th frame
		if (interval > 1 && ++frame % interval > 0) return;
		else frame = 0;
		
		castRay();
	}
	
	protected void castRay() {
		if (castType == RaycastType.Mouse) {
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
		} else if (castType == RaycastType.ScreenCenter) {
			ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
			//ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
			
			// known issue: when raycasting with locked mouse, MouseManager.dragRange must be set to (0 < n <= 1)
		}
		
		hits = Physics.RaycastAll(ray, maxDistance, layer);
		
		// sort the result on distance
		if (hits.Length > 1) {
			if (sorter == null) sorter = new RaycastDistanceSorter();
			System.Array.Sort(hits, sorter);
		}
		
		// dispatch event to any listeners
		if (hitUpdate != null) hitUpdate(ray, hits);
	}
	
	// ---- public methods ----
	
	public void useMouse() {
		castType = RaycastType.Mouse;
		castRay();
	}
	
	public void useCenter() {
		castType = RaycastType.ScreenCenter;
		castRay();
	}
	
	// ---- public static methods ----
	
	public static RaycastHit getHitByTransform(Transform tf) {
		
		foreach (RaycastHit hit in instance.hits) {
			if (hit.transform == tf) return hit;
		}
		
		return new RaycastHit();
	}
	
}

public enum RaycastType {
	Mouse, ScreenCenter
}

// class for comparing the distance between two RaycastHit objects
class RaycastDistanceSorter : IComparer {
	
	int IComparer.Compare(System.Object a, System.Object b) {
		if (!(a is RaycastHit && b is RaycastHit)) return 0;
		
		// cast object to RaycastHit
		RaycastHit raycastHitA = (RaycastHit) a;
		RaycastHit raycastHitB = (RaycastHit) b;
		
		// compare on distance
		return raycastHitA.distance.CompareTo(raycastHitB.distance);
	}
}