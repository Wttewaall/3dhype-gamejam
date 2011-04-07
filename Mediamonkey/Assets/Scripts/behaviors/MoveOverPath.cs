using UnityEngine;
using System.Collections;

public class MoveOverPath : MonoBehaviour {
	
	public string pathName;
	
	// Use this for initialization
	void Start () {
		Vector3[] path = iTweenPath.GetPath(pathName);
		transform.LookAt(path[0]);
		transform.position = path[0];
		iTween.MoveTo(gameObject, iTween.Hash("path", path, "orienttopath", true, "time", 5, "delay", 1, "easetype", iTween.EaseType.easeInOutSine));
	}
	
}
