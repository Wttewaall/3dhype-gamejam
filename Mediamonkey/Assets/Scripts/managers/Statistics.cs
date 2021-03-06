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

[Serializable]
public class Statistics {
    
	// use int: 32 bits means 32 possible flags (use long for 64 bits)
	// if this is insufficient, use multiple flags! or specify different flags per property type
	protected static int changeFlags;
	
	// ---- events ----
	
	public delegate void PropertyChangeEvent(int flags);
	
	public static event PropertyChangeEvent propertyChange;
	
	// ---- protected methods ----
    
	// collect changes by tag and dispatch event in time
	// the listener is responsible for getting the changed values
	protected static void Update(int tag) {
		
		// collect tags
		changeFlags |= tag;
		
		// TODO: notify every <time> ms?
		if (true) {
			
			DispatchPropertyChangeEvent(propertyChange);
			changeFlags = 0; // reset
		}
	}
	
	// ---- public methods ----
	
	// fill Statistics with encoded data from the server
	public static void SetData(string encodedString) {
		// decode data
		// set properties
		// set achievements by firing a change event with flags
		Update(0);
	}
	
	// return encoded statistics to send to the server
	public static string GetData() {
		//..
		return "";
	}
	
	// ---- private methods ----
	
	private static void DispatchPropertyChangeEvent(PropertyChangeEvent evt) {
		if (evt != null) evt(changeFlags);
	}
	
}