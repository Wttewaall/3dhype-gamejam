using UnityEngine;
using System;

public class Level2 : Level {
	
	override public void Setup() {
		// create a round
		var round = CreateRound();
		
		// create a wave with multiple enemy groups
		var wave = round.CreateWave(spawners[0], goals[0]);
		wave.CreateGroup(EnemyType.RUNNER, 4, 0);
		wave.CreateGroup(EnemyType.ORC, 4, 0);
		wave.CreateGroup(EnemyType.ARCHER, 4, 0);
	}
	
}
