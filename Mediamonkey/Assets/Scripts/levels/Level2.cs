using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Level2 : Level {
	
	override public void Setup() {
		// create a round
		var round = CreateRound();
		
		// create a wave with multiple enemy groups
		var wave = CreateWave(round);
		wave.CreateGroup(EnemyType.RUNNER, 4, 0);
		wave.CreateGroup(EnemyType.ORC, 4, 0);
		wave.CreateGroup(EnemyType.ARCHER, 4, 0);
	}
	
	public Round CreateRound() {
		var round = new Round();
		SetEventHandlers(round, true);
		
		if (rounds == null) rounds = new List<Round>();
		rounds.Add(round);
		
		round.index = rounds.Count - 1;
		return round;
	}
	
	public Wave CreateWave(Round round) {
		// lookup pool by type
		Wave wave = new Wave(round.waves.Count);
		round.waves.Add(wave);
		return wave;
	}
	
}
