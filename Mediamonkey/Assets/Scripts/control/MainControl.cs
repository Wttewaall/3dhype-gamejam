using UnityEngine;
using System;
using System.Collections;

public class MainControl : MonoBehaviour {
	
	public bool startWithMouseControls = true;
	
	protected bool mute;
	protected float volume;
	
	protected KeyboardControl kc;
	protected MouseControl mc;
	
	[NonSerializedAttribute]
	public string controlsText = "" +
		"C = toggle keyboard/mouse controls\n" +
		"M = toggle mute\n" +
		"R = reset level";
	
	protected void Start () {
		kc = GetComponent<KeyboardControl>();
		mc = GetComponent<MouseControl>();
		
		kc.enabled = !startWithMouseControls;
		mc.enabled = startWithMouseControls;
	}
	
	void OnGUI() {
		GUILayout.TextArea(controlsText);
		if (kc.enabled) GUILayout.TextArea(kc.controlsText);
		if (mc.enabled) GUILayout.TextArea(mc.controlsText);
	}
	
	void Update() {
		// switch cannon controls
		if (Input.GetKeyUp(KeyCode.C)) {
			kc.enabled = !kc.enabled;
			mc.enabled = !mc.enabled;
		}
		
		// mute
		if (Input.GetKeyDown(KeyCode.M)) {
			if (!mute) volume = AudioListener.volume;
			mute = !mute;
			AudioListener.volume = (mute)? 0 : volume;
		}
		
		// restart
		if (Input.GetKeyDown(KeyCode.R)) Application.LoadLevel(0);
	}
	
}
