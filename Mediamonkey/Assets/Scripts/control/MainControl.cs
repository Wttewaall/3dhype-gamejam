using UnityEngine;
using System.Collections;

public class MainControl : MonoBehaviour {
	
	protected bool mute;
	protected float volume;
	
	protected KeyboardControl kc;
	protected MouseControl mc;
	
	protected void Start () {
		kc = GetComponent<KeyboardControl>();
		mc = GetComponent<MouseControl>();
		
		kc.enabled = false;
		mc.enabled = true;
	}
	
	void Update() {
		// switch cannon controls
		if (Input.GetKeyUp(KeyCode.C)) {
			kc.enabled = !kc.enabled;
			mc.enabled = !mc.enabled;
		}
		
		// mute
		if (Input.GetKeyDown(KeyCode.M)) {
			if (!mute) volume = Camera.current.audio.volume;
			mute = !mute;
			Camera.current.audio.volume = (mute)? 0 : volume;
		}
		
		// restart
		if (Input.GetKeyDown(KeyCode.R)) Application.LoadLevel(0);
	}
	
}
