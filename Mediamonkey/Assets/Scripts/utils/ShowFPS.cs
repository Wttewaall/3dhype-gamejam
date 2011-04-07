using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class ShowFPS : MonoBehaviour {
	
	private GUIText		label;
	private float		updateInterval = 1.0f;
	private float		lastInterval; // Last interval end time
	private int			frames = 0; // Frames over current interval
	
	public Vector2		pixelOffset = new Vector2(5, 20);
	
	// ---- inherited handlers ----
	
	public void Start() {
	    lastInterval = Time.realtimeSinceStartup;
	    frames = 0;
	}
	
	public void Update() {
	    // count frames
		++frames;
		
	    float timeNow = Time.realtimeSinceStartup;
	    if (timeNow > lastInterval + updateInterval) {
	    	
			// create possibly missing label
			if (!label) createLabel();
			
			// calculate & set text
	        float fps = frames / (timeNow - lastInterval);
			float ms = 1000.0f / Mathf.Max(fps, 0.00001f);
			label.text = "FPS: "+fps.ToString("f2")+"  ("+ms.ToString("f1")+"ms)";
	        
			// reset
			frames = 0;
	        lastInterval = timeNow;
	    }
	}
	
	public void OnEnable () {
		if (!label) createLabel();
	}
	
	public void OnDisable () {
		if (label) DestroyImmediate(label.gameObject);
	}
	
	// ---- protected methods ----
	
	protected void createLabel() {
		if (!label) {
			GameObject go = new GameObject("ShowFPS");
			go.hideFlags = HideFlags.HideAndDontSave;
			go.transform.position = new Vector3(0,0,0);
			
			go.AddComponent<GUIText>();
			label = go.guiText;
			label.pixelOffset = this.pixelOffset;
		}
	}
	
}