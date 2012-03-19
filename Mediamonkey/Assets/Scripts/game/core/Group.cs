using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Group {
	
	public EnemyType type;
	public int amount = 1;
	public float delay = 0;
	
	[NonSerializedAttribute]
	public Spawner spawner;
	
	public Group(EnemyType type, int amount) {
		this.type = type;
		this.amount = amount;
	}
	
}
