using UnityEngine;
using System.Collections;

public class IniTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var file = new IniFile("config.cfg");
		Utils.trace(file.GetValue("owner", "name"));
		Utils.trace(file.GetValue("owner", "organization"));
		Utils.trace(file.GetValue("database", "server"));
		Utils.trace(file.GetValue("database", "port"));
		Utils.trace(file.GetValue("database", "file"));
	}
}
