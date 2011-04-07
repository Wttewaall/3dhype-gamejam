using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Achievements : MonoBehaviour {
    
	public Texture2D background;
	
    public List<Achievement> list = new List<Achievement>();
	
	// ---- inherited handlers ----
	
	void Start() {
		
		// add achievements to list
		list.Add(new Achievement01());
		list.Add(new Achievement02());
		
		// add event listener
		Statistics.propertyChange += statisticsChangeHandler;
	}
	
	// ---- protected methods ----
	
	protected List<Achievement> getByTag(int tag) {
		List<Achievement> result = new List<Achievement>();
		
		foreach (Achievement a in list) {
			if (a.tags.Contains(tag)) result.Add(a);
		}
		
		return result;
	}
	
	// ---- event handlers ----
	
	protected void statisticsChangeHandler(int flag) {
		List<Achievement> result = new List<Achievement>();
		
		bool change = false;
		
		foreach (Achievement a in list) {
			
			if (a.unlocked == false && a.tagInFlag(flag)) {
				a.unlocked = a.validate();
				
				change = change || a.unlocked;
				if (change) result.Add(a);
			}
		}
		
		if (change) {
			Debug.Log("achievements unlocked: "+result.Count);
			foreach (Achievement a in result) Debug.Log(a.name);
			show(result[0]);
		}
	}
	
	// ---- public methods ----
	
	protected GameObject go;
	
	public void show(Achievement a) {
		createGO();
		
		go.guiText.text = a.name;
		
		// from position
		float x = (Screen.width-background.width)/2;
		float y = -background.height;
		Vector3 v1 = Camera.main.ScreenToViewportPoint(new Vector3(x, y, 0));
		
		// to position
		y = background.height + 50;
		Vector3 v2 = Camera.main.ScreenToViewportPoint(new Vector3(x, y, 0));
		
		go.transform.position = v2;
		//go.transform.position = v1;
		//iTween.MoveTo(go, iTween.Hash("position", v2, "time", 4, "delay", 1));
	}
	
	public void hide() {
		if (go) DestroyImmediate(go);
	}
	
	protected void createGO() {
		if (!go) {
			go = new GameObject("AchievementGO");
			go.hideFlags = HideFlags.HideAndDontSave;
			go.transform.position = Vector3.zero;
			
			GUITexture bg = go.AddComponent<GUITexture>();
			bg.texture = background;
			bg.pixelInset = new Rect(-bg.texture.width/2, -bg.texture.height/2, bg.texture.width, bg.texture.height);
			
			GUIText text = go.AddComponent<GUIText>();
			text.pixelOffset = new Vector2(-50, 10);
		}
	}
	
	public void OnDisable () {
		if (go) DestroyImmediate(go);
	}
	
}

// ---- base Achievement class ----

public class Achievement {
	
	public string name;
	public string description;
	public Texture2D texture;
	public List<int> tags = new List<int>();
	public bool unlocked;
	
	public Achievement() {}
	
	public virtual bool validate() {
		return false;
	}
	
	public bool tagInFlag(int flag) {
		for (int i=0; i<tags.Count; i++) {
			int result = flag & tags[i];
			if (result > 0) return true;
		}
		return false;
	}
	
}

// ---- Custom Achievements ----

public class Achievement01 : Achievement {
	
	public Achievement01() {
		name = "Ammo galore";
		description = "Fire 10 bullets";
		tags.Add(StatisticTag.BULLET_FIRED);
	}
	
	override public bool validate() {
		return (Statistics.bulletsFired >= 10);
	}
	
}

public class Achievement02 : Achievement {
	
	public Achievement02() {
		name = "Punisher";
		description = "Hit 20 targets";
		tags.Add(StatisticTag.TARGET_HIT);
	}
	
	override public bool validate() {
		return (Statistics.targetsHit >= 20);
	}
	
}