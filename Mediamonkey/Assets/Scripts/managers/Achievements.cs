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
				if (a.unlocked) result.Add(a);
				
				change = change || a.unlocked;
			}
		}
		
		if (change) {
			Debug.Log("achievements unlocked: "+result.Count);
			foreach (Achievement a in result) Debug.Log(a.name);
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
		description = "Hit 20 targets";
		tags.Add(StatisticTag.TARGET_HIT);
	}
	
	override public bool validate() {
		return (Statistics.targetsHit >= 20);
	}
	
}