using UnityEngine;
using System.Collections;

public class MoveOverPath : MonoBehaviour {
	
	public string pathName;
	
	protected Vector3[] path;
	protected Hashtable tweenHash;
	
	// Use this for initialization
	void Start () {
		path = iTweenPath.GetPath(pathName);
		//transform.LookAt(path[0]);
		//transform.position = path[0];
		
		tweenHash = iTween.Hash(
			"path", path,
			"orienttopath", true,
			"movetopath", true,
			"time", 5,
			"delay", 1,
			"easetype", iTween.EaseType.easeInOutSine,
			"oncomplete", "pingpong"
		);
		
		iTween.MoveTo(gameObject, tweenHash);
	}
	
	protected void pingpong() {
		path = inverseArray(path);
		tweenHash["path"] = path;
		iTween.MoveTo(gameObject, tweenHash);
	}
	
	protected Vector3[] inverseArray(Vector3[] array) {
		Vector3[] newArray = new Vector3[array.Length];
		
		int i = array.Length;
		while (i-- > 0) newArray[array.Length-1-i] = array[i];
		
		return newArray;
	}
	
}
