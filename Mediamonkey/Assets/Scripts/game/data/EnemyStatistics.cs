using UnityEngine;
using System;

[Serializable]
public class EnemyStatistics {
	
	// standard
	public float health			= 200;
	public float armor			= 200;
	
	[NonSerializedAttribute]
	public float startHealth;
	
	[NonSerializedAttribute]
	public float startArmor;
	
	// scalar resistance to certain attacks
	public float resistDirectHit;
	public float resistSplashDamage;
	public float resistKnockdown;
	
	// mental state
	public float courage		= 50; // -courage = fear (sense of self-preservation)
	public float rage			= 0; // -rage = happiness/joy
	
	public EnemyStatistics() {
		startHealth = health;
		startArmor = armor;
	}
	
}