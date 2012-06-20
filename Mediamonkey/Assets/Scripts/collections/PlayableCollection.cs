using System;
using System.Collections.Generic;

public class PlayableCollection : DataProvider<IPlayable>, IPlayable {
	
	// ---- implemented events ----
	
	public event PlayableEventHandler OnPlaying;
	public event PlayableEventHandler OnPaused;
	public event PlayableEventHandler OnResumed;
	public event PlayableEventHandler OnStopped;
	public event PlayableEventHandler OnReset;
	public event PlayableEventHandler OnComplete;
	
	public event PlayableCollectionEventHandler OnIndexChanged;
	public event PlayableCollectionEventHandler OnItemPlaying;
	public event PlayableCollectionEventHandler OnItemPaused;
	public event PlayableCollectionEventHandler OnItemResumed;
	public event PlayableCollectionEventHandler OnItemStopped;
	public event PlayableCollectionEventHandler OnItemReset;
	public event PlayableCollectionEventHandler OnItemComplete;
	
	// ---- getters & setters ----
	
	private IPlayable _currentItem;
	
	public IPlayable currentItem {
		get { return _currentItem; }
		
		private set {
			if (_currentItem != null) {
				_currentItem.OnPlaying	-= itemPlayingHandler;
				_currentItem.OnPaused	-= itemPausedHandler;
				_currentItem.OnResumed	-= itemResumedHandler;
				_currentItem.OnStopped	-= itemStoppedHandler;
				_currentItem.OnReset	-= itemResetHandler;
				_currentItem.OnComplete	-= itemCompleteHandler;
			}
			
			_currentItem = value;
			
			if (_currentItem != null) {
				_currentItem.OnPlaying	+= itemPlayingHandler;
				_currentItem.OnPaused	+= itemPausedHandler;
				_currentItem.OnResumed	+= itemResumedHandler;
				_currentItem.OnStopped	+= itemStoppedHandler;
				_currentItem.OnReset	+= itemResetHandler;
				_currentItem.OnComplete	+= itemCompleteHandler;
			}
			
			DispatchPlayableCollectionEvent(OnIndexChanged);
		}
	}
	
	private bool _isPlaying;
	
	public bool isPlaying {
		get { return _isPlaying; }
		private set {
			if (_isPlaying == value) return;
			_isPlaying = value;
		}
	}
	
	private bool _isPaused;
	
	public bool isPaused {
		get { return _isPaused; }
		private set {
			if (_isPaused == value) return;
			_isPaused = value;
		}
	}
	
	// ---- constructor ----
	
	public PlayableCollection():base() {
		Init();
	}
	
	public PlayableCollection(int capacity):base(capacity) {
		Init();
	}
	
	public PlayableCollection(object source):base(source) {
		Init();
	}
	
	protected void Init() {
		this.OnIndexChange += delegate {
			currentItem = selectedItem;
		};
	}
	
	// ---- public methods ----
	
	public bool Play() {
		return Play(0);
	}
	
	public bool Play(int index) {
		if (isPlaying || !isValidIndex(index)) return false;
		
		selectedIndex = index;
		
		isPlaying = true;
		isPaused = false;
		
		DispatchPlayableEvent(OnPlaying);
		currentItem.Play();
		
		return true;
	}

	public bool Pause() {
		if (!isPlaying || isPaused) return false;
		isPaused = currentItem.Pause();
		return isPaused;
	}
	
	public bool Resume() {
		if (!isPlaying || !isPaused) return false;
		isPaused = !currentItem.Resume();
		return !isPaused;
	}
	
	public bool Stop() {
		if (!isPlaying) return false;
		isPlaying = !currentItem.Stop();
		return !isPlaying;
	}
	
	public void Reset() {
		// reset items in dataProvider
		ForEach(delegate(IPlayable item, int index, DataProvider<IPlayable> collection) {
			item.Reset();
		});
		isPaused = isPlaying = false;
		selectedIndex = 0;
	}
	
	override public string ToString() {
		return "PlayableCollection ["+OutputToString()+"]";
	}
	
	// ---- event handlers ----
	
	protected void itemPlayingHandler(IPlayable sender) {
		DispatchPlayableCollectionEvent(OnItemPlaying);
		Play(GetItemIndex(sender));
	}
	
	protected void itemPausedHandler(IPlayable sender) {
		DispatchPlayableCollectionEvent(OnItemPaused);
		Pause();
	}
	
	protected void itemResumedHandler(IPlayable sender) {
		DispatchPlayableCollectionEvent(OnItemResumed);
		Resume();
	}
	
	protected void itemStoppedHandler(IPlayable sender) {
		DispatchPlayableCollectionEvent(OnItemStopped);
		Stop();
	}
	
	protected void itemResetHandler(IPlayable sender) {
		DispatchPlayableCollectionEvent(OnItemReset);
		Stop(); // not Reset();
	}
	
	protected void itemCompleteHandler(IPlayable sender) {
		DispatchPlayableCollectionEvent(OnItemComplete);
		
		if (hasNext) {
			Next().Play();
			
		} else {
			Stop();
			DispatchPlayableEvent(OnComplete);
		}
	}
	
	// ---- protected methods ----
	
	protected void DispatchPlayableEvent(PlayableEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	protected void DispatchPlayableCollectionEvent(PlayableCollectionEventHandler evt) {
		if (evt != null) evt(this, currentItem);
	}
	
	public delegate void PlayableCollectionEventHandler(IPlayable sender, IPlayable item);
}
