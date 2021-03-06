/**
 * Copyright (c) 2011 Bart Wttewaall
 * Website: http://www.mediamonkey.nl
 */

using UnityEngine;
using System;

[AddComponentMenu("Mediamonkey/Managers/MouseManager")]

public class MouseManager : MonoBehaviour {
	
	// ---- singleton & static methods ----
	
	private static MouseManager _instance;
	
	public static MouseManager instance {
		get {
			if (!_instance) throw new UnityException("this behavior should be added at least once to a live object");
			return _instance;
		}
	}
	
	// ---- EventHandler & events ----
	
	public delegate void EventHandler(int buttonID);
	
	public static event EventHandler mouseUp;
	public static event EventHandler mouseDown;
	public static event EventHandler mouseClick;
	public static event EventHandler mouseDoubleClick;
	public static event EventHandler mouseDragStart;
	public static event EventHandler mouseDragMove;
	public static event EventHandler mouseDragStop;
	
	// ---- public settings ----
	
	public bool useLeftButton		= true;
	public bool useRightButton		= false;
	public bool useMiddleButton		= false;
	public bool useBackButton		= false;
	public bool useForwardButton	= false;
	
	public Dragging dragging = new Dragging();
	public DoubleClicking doubleClicking = new DoubleClicking();
	
	// ---- class members ----
	
	public static MouseState[] states;
	
	// ---- inherited ----
	
	void Awake () {
		if (!_instance) _instance = this;
		else return;
		
		states = new MouseState[5];
		if (useLeftButton)		states[0] = new MouseState(0);
		if (useRightButton)		states[1] = new MouseState(1);
		if (useMiddleButton)	states[2] = new MouseState(2);
		if (useBackButton)		states[3] = new MouseState(3);
		if (useForwardButton)	states[4] = new MouseState(4);
	}
	
	void Update () {
		MouseState	state;
		Vector3		diff;
		
		// TODO: get current ray at mouseposition
		/*if (useAngles && states.Length > 0) {
			Ray currentRay = new Ray(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.transform.forward);
		}*/
		
		for (int i=0; i<states.Length; i++) {
			
			state = states[i];
			if (state == null) continue;
			
			if (Input.GetMouseButton(i)) {
				if (state.isDown == false) {
					state.isDown = true;
					
					// TODO: store current ray in state
					//if (useAngles) state.downRay = currentRay;
					
					state.downPosition = Input.mousePosition;
					
					if (mouseDown != null) mouseDown(state.buttonID);
				}
				
				// TODO: calculate angle diff from currentRay - state.downRay
				diff = Input.mousePosition - state.downPosition;
				
				if (dragging.enabled && !state.dragging && diff.magnitude >= dragging.range) {
					state.dragging = true;
					if (mouseDragStart != null) mouseDragStart(state.buttonID);
				}
			}
			
			if (Input.GetMouseButtonUp(i)) {
				state.isDown = false;
				if (mouseUp != null) mouseUp(state.buttonID);
				
				if (dragging.enabled && state.dragging) {
					state.dragging = false;
					if (mouseDragStop != null) mouseDragStop(state.buttonID);
					
				} else if (!state.dragging) {
					if (mouseClick != null) mouseClick(state.buttonID);
				}
				
				diff = Input.mousePosition - state.downPosition;
				
				if (doubleClicking.enabled && Time.time-state.downTime <= doubleClicking.time && diff.magnitude < doubleClicking.range) {
					if (mouseDoubleClick != null) mouseDoubleClick(state.buttonID);
					state.downTime = Time.time - doubleClicking.time;
					
				} else {
					state.downTime = Time.time;
				}
				
			}
			
			if (dragging.enabled && state.dragging) {
				if (mouseDragMove != null) mouseDragMove(state.buttonID);
			}
		}
	}
}

[Serializable]
public class Dragging {
	
	/** If checked, the drag events will trigger. */
	public bool		enabled = false;
	
	/** When you move the mouse beyond this range the dragging will be triggered. */
	public float	range = 2.0f; // known issue: when raycasting with locked mouse, this must be set to 1.0f
	
	/** If checked, the drag position difference will be calculated by Rays. */
	public bool		useAngles = false;
	
}

[Serializable]
public class DoubleClicking {
	
	/** If checked, the drag events will trigger. */
	public bool		enabled = false;
	
	/** Value in seconds for triggering a doubleclick event. Default value is 250ms. */
	public float	time = 0.25f;
	
	/** While doubleclicking, if you move the mouse beyond this value, the double click will not trigger. */
	public float	range = 2.0f;
	
}

public class MouseState {
	
	public int		buttonID;
	public bool		isDown;
	public Vector3	downPosition;
	//public Ray	downRay;
	public float	downTime;
	public bool		dragging;
	
	public MouseState(int buttonID) {
		this.buttonID = buttonID;
		isDown = false;
		downTime = 0.0f;
	}
	
}

public struct MouseButton {
	
	public const int LEFT		= 0;
	public const int RIGHT		= 1;
	public const int MIDDLE		= 2;
	public const int BACK		= 3;
	public const int FORWARD	= 4;
	
}