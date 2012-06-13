using UnityEngine;
using System;
using System.Collections;

[AddComponentMenu("King's Ruby/Behaviors/Healthbar")]

//[ExecuteInEditMode]
public class Healthbar : MonoBehaviour {
	
	public Texture2D background;
	public Texture2D foreground;
	
	protected Texture2D main;
	protected Color[] bgPixels;
	protected Color[] fgPixels;
	
	// ---- getters & setters ----
	
	[SerializeField] 
	private float _health = 1;
	
	public float health {
		get {
			return _health;
		}
		set {
			if (_health == value) return;
			_health = Mathf.Clamp(value, 0, 1);
			DrawTexture2();
		}
	}
	
	// ---- inherited handlers ----
	
	void Start () {
		main = renderer.material.mainTexture as Texture2D;
		main = new Texture2D(main.width, main.height, TextureFormat.RGBA32, false);
		main.filterMode = FilterMode.Point;
		renderer.material.mainTexture = main;
		
		bgPixels = background.GetPixels();
		fgPixels = foreground.GetPixels();
		
		health = 1;
		TweenHealth(0, 5, 1);
	}
	
	void Update() {
		#if UNITY_EDITOR
			DrawTexture2();
		#endif
	}
	
	// ---- public methods ----
	
	public void TweenHealth(float value) {
		TweenHealth(value, 1, 0);
	}
	
	public void TweenHealth(float value, float time) {
		TweenHealth(value, time, 0);
	}
	
	public void TweenHealth(float value, float time, float delay) {
		iTween.ValueTo(gameObject,
			iTween.Hash(
				"from", health,
				"to", value,
				"time", time,
				"delay", delay,
				"onUpdate", "tweenUpdateHandler"
			)
		);
	}
	
	// ---- private methods ----
	
	private void DrawTexture1() {
		var w = (int) (main.width * health);
		var h = main.height;
		
		Color[] px = new Color[w * h];
		
		int i = 0;
		// copy portion of pixels
		for (int row = 0; row < h; row++) {
			for (int col = 0; col < w; col++) {
				px[i++] = fgPixels[row * main.width + col];
			}
		}
		
		main.SetPixels(bgPixels);
		main.SetPixels(0, 0, w, h, px);
		main.Apply(false);
	}
	
	private void DrawTexture2() {
		
		Color[] pixels = new Color[main.width * main.height];
		int level = (int) (main.width * health);
		
		int x = 0;
		int y = 0;
		int index = 0;
		
		// TODO
		// keep the Color[] and only draw changed pixels (calculate rect)
		
		for (x=0; x<main.width; x++) {
			
			for (y=0; y<main.height; y++) {
				index = x + y * main.width;
				
				if (level > 0 && x <= level) pixels[index] = foreground.GetPixel(x, y);
				else pixels[index] = background.GetPixel(x, y);
			}
		}
		
		main.SetPixels(pixels);
		main.Apply(false);
	}
	
	private void tweenUpdateHandler(float val) {
		health = val;
	}
	
}
