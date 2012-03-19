using UnityEngine;
using System;

[Serializable]
public class Group {
	
	public EnemyType type;
	public int amount;
	public Spawner spawner;
	
	public Group(EnemyType type, int amount) {
		this.type = type;
		this.amount = amount;
	}
	
}
