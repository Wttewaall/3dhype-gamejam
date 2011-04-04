using UnityEngine;
using System.Collections;

// Multithread safe Singleton
public sealed class GUIModel {
	
    static readonly GUIModel _instance = new GUIModel();

    static GUIModel() {}
    GUIModel() {}

    public static GUIModel instance {
        get {
            return _instance;
        }
    }
	
}