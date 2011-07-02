using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChildUtils {

	public static List<Transform> getChildren(Transform parent, bool recurse) {
		List<Transform> children = new List<Transform>();
		
		int count = parent.GetChildCount();
		if (count == 0) return children;
		
		Transform child;
		for (int i=0; i<count; i++) {
			child = parent.GetChild(i);
			children.Add(child);
			
			if (recurse && child.GetChildCount() > 0) {
				List<Transform> subChildren = getChildren(child, recurse);
				children.AddRange(subChildren);
			}
		}
		
		return children;
	}
	
	public static List<GameObject> getChildren(GameObject parent, bool recurse) {
		List<GameObject> children = new List<GameObject>();
		
		Transform tf = parent.transform;
		int count = tf.GetChildCount();
		if (count == 0) return children;
		
		GameObject child;
		for (int i=0; i<count; i++) {
			child = tf.GetChild(i).gameObject;
			children.Add(child);
			
			if (recurse && child.transform.GetChildCount() > 0) {
				List<GameObject> subChildren = getChildren(child, recurse);
				children.AddRange(subChildren);
			}
		}
		
		return children;
	}
	
}
