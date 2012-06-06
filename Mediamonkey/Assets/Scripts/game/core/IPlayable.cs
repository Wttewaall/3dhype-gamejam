using System;

public delegate void PlayableEventHandler(IPlayable sender);

public interface IPlayable {
	
	event PlayableEventHandler playing;
	event PlayableEventHandler paused;
	event PlayableEventHandler resumed;
	event PlayableEventHandler stopped;
	event PlayableEventHandler reset;
	event PlayableEventHandler complete;
	
	bool isPlaying { get; }
	bool isPaused { get; }
	
	void Play();
	void Pause();
	void Resume();
	void Stop();
	void Reset();
	
}