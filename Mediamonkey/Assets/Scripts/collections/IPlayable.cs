using System;

public delegate void PlayableEventHandler(IPlayable sender);

public interface IPlayable {
	
	event PlayableEventHandler OnPlaying;
	event PlayableEventHandler OnPaused;
	event PlayableEventHandler OnResumed;
	event PlayableEventHandler OnStopped;
	event PlayableEventHandler OnReset;
	event PlayableEventHandler OnComplete;
	
	bool isPlaying { get; }
	bool isPaused { get; }
	
	bool Play();
	bool Pause();
	bool Resume();
	bool Stop();
	void Reset();
	
}