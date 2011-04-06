using UnityEngine;
using System.Collections;

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
	
	public bool		useLeftButton = true;
	public bool		useRightButton = true;
	public bool		useMiddleButton = true;
	public bool		useBackButton = true;
	public bool		useForwardButton = true;
	
	/** If checked, the drag events will trigger. */
	public bool		useDragging = true;
	
	/** When you move the mouse beyond this range the dragging will be triggered. */
	public float	dragRange = 2.0f;
	
	/** If checked, the double click event will trigger. */
	public bool		useDoubleClick = true;
	
	/** Value in seconds for triggering a doubleclick event. Default value is 250ms. */
	public float	doubleClickTime = 0.25f;
	
	/** While doubleclicking, if you move the mouse beyond this value, the double click will not trigger. */
	public float	doubleClickRange = 2.0f;
	
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
		
		for (int i=0; i<states.Length; i++) {
			
			state = states[i];
			if (state == null) continue;
			
			if (Input.GetMouseButton(i)) {
				if (state.isDown == false) {
					state.isDown = true;
					state.downPosition = Input.mousePosition;
					if (mouseDown != null) mouseDown(state.buttonID);
				}
				
				diff = Input.mousePosition - state.downPosition;
				
				if (useDragging && !state.dragging && diff.magnitude >= dragRange) {
					state.dragging = true;
					if (mouseDragStart != null) mouseDragStart(state.buttonID);
				}
			}
			
			if (Input.GetMouseButtonUp(i)) {
				state.isDown = false;
				if (mouseUp != null) mouseUp(state.buttonID);
				
				if (useDragging && state.dragging) {
					state.dragging = false;
					if (mouseDragStop != null) mouseDragStop(state.buttonID);
					
				} else if (!state.dragging) {
					if (mouseClick != null) mouseClick(state.buttonID);
				}
				
				diff = Input.mousePosition - state.downPosition;
				
				if (useDoubleClick && Time.time-state.downTime <= doubleClickTime && diff.magnitude < doubleClickRange) {
					if (mouseDoubleClick != null) mouseDoubleClick(state.buttonID);
					state.downTime = Time.time - doubleClickTime;
					
				} else {
					state.downTime = Time.time;
				}
				
			}
			
			if (useDragging && state.dragging) {
				if (mouseDragMove != null) mouseDragMove(state.buttonID);
			}
		}
	}
}

public class MouseState {
	
	public int		buttonID;
	public bool		isDown;
	public Vector3	downPosition;
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