/*-----------------------------------------------------------------------------
 * Scoring.cs - Keeps track of the player's score and levels up as required.
 * Copyright (C) 2010 Justin Lloyd
 * http://www.otakunozoku.com/
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU lesser General Public License
 * along with this library.  If not, see <http://www.gnu.org/licenses/>.
 *
-----------------------------------------------------------------------------*/

using UnityEngine;
//using System; // remove?
using System.Collections;
//using System.Text; // remove?

/**
 * TODO:
 * . remove MonoBehavior dependency?
 * . use events to trigger sounds in some other manager (keep it clean)
 */

[RequireComponent(typeof(AudioSource))]
public class Scoring : MonoBehaviour {
    
    // Audio clips to play for each level up sound.
    public AudioClip[] nextLevelSound;
	
    // Maximum permitted level.
    public int maxLevel = 100;
	
    // The list of scores required to advance to the next level.
    public int[] levelScores = { 0, 3000, 7000, 12000, 18000, 25000, 34000, 44000, 56000, 69000, 80000 };
	
    // The number of required points to score to advance to the next level once the score has gone beyond the provided list of points.
    public int nextLevelScoreProgression = 100000;
	
	// The player's current score.
    public int score = 0;
	
    // The player's current level.
    private int _level = 1;
	
    // The minimum level permitted.
    private const int minLevel = 1;
	
    // Adjust the score by the specified number of points. Negative values will subtract points.
    public void adjustScore(int value) {
        score += value;
    }
	
	// Adjust the current level by the specified number of levels. Negative
    // values will subtract levels. Does not adjust the score to match. The
    // new level will be clamped to within the maximum permitted level.
    public void adjustLevel(int value) {
        level = Mathf.Clamp(level + value, minLevel, maxLevel);
    }
	// The player's current level. Specifying a new level will ensure that the
    // new level is clamped to the maximum permitted level.
    public int level {
        get {
            return _level;
        }
		
        set {
            _level = Mathf.Clamp(value, minLevel, maxLevel);
        }
    }
	
    // Play the audio for level up sound.
    public void playNextLevelSound() {
        int levelUpIndex = Mathf.Clamp(level, minLevel, nextLevelSound.Length - 1) - 1;
        if (nextLevelSound[levelUpIndex] == null) return;
        this.audio.PlayOneShot(nextLevelSound[levelUpIndex]);
    }
	
    // Checks for completion of the current level and advances to the next level if the score is high enough.
    public virtual void checkForLevelUp() {
        // if we have reached the maximum level, do nothing
        if (level >= maxLevel) return;
		
        // check for the next required score
        int nextScore = 0;
       
        // if there are no more scores in the level score progression array
        if (level >= levelScores.Length) {
        	
        	//switch over to linear progression
            nextScore = (level - levelScores.Length + 1) * nextLevelScoreProgression;
            
        } else {
        	// otherwise use the non-linear progression
            nextScore = levelScores[level];
        }
		
        // if we have the required score to level up, advance to the next level
        if (score >= nextScore) {
            level = Mathf.Min(level + 1, maxLevel);
            playNextLevelSound();
        }
    }

    void Update() {
        checkForLevelUp();
    }
	
}