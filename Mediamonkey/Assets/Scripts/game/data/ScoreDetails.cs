using UnityEngine;

public class ScoreDetails {
	
	public int gatePoints;
	public int gatePointsLost;
	public int completionTime;
	public int parTime;
	public int trapKills;
	public int combatKills;
	public bool gateBonus;
	public int combos;
	public int killStreaks;
	public int biggestKillStreak;

	public int stars {
		get {
			float perc = gatePointsLost/gatePoints;
			int amount = 0;
			if (perc <= 0.25f) amount++;
			if (perc <= 0.50f) amount++;
			if (perc <= 0.75f) amount++;
			if (perc == 1.00f) amount++;
			if (completionTime <= parTime) amount++;
			return amount;
		}
	}
	
	public int totalScore {
		get {
			return 0;
		}
	}
	
	override public string ToString() {
		return "ScoreDetails";
	}
	
}
