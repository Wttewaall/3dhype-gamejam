using UnityEngine;
using System;
using System.Collections;

[AddComponentMenu("King's Ruby/Behaviors/Healthbar")]

public class Healthbar : MonoBehaviour {
	
	public Texture2D background;
	public Texture2D foreground;
	
	protected Texture2D main;
	protected Color[] bgPixels;
	protected Color[] fgPixels;
	
	// ---- getters & setters ----
	
	private float _health;
	
	public float health {
		get {
			return _health;
		}
		set {
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
		//TweenHealth(0.0f);
	}
	
	// ---- public methods ----
	
	public void TweenHealth(float value) {
		iTween.ValueTo(gameObject,
			iTween.Hash(
				"from", health,
				"to", value,
				"time", 1,
				"onupdate", "TweenUpdateHandler"
			)
		);
	}
	
	// ---- private methods ----
	
	private void DrawTexture() {
		main.SetPixels(bgPixels);
		main.SetPixels(0, 0, (int) (foreground.width * _health), foreground.height, fgPixels);
		main.Apply(false);
	}
	
	private void DrawTexture2() {
		
		Color[] pixels = new Color[main.width * main.height];
		int level = (int) (main.width * _health);
		
		int x = 0;
		int y = 0;
		int index = 0;
		
		// TODO
		// keep the Color[] and only draw changed pixels (calculate rect)
		
		for (x=0; x<main.width; x++) {
			
			for (y=0; y<main.height; y++) {
				index = x + y * main.width;
				
				if (x <= level) pixels[index] = foreground.GetPixel(x, y);
				else pixels[index] = background.GetPixel(x, y);
			}
		}
		
		main.SetPixels(pixels);
		main.Apply(false);
	}
	
	private void TweenUpdateHandler(float val) {
		health = val;
	}
	
}
