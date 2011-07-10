using UnityEngine;
using System;
using System.Collections.Generic;

[AddComponentMenu("King's Ruby/Behaviors/TriggerTrap")]

public class TriggerTrap : MonoBehaviour {
	
	public HitStats hitStats;
	public List<TriggerEffect> triggerEffects;
	
	protected bool triggered;
	
	void Start () {
	
	}
}

[Serializable]
public class TriggerEffect {
	
	public float radius				= 3; // area of effect
	public float duration			= 0; // 0 = burst
	public AffectMode affectMode	= AffectMode.Triggerer;
	
}

public enum AffectMode {
	Triggerer,
	Nearest,
	All
}