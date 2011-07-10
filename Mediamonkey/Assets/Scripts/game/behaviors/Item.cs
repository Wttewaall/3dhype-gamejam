using UnityEngine;
using System;

[AddComponentMenu("King's Ruby/Behaviors/Item")]

public class Item : MonoBehaviour {
	
	public bool useOnSelf;
	public bool useOnEnemy;
	public bool useOnAnything;
	
	// ---- public methods ----
	
	public void use(object target) {
		if (canUse(target)) useItem(target);
	}
	
	// ---- protected methods ----
	
	protected bool canUse(object target) {
		if (useOnAnything) return true;
		
		bool valid = true;
		
		if (useOnEnemy) valid = valid && (target is Enemy);
		if (useOnSelf) valid = valid && !(target is Enemy);
		
		return valid;
	}
	
	virtual protected void useItem(object target) {
		throw new UnityException("override this method");
		// handle target Type cases
	}
	
}