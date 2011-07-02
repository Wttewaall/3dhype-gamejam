using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Mediamonkey/Managers/CursorManager")]

public class CursorManager : MonoBehaviour {
	
	// public variables (serializable)
	public List<Cursor> cursors;
	public int defaultCursorIndex = -1;
	
	// protected variables
	protected Rect emptyOffsetRect = new Rect(0, 0, 1, 1);
	protected List<CursorPriorityPair> priorityList = new List<CursorPriorityPair>();
	
	// private flags
	private bool _enabledReticleVisibility		= false;
	private bool _enabledScreenCursorVisibility	= true;
	
	// ---- getters & setters ----
	
	private static CursorManager _instance;
	
	public static CursorManager instance {
		get { return _instance; }
	}
	
	private GUITexture _reticle;
	
	public GUITexture reticle {
		get {
			if (!_reticle) {
				GameObject go = new GameObject("reticle");
				go.hideFlags = HideFlags.HideAndDontSave;
				go.transform.position = Vector3.zero;
				
				_reticle = go.AddComponent<GUITexture>();
				_reticle.transform.position = Vector3.zero;
				_reticle.transform.localScale = Vector3.zero;
				_reticle.enabled = _enabledReticleVisibility;
			}
			return _reticle;
		}
	}
	
	private Texture2D _emptyTexture;
	
	public Texture2D emptyTexture {
		get {
			if (!_emptyTexture) {
				_emptyTexture = new Texture2D(1, 1);
			}
			return _emptyTexture;
		}
	}	
	
	private int _selectedIndex = -1;
	
	public int selectedIndex {
		get { return _selectedIndex; }
		set {
			if (_selectedIndex == value) return;
			_selectedIndex = value;
			
			UpdateCursor();
			DispatchChangeEvent();
		}
	}
	
	public Cursor selectedCursor {
		get {
			bool validIndex = (selectedIndex > -1 && selectedIndex < cursors.Count);
			return validIndex ? cursors[selectedIndex] : null;
		}
		set {
			if (value == null) selectedIndex = -1;
			else selectedIndex = cursors.IndexOf(value);
		}
	}
	
	public bool reticleVisible {
		get { return _enabledReticleVisibility; }
	}
	
	public bool screenCursorVisible {
		get { return _enabledScreenCursorVisibility; }
	}
	
	public Vector3 screenMousePosition {
		get { return new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0); }
	}
	
	public Vector3 viewportMousePosition {
		get { return new Vector3(Camera.current.pixelWidth / Input.mousePosition.x, Camera.current.pixelHeight / Input.mousePosition.y, 0); }
	}
	
	public bool showingScreenCursor {
		get { return Screen.showCursor; }
	}
	
	private bool _followMouse = true;
	
	public bool followMouse {
		get { return _followMouse; }
		set {
			if (_followMouse == value) return;
			_followMouse = value;
		}
	}
	
	// ---- default handlers ----
	
	void Awake() {
		if (_instance != null)
			throw new UnityException("You can have only one instance of this component");
		else _instance = this;
		
		// begin with cursor at index defaultCursorIndex
		if (defaultCursorIndex > -1 && defaultCursorIndex < cursors.Count) {
			SetCursor(cursors[defaultCursorIndex]);
			ShowReticle();
		}
	}
	
	void Update() {
		if (_followMouse && enabled) PositionAt(screenMousePosition);
	}
	
	void OnEnable() {
		reticle.enabled = _enabledReticleVisibility;
		Screen.showCursor = _enabledScreenCursorVisibility;
		UpdateCursor();
	}
	
	void OnDisable () {
		reticle.enabled = false;
		Screen.showCursor = true;
		UpdateCursor();
	}
	
	void OnApplicationQuit() {
		if (_reticle) DestroyImmediate(_reticle.gameObject);
	}
	
	// ---- public methods ----
	
	public Cursor GetCursorByName(string cursorName) {
		if (name == null || name == "") return null;
		
		foreach (Cursor cursor in cursors) {
			if (cursor.name == cursorName) return cursor;
		}
		
		return null;
	}
	
	public void SetCursor(string cursorName) {
		Cursor cursor = GetCursorByName(cursorName);
		if (cursor != null) SetCursor(cursor, CursorPriority.LOW);
	}
	
	public void SetCursor(string cursorName, int priority) {
		Cursor cursor = GetCursorByName(cursorName);
		if (cursor != null) SetCursor(cursor, priority);
	}
	
	public void SetCursor(Cursor cursor) {
		SetCursor(cursor, CursorPriority.LOW);
	}
	
	public void SetCursor(Cursor cursor, int priority) {
		
		CursorPriorityPair pair = GetCursorPriorityPair(priority);
		
		if (pair == null) {
			// add cursor with new priority
			pair = new CursorPriorityPair(cursor, priority);
			priorityList.Add(pair);
			
		} else {
			// overwrite cursor with same priority
			pair.cursor = cursor;
			pair.priority = priority;
		}
		
		UpdateCursor();
	}
	
	public void RemoveCursor(string cursorName) {
		CursorPriorityPair pair = GetCursorPriorityPair(cursorName);
		if (pair == null) return;
		
		priorityList.Remove(pair);
		UpdateCursor();
	}
	
	public void RemoveCursor(int priority) {
		CursorPriorityPair pair = GetCursorPriorityPair(priority);
		if (pair == null) return;
		
		priorityList.Remove(pair);
		UpdateCursor();
	}
	
	public void RemoveAllCursors() {
		priorityList.Clear();
		UpdateCursor();
	}
	
	public void ShowReticle() {
		ShowReticle(false);
	}
	
	public void ShowReticle(bool withScreenCursor) {
		_enabledReticleVisibility = true;
		
		if (enabled) {
			reticle.enabled = true;
			
			if (withScreenCursor) ShowScreenCursor();
			else HideScreenCursor();
		}
	}
	
	public void HideReticle() {
		_enabledReticleVisibility = false;
		if (enabled) reticle.enabled = _enabledReticleVisibility;
	}
	
	public void ShowScreenCursor() {
		if (enabled) Screen.showCursor = _enabledScreenCursorVisibility = true;
	}
	
	public void HideScreenCursor() {
		if (enabled) Screen.showCursor = _enabledScreenCursorVisibility = false;
	}
	
	public void FollowMouse(bool value) {
		followMouse = value;
	}
	
	public void PositionAt(Vector3 position) {
		reticle.transform.position = position;
	}
	
	public void PositionAtCenter() {
		followMouse = false;
		PositionAt(new Vector3(0.5f, 0.5f, 0));
	}
	
	// ---- protected methods ----
	
	protected void UpdateCursor() {
		
		// get most important cursor by priority
		CursorPriorityPair pair = GetHighestCursorPriorityPair();
		
		// set cursorIndex without triggering another UpdateCursor
		_selectedIndex = (pair != null) ? cursors.IndexOf(pair.cursor) : -1;
		
		if (selectedCursor != null) {
			reticle.texture = selectedCursor.icon;
			reticle.pixelInset = selectedCursor.offsetRect;
			
		} else {
			reticle.texture = emptyTexture;
			reticle.pixelInset = emptyOffsetRect;
		}
	}
	
	protected CursorPriorityPair GetHighestCursorPriorityPair() {
		CursorPriorityPair highest = null;
		
		for (int i=0; i<priorityList.Count; i++) {
			highest = (highest == null || priorityList[i].priority >= highest.priority) ? priorityList[i] : highest;
		}
		
		return highest;
	}
	
	protected CursorPriorityPair GetCursorPriorityPair(int priority) {
		
		for (int i=0; i<priorityList.Count; i++) {
			if (priorityList[i].priority == priority) return priorityList[i];
		}
		
		return null;
	}
	
	protected CursorPriorityPair GetCursorPriorityPair(string cursorName) {
		
		for (int i=0; i<priorityList.Count; i++) {
			if (priorityList[i].cursor.name == cursorName) return priorityList[i];
		}
		
		return null;
	}
	
	protected void DispatchChangeEvent() {
		//..
	}
	
}

[Serializable]
public class Cursor {
	
	public string name;
	public Texture icon;
	public Vector2 offset;
	public int priority = 1;
	
	public Cursor() {
	}
	
	public Rect offsetRect {
		get {
			if (icon == null) return new Rect(offset.x, offset.y, 0, 0);
			else return new Rect(offset.x, offset.y, icon.width, icon.height);
		}
	}
	
}

public struct CursorPriority {
	
	public const int LOW		= 1;
	public const int MEDIUM		= 2;
	public const int HIGH		= 3;
	public const int BUSY		= 99;
	
}

public class CursorPriorityPair {
	
	public Cursor cursor;
	public int priority = 1;
	
	public CursorPriorityPair(Cursor cursor) {
		this.cursor = cursor;
	}
	
	public CursorPriorityPair(Cursor cursor, int priority) {
		this.cursor = cursor;
		this.priority = priority;
	}
	
}