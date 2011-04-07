using UnityEngine;
using System.Collections;

public class MouseTest : MonoBehaviour {
	
	protected MouseManager mouseManager;
	
	// Use this for initialization
	protected void Start () {
		mouseManager = MouseManager.instance;
		MouseManager.mouseClick += mouseClickHandler;
	}
	
	protected void mouseClickHandler(int buttonID) {
		Debug.Log("click "+buttonID);
	}
}
