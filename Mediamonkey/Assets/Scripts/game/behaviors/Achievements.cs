using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

[AddComponentMenu("King's Ruby/Behaviors/Achievements")]

public class Achievements : MonoBehaviour {
    
	public Texture2D background;
	public AudioClip unlockedSound;
	
    protected List<Achievement> list = new List<Achievement>();
	protected Queue<Achievement> queue = new Queue<Achievement>();
	protected Achievement current;
	
	protected Rect backgroundRect;
	protected Rect textRect;
	protected bool showing = false;
	protected Vector2 popupOffscreen;
	protected Vector2 popupOnscreen;
	protected Vector2 popupOffset;
	
	// ---- inherited handlers ----
	
	void Awake() {
		Setup();
	}
	
	void Start() {
		
		// add achievements to list
		list.Add(new Achievement01());
		list.Add(new Achievement02());
		list.Add(new Achievement03());
		
		// add event listener
		Statistics.propertyChange += statisticsChangeHandler;
	}
	
	void OnGUI() {
		if (!showing) return;
		
		GUI.DrawTexture(new Rect(backgroundRect.x + popupOffset.x, backgroundRect.y + popupOffset.y, backgroundRect.width, backgroundRect.height), background);
		GUI.Label(new Rect(textRect.x + popupOffset.x, textRect.y + popupOffset.y, textRect.width, textRect.height), "Achievement unlocked\n"+current.name+"\n"+current.description);
	}
	
	// ---- protected methods ----
	
	// positions for the popup elements
	protected void Setup() {
		
		popupOffscreen = new Vector2(0, -background.height);
		popupOnscreen = new Vector2(0, 0);
		popupOffset = popupOffscreen;
		
		float x = (Screen.width-background.width)/2;
		float y = 10;
		float w = background.width;
		float h = background.height;
		backgroundRect = new Rect(x, y, w, h);
		
		x += 70;
		y += 5;
		w -= 90;
		h -= 10;
		textRect = new Rect(x, y, w, h);
	}
	
	protected List<Achievement> GetByTag(int tag) {
		List<Achievement> result = new List<Achievement>();
		
		foreach (Achievement a in list) {
			if (a.tags.Contains(tag)) result.Add(a);
		}
		
		return result;
	}
	
	protected void Show() {
		if (queue.Count == 0 || showing || gameObject == null) return;
		
		// set current Achievement
		current = queue.Dequeue();
		
		// play animation
		Hashtable valueHash = iTween.Hash(
			"from", popupOffscreen,
			"to", popupOnscreen,
			"time", 2,
			"onUpdate", "animatePopupOffset",
			"onComplete", "animatePopupInComplete",
			"easeType", iTween.EaseType.bounce
		);
		
		showing = true;
		iTween.ValueTo(gameObject, valueHash);
		audio.PlayOneShot(unlockedSound);
	}
	
	// animate back after some delay
	protected void Hide() {
		
		Hashtable valueHash = iTween.Hash(
			"from", popupOnscreen,
			"to", popupOffscreen,
			"time", 2,
			"delay", 2,
			"onUpdate", "animatePopupOffset",
			"onComplete", "animatePopupOutComplete",
			"easeType", iTween.EaseType.easeInSine
		);
		
		iTween.ValueTo(gameObject, valueHash);
	}
	
	// ---- tween handlers ----
	
	protected void animatePopupOffset(Vector2 offset) {
		popupOffset = offset;
	}
	
	protected void animatePopupInComplete() {
		Hide();
	}
	
	protected void animatePopupOutComplete() {
		showing = false;
		current = null;
		
		// if more in queue, show another achievement
		if (queue.Count > 0) Show();
	}
	
	// ---- event handlers ----
	
	protected void statisticsChangeHandler(int flag) {
		// unlocking multiple achievements at once,
		// or when already showing a notification of one is possible
		
		List<Achievement> result = new List<Achievement>();
		
		bool change = false;
		
		foreach (Achievement a in list) {
			
			if (a.unlocked == false && a.tagInFlag(flag)) {
				a.unlocked = a.validate();
				
				if (a.unlocked) result.Add(a);
				change = change || a.unlocked;
			}
		}
		
		if (change) {
			// add to queue
			foreach (Achievement a in result) {
				queue.Enqueue(a);
			}
			
			// show until queue is empty (often just 1 item)
			if (!showing) Show();
		}
		
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
		description = "Hit a target";
		tags.Add(StatisticTag.TARGET_HIT);
	}
	
	override public bool validate() {
		return (Statistics.targetsHit >= 1);
	}
	
}

public class Achievement03 : Achievement {
	
	public Achievement03() {
		name = "Fire in the hole!";
		description = "Fire 20 bullets";
		tags.Add(StatisticTag.BULLET_FIRED);
	}
	
	override public bool validate() {
		return (Statistics.bulletsFired >= 20);
	}
	
}