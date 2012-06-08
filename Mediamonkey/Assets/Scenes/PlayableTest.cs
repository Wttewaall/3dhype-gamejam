using UnityEngine;
using System.Collections.Generic;

public class PlayableTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		var list = new List<IPlayable>();
		
		/*//
		list.Add(new PlayableItem());
		list.Add(new PlayableItem());
		list.Add(new PlayableItem());
		list.Add(new PlayableItem());
		//*/
		
		//*//
		list.Add(new Wave(1));
		list.Add(new Wave(2));
		list.Add(new Wave(3));
		list.Add(new Wave(4));
		//*/
		
		var collection = new PlayableCollection();
		collection.OnPlaying += delegate(IPlayable sender) {
			Utils.trace("Playing:", collection.selectedIndex);
		};
		collection.OnItemPlaying += delegate(IPlayable sender, IPlayable item) {
			Utils.trace("- Item playing:", item);
		};
		collection.OnComplete += delegate(IPlayable sender) {
			Utils.trace("Complete:", collection.selectedIndex);
		};
		
		collection.AddItems(list);
		Utils.trace(collection);
		Utils.trace("Play:", collection.Play());
		
	}
	
}
