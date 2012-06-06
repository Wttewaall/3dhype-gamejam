using System;

public class PlayableCollection : IPlayable {
	
	// ---- implemented events ----
	
	private event PlayableEventHandler _playing;
	public event PlayableEventHandler playing {
		add { _playing += value; }
		remove { _playing -= value; }
	}
	
	private event PlayableEventHandler _paused;
	public event PlayableEventHandler paused {
		add { _paused += value; }
		remove { _paused -= value; }
	}
	
	private event PlayableEventHandler _resumed;
	public event PlayableEventHandler resumed {
		add { _resumed += value; }
		remove { _resumed -= value; }
	}
	
	private event PlayableEventHandler _stopped;
	public event PlayableEventHandler stopped {
		add { _stopped += value; }
		remove { _stopped -= value; }
	}
	
	private event PlayableEventHandler _reset;
	public event PlayableEventHandler reset {
		add { _reset += value; }
		remove { _reset -= value; }
	}
	
	private event PlayableEventHandler _complete;
	public event PlayableEventHandler complete {
		add { _complete += value; }
		remove { _complete -= value; }
	}
	
	public event PlayableCollectionEventHandler changed;
	
	// public variables
	public DataProvider<IPlayable> dataProvider;
	
	// ---- getters & setters ----
	
	private IPlayable _currentItem;
	
	public IPlayable currentItem {
		get { return _currentItem; }
		
		private set {
			if (_currentItem != null) {
				_currentItem.playing -= itemStateHandler;
				_currentItem.paused -= itemStateHandler;
				_currentItem.resumed -= itemStateHandler;
				_currentItem.stopped -= itemStateHandler;
				_currentItem.reset -= itemStateHandler;
				_currentItem.complete -= itemCompleteHandler;
			}
			
			_currentItem = value;
			
			if (_currentItem != null) {
				_currentItem.playing += itemStateHandler;
				_currentItem.paused += itemStateHandler;
				_currentItem.resumed += itemStateHandler;
				_currentItem.stopped += itemStateHandler;
				_currentItem.reset += itemStateHandler;
				_currentItem.complete += itemCompleteHandler;
			}
			
			DispatchPlayableCollectionEvent(changed);
		}
	}
	
	private bool _isPlaying;
	
	public bool isPlaying {
		get { return _isPlaying; }
		private set {
			if (_isPlaying == value) return;
			_isPlaying = value;
			
			DispatchPlayableCollectionEvent(changed);
		}
	}
	
	private bool _isPaused;
	
	public bool isPaused {
		get { return _isPaused; }
		private set {
			if (_isPaused == value) return;
			_isPaused = value;
			
			DispatchPlayableCollectionEvent(changed);
		}
	}
	
	// ---- constructor ----
	
	public PlayableCollection() {
		dataProvider = new DataProvider<IPlayable>();
		dataProvider.OnIndexChange += delegate {
			currentItem = dataProvider.selectedItem;
		};
	}
	
	// ---- public methods ----
	
	public void Play() {
		if (currentItem == null && dataProvider.hasNext) dataProvider.Next();
		else Utils.trace("done");
		
		isPlaying = true;
		isPaused = false;
		
		currentItem.Play();
	}

	public void Pause() {
		currentItem.Pause();
	}
	
	public void Resume() {
		currentItem.Resume();
	}
	
	public void Stop() {
		currentItem.Stop();
	}
	
	public void Reset() {
		currentItem.Reset();
	}
	
	// ---- event handlers ----
	
	protected void itemStateHandler(IPlayable sender) {
		isPlaying = sender.isPlaying;
		isPaused = sender.isPaused;
	}
	
	protected void itemCompleteHandler(IPlayable sender) {
		Utils.trace("complete");
		if (dataProvider.hasNext) dataProvider.Next();
		else DispatchPlayableEvent(_complete);
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
