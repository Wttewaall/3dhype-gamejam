using System;
using System.Collections;

public class Statistics {
    
	protected static int changeFlag;
	
	// ---- events ----
	
	public delegate void EventHandler(int flags);
	
	public static event EventHandler propertyChange;
	
	// ---- protected methods ----
    
	// collect changes by tag and dispatch event in time
	// the listener is responsible for getting the changed values
	protected static void update(int tag) {
		
		// collect tags
		changeFlag |= tag;
		
		// notify every <time> ms?
		if (true) {
			
			if (propertyChange != null) propertyChange(changeFlag);
			changeFlag = 0; // reset
		}
	}
	
	// ---- public methods ----
	
	// fill Statistics with encoded data from the server
	public static void setStatistics(string encodedString) {
		//..
	}
	
	// return encoded statistics to send to the server
	public static string getStatistics() {
		//..
		return "";
	}
	
	// ---- getters & setters ----
	
	private static int _bulletsFired;
	
    public static int bulletsFired {
    	get {
			return _bulletsFired;
		}
    	set {
			_bulletsFired = value;
			update(StatisticTag.BULLET_FIRED);
		}
    }
	
	private static int _targetsHit;
	
    public static int targetsHit {
    	get {
			return _targetsHit;
		}
    	set {
			_targetsHit = value;
			update(StatisticTag.TARGET_HIT);
		}
    }
	
}

public struct StatisticTag {
	
	public const byte BULLET_FIRED		= 1 << 0;
	public const byte TARGET_HIT		= 1 << 1;
	// more...
	
}