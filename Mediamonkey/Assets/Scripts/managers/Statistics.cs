using UnityEngine;
using System;
using System.Collections;

/**
 * This class dispatches a change event, carrying a bit flag composed of StatisticsTag bits
 * 
 * This can be used as a crude binding system:
 * A class can listen to the change event and when a certain bit is set
 * within the flag the corresponding property can be retrieved.
 **/

public class Statistics {
    
	// use int: 32 bits means 32 possible flags (use long for 64 bits)
	// if this is insufficient, use multiple flags! or specify different flags per property type
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
		
		// TODO: notify every <time> ms?
		if (true) {
			
			if (propertyChange != null) propertyChange(changeFlag);
			changeFlag = 0; // reset
		}
	}
	
	// ---- public methods ----
	
	// fill Statistics with encoded data from the server
	public static void setData(string encodedString) {
		// decode data
		// set properties
		// set achievements by firing a change event with flags
		update(0);
	}
	
	// return encoded statistics to send to the server
	public static string getData() {
		//..
		return "";
	}
	
	// ---- getters & setters ----
	
	private static int _bulletsFired;
	
    public static int bulletsFired {
    	get { return _bulletsFired; }
    	set { _bulletsFired = value;
			update(StatisticTag.BULLET_FIRED);
		}
    }
	
	private static int _targetsHit;
	
    public static int targetsHit {
    	get { return _targetsHit; }
    	set { _targetsHit = value;
			update(StatisticTag.TARGET_HIT);
		}
    }
	
	// more...
	
}

public struct StatisticTag {
	
	public const int BULLET_FIRED		= 1 << 0;
	public const int TARGET_HIT			= 1 << 1;
	// more...
	
}