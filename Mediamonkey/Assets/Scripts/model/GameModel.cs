using UnityEngine;
using System.Collections;

// Multithread safe Singleton
public sealed class GameModel {
	
    static readonly GameModel _instance = new GameModel();

    static GameModel() {}
    GameModel() {}

    public static GameModel instance {
        get {
            return _instance;
        }
    }
	
	public bool enabled = true;
	public bool clickEnabled = true;
	
}