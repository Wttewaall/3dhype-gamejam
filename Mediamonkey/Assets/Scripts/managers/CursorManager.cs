using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour {

	/**
	 * TODO
	 * . draw cursor as texture directly in screen instead of creating a GameObject:
	 * 		public function OnGUI() {
	 * 			GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursorSize.x, cursorSize.y), cursorImage);
	 * 		}
	 * . add add/deleteCursor(name, texture)
	 * . add new cursors to an asset bundle or something instead of defining them in setCursor()
	 **/
	
	public Vector2 cursorSize = new Vector2(32, 32);
	
	// cursor textures
	public Texture2D noCursor;
	public Texture2D defaultCursor;
	public Texture2D busyCursor;
	public Texture2D overCursor;
	public Texture2D dragCursor;
	public Texture2D dropValidCursor;
	public Texture2D dropInvalidCursor;
	
	protected ArrayList priorityArray;
	protected bool follow;
	
	// -- stored enabled flags
	private bool _enabledReticleVisibility;
	private bool _enabledScreenCursorVisibility;
	
	// ---- getters & setters ----
	
	private static CursorManager _instance;
	private GUITexture _reticle;
	private Texture2D _clearTexture;
	
	public static CursorManager getInstance() {
		return _instance;
	}
	
	public GUITexture reticle {
		get {
			if (!_reticle) {
				
				GameObject go = new GameObject("reticle");
				go.hideFlags = HideFlags.HideAndDontSave;
				go.transform.position = Vector3.zero;
				
				_reticle = go.AddComponent<GUITexture>();
				_reticle.pixelInset = new Rect(-cursorSize.x/2, -cursorSize.y/2, cursorSize.x, cursorSize.y);
				_reticle.transform.position = Vector3.zero;
				_reticle.transform.localScale = Vector3.zero;
				
				// init
				if (_reticle.texture == null) {
					setCursor("default");
					_reticle.enabled = _enabledReticleVisibility = false;
				}
			}
			return _reticle;
		}
	}
	
	// creates a filled transparent Texture2D
	public Texture2D clearTexture {
		get {
			if (!_clearTexture) {
				
				if (noCursor) {
					_clearTexture = noCursor;
					
				} else {
					_clearTexture = new Texture2D((int) cursorSize.x, (int) cursorSize.y);
					
					int w = (int) cursorSize.x;
					while (w-- > 0) {
						
						int h = (int) cursorSize.y;
						while (h-- > 0) {
							_clearTexture.SetPixel(w, h, Color.clear);
						}
					}
				}
				
			}
			return _clearTexture;
		}
	}
	
	public bool reticleVisible {
		get {
			return _enabledReticleVisibility;
		}
	}
	
	public bool screenCursorVisible {
		get {
			return _enabledScreenCursorVisibility;
		}
	}
	
	public Vector3 mouseScreenPosition {
		get {
			float x = Input.mousePosition.x / Screen.width;
			float y = Input.mousePosition.y / Screen.height;
			return new Vector3(x, y, 0);
		}
	}
	
	// ---- inherited handlers ----
	
	void Awake() {
		if (_instance != null) throw new UnityException("You can have only one instance of this component");
		else _instance = this;
		
		priorityArray = new ArrayList();
	}
	
	void Update() {
		if (follow && enabled) positionAt(mouseScreenPosition);
	}
	
	void OnEnable() {
		reticle.enabled = _enabledReticleVisibility;
		Screen.showCursor = _enabledScreenCursorVisibility;
	}
	
	void OnDisable () {
		reticle.enabled = false;
		Screen.showCursor = true;
	}
	
	/*public function OnApplicationQuit() {
		if (_reticle) DestroyImmediate(_reticle.gameObject);
	}*/
	
	// ---- public methods ----
	
	public void setCursor(string cursorName) {
		setCursor(cursorName, CursorPriority.LOW);
	}
	
	public void setCursor(string cursorName, int priority) {
		Texture2D cursor = null;
		
		switch (cursorName) {
			case "default":			cursor = defaultCursor;		break;
			case "normal":			cursor = defaultCursor;		break;
			case "busy":			cursor = busyCursor;		break;
			case "over":			cursor = overCursor;		break;
			case "drag":			cursor = dragCursor;		break;
			case "dragValid":		cursor = dropValidCursor;	break;
			case "dragInvalid":		cursor = dropInvalidCursor;	break;
			case "none":			cursor = clearTexture;		break;
			default:				cursor = null;				break;
		}
		
		if (cursor != null) {
			PriorityItem item = getPriorityItem(priority);
			
			if (item == null) {
				// add cursor with new priority
				item = new PriorityItem(cursorName, cursor, priority);
				priorityArray.Add(item);
				
			} else {
				// overwrite cursor with same priority
				item.name = cursorName;
				item.texture = cursor;
			}
		}
		
		commitCursor();
	}
	
	public void removeCursor(string cursorName) {
		PriorityItem item = getPriorityItem(cursorName);
		if (item == null) return;
		
		priorityArray.Remove(item);
		commitCursor();
	}
	
	public void removeCursor(int priority) {
		PriorityItem item = getPriorityItem(priority);
		if (item == null) return;
		
		priorityArray.Remove(item);
		commitCursor();
	}
	
	public void removeAllCursors() {
		priorityArray.Clear();
		commitCursor();
	}
	
	public void setBusyCursor() {
		setCursor("busy", CursorPriority.BUSY);
	}
	
	public void removeBusyCursor() {
		removeCursor(CursorPriority.BUSY);
	}
	
	public void showReticle() {
		_enabledReticleVisibility = true;
		if (enabled) reticle.enabled = _enabledReticleVisibility;
	}
	
	public void showReticle(bool withScreenCursor) {
		showReticle();
		
		if (withScreenCursor) showScreenCursor();
		else hideScreenCursor();
	}
	
	public void hideCursor() {
		_enabledReticleVisibility = false;
		if (enabled) reticle.enabled = _enabledReticleVisibility;
	}
	
	public void showScreenCursor() {
		Screen.showCursor = _enabledScreenCursorVisibility = true;
	}
	
	public void hideScreenCursor() {
		Screen.showCursor = _enabledScreenCursorVisibility = false;
	}
	
	public void followMouse(bool value) {
		follow = value;
	}
	
	public void positionAt(Vector3 position) {
		reticle.transform.position = position;
	}
	
	public void positionAtCenter() {
		follow = false;
		positionAt(new Vector3(0.5f, 0.5f, 0));
	}
	
	// ---- protected methods ----
	
	protected void commitCursor() {
		PriorityItem item = getHighestPriorityItem();
		reticle.texture = (item != null) ? item.texture : clearTexture;
	}
	
	protected PriorityItem getHighestPriorityItem() {
		PriorityItem highest = null;
		
		foreach (PriorityItem item in priorityArray) {
			highest = (highest == null || item.priority >= highest.priority) ? item : highest;
		}
		
		return highest;
	}
	
	protected PriorityItem getPriorityItem(int priority) {
		foreach (PriorityItem item in priorityArray) {
			if (item.priority == priority) return item;
		}
		return null;
	}
	
	protected PriorityItem getPriorityItem(string cursorName) {
		foreach (PriorityItem item in priorityArray) {
			if (item.name == cursorName) return item;
		}
		return null;
	}
}

public struct CursorPriority {
	
	public const int LOW = 1;
	public const int MEDIUM	 = 2;
	public const int HIGH = 3;
	public const int BUSY = 99;
	
}

public class PriorityItem {
	
	public string name;
	public Texture2D texture;
	public int priority;
	
	public PriorityItem(string name, Texture2D texture) {
		this.name = name;
		this.texture = texture;
		this.priority = 0;
	}
	
	public PriorityItem(string name, Texture2D texture, int priority) {
		this.name = name;
		this.texture = texture;
		this.priority = priority;
	}
	
}