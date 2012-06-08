using UnityEngine;
using System.Collections;

public class PlayableItem : IPlayable {
	
	// ---- events ----
	
	public event PlayableEventHandler OnPlaying;
	public event PlayableEventHandler OnPaused;
	public event PlayableEventHandler OnResumed;
	public event PlayableEventHandler OnStopped;
	public event PlayableEventHandler OnReset;
	public event PlayableEventHandler OnComplete;
	
	// ---- variables ----
	
	protected Timer timer;
	
	// ---- getters & setters ----
	
	private bool _isPlaying;
	
	public virtual bool isPlaying {
		get { return _isPlaying; }
		protected set { _isPlaying = value; }
	}
	
	private bool _isPaused;
	
	public virtual bool isPaused {
		get { return _isPaused; }
		protected set { _isPaused = value; }
	}
	
	// ---- constructors ----
	
	public PlayableItem() {
		Initialize();
	}
	
	// ---- public methods ----
	
	public virtual void Initialize() {
		timer = Timer.Create(300, 3);
		timer.OnTimerTick		+= timerTickHandler;
		timer.OnTimerComplete	+= timerCompleteHandler;
	}
	
	public virtual bool Play() {
		if (isPlaying) return false;
		
		timer.Play();
		DispatchPlayableEvent(OnPlaying);
		return true;
	}
	
	public virtual bool Pause() {
		if (!isPlaying || isPaused) return false;
		
		isPaused = true;
		
		timer.Pause();
		DispatchPlayableEvent(OnPaused);
		return true;
	}
	
	public virtual bool Resume() {
		if (!isPlaying || !isPaused) return false;
		
		isPaused = false;
		
		timer.Resume();
		DispatchPlayableEvent(OnResumed);
		return true;
	}
	
	public virtual bool Stop() {
		if (!isPlaying) return false;;
		
		isPlaying = false;
		isPaused = false;
		
		timer.Stop();
		DispatchPlayableEvent(OnStopped);
		return true;
	}
	
	public virtual void Reset() {
		isPlaying = false;
		isPaused = false;
		
		timer.Reset();
		DispatchPlayableEvent(OnReset);
	}
	
	// ---- protected methods ----
	
	protected void DispatchCompleteEvent() {
		DispatchPlayableEvent(OnComplete);
	}
	
	// ---- event handlers ----
	
	protected virtual void timerTickHandler(Timer target) {
		//Utils.trace("tick");
	}
	
	protected virtual void timerCompleteHandler(Timer target) {
		DispatchCompleteEvent();
	}
	
	// ---- protected methods ----
	
	protected void DispatchPlayableEvent(PlayableEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	
}

/* old way of implementing events from interface

protected event PlayableEventHandler _playing;
	public event PlayableEventHandler OnPlaying {
		add { _playing += value; }
		remove { _playing -= value; }
	}
	
	protected event PlayableEventHandler _paused;
	public event PlayableEventHandler OnPaused {
		add { _paused += value; }
		remove { _paused -= value; }
	}
	
	protected event PlayableEventHandler _resumed;
	public event PlayableEventHandler OnResumed {
		add { _resumed += value; }
		remove { _resumed -= value; }
	}
	
	protected event PlayableEventHandler _stopped;
	public event PlayableEventHandler OnStopped {
		add { _stopped += value; }
		remove { _stopped -= value; }
	}
	
	protected event PlayableEventHandler _reset;
	public event PlayableEventHandler OnReset {
		add { _reset += value; }
		remove { _reset -= value; }
	}
	
	protected event PlayableEventHandler _complete;
	public event PlayableEventHandler OnComplete {
		add { _complete += value; }
		remove { _complete -= value; }
	}
*/