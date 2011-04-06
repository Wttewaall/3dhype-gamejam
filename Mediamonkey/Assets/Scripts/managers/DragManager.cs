using UnityEngine;
using System.Collections;

public static class DragManager {
	
	public static GameObject dragProxy;
	
	// raycast properties
	public static float				maxDistance = 20.0f;
	public static LayerMask			layer;
	
	// drag properties
	private static bool				_dragging;
	//private static GameObject		dragInitiator;
	//private static GameObject		dragSource;
	//private static Bounds			dragSourceBounds;
	
	// temporary properties
	private static Ray				ray;
	private static RaycastHit[]		hits;
	private static RaycastHit		hit;
	private static RaycastDistanceSorter sorter;
	
	// ---- EventHandler & events ----
	
	public delegate void EventHandler(RaycastHit[] hits);
	
	public static event EventHandler dragUpdate;
	
	// ----
	
	/** Read-only property that returns true if a drag is in progress. */
	public static bool isDragging() {
		return _dragging;
	}
	
	/** Call this method from your dragEnter event handler if you accept the drag/drop data. */
	public static void acceptDragDrop(GameObject target) {
	}
	
	/** Initiates a drag and drop operation. */
	public static void doDrag(GameObject dragInitiator, GameObject dragSource, Event mouseEvent, Texture2D dragImage, Vector2 offset, float imageAlpha, bool allowMove) {
		_dragging = true;
		
		//DragManager.dragInitiator = dragInitiator;
		//DragManager.dragSource = dragSource;
		
		//dragSourceBounds = dragSource.collider.bounds;
		
		// use to mouse events to drag and drop the dragSource
		MouseManager.mouseDragMove += mouseMoveHandler;
		MouseManager.mouseDragStop += mouseDragStopHandler;
		
		// show dragImage as proxy for the dragSource
		//..
		
		if (sorter == null) sorter = new RaycastDistanceSorter();
	}
	
	/* Returns the current drag and drop feedback.
	public static string getFeedback() {
		return "";
	}
	
	// Sets the feedback indicator for the drag and drop operation.
	public static void showFeedback(string feedback) {
	}*/
	
	// ---- protected methods ----
	
	public static void mouseMoveHandler(int buttonID) {
		Debug.Log("DragManager.mouseMoveHandler");
		
		// Raycast from mouse position
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		hits = Physics.RaycastAll(ray, maxDistance, layer);
		
		// sort the result on distance
		System.Array.Sort(hits, sorter);
		
		// dispatch event
		dragUpdate(hits);
	}
	
	public static void mouseDragStopHandler(int buttonID) {
		MouseManager.mouseDragMove -= mouseMoveHandler;
		MouseManager.mouseDragStop -= mouseDragStopHandler;
	}
	
}

class RaycastDistanceSorter : IComparer {
	
    int IComparer.Compare(System.Object a, System.Object b) {
		if (!(a is RaycastHit && b is RaycastHit)) return 0;
		
        RaycastHit raycastHitA = (RaycastHit) a;
        RaycastHit raycastHitB = (RaycastHit) b;
		return raycastHitA.distance.CompareTo(raycastHitB.distance);
    }
}