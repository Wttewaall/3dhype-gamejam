using UnityEngine;
using System;

[Serializable]
public class LevelStatistics : Statistics {

	public LevelStatistics() {
		//..
	}
	
	// ---- getters & setters ----
	
	private static int _bulletsFired;
	
    public static int bulletsFired {
    	get { return _bulletsFired; }
    	set { _bulletsFired = value;
			Update(StatisticTag.BULLET_FIRED);
		}
    }
	
	private static int _targetsHit;
	
    public static int targetsHit {
    	get { return _targetsHit; }
    	set { _targetsHit = value;
			Update(StatisticTag.TARGET_HIT);
		}
    }
	
	// more...
	
}