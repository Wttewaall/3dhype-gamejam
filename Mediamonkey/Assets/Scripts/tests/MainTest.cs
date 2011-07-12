using UnityEngine;
using System.Collections;

public class MainTest : MonoBehaviour {
	
	protected KeyboardControl kc;
	protected MouseControl mc;
	
	protected void Start () {
		kc = GetComponent<KeyboardControl>();
		mc = GetComponent<MouseControl>();
		
		kc.enabled = false;
		mc.enabled = true;
	}
	
	void Update() {
		if (Input.GetKeyUp(KeyCode.C)) {
			kc.enabled = !kc.enabled;
			mc.enabled = !mc.enabled;
		}
	}
	
}
