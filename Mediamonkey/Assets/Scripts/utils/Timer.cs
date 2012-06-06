using UnityEngine;
using System;

/**
 * Author: Bart "Mediamonkey" Wttewaall
 * Website: www.mediamonkey.nl
 * 
 * // Example 1
 * var timer = Timer.Create(1000, 3);
 * timer.OnTimerTick += new TimerEventHandler(timerTickHandler);
 * 
 * protected void timerTickHandler(Timer timer) {
 * 		Debug.Log("Callback tick "+timer.currentCount);
 * }
 *
 * // Example 2
 * Timer.Callback(delegate(Timer timer) {
 *		Debug.Log("Callback tick "+timer.currentCount);
 *	}, 1000, 2);
 * 
 */

public class Timer : MonoBehaviour {
	
	// events
	public event TimerEventHandler OnTimerTick;
	public event TimerEventHandler OnTimerComplete;
	
	// static counter
	public static uint numTimers = 0;
	
	// public variables
	public float		delay;
	public uint			repeatCount;
	public bool			autoReset;
	public bool			destroyGameObject;
	
	// protected variables
	protected float		prevTime;
	protected float		pauseTime;
	
	// ---- getters & setters ----
	
	private int _currentCount;
	
	public int currentCount {
		get { return _currentCount; }
		private set { _currentCount = value; }
	}
	
	private bool _running;
	
	public bool running {
		get { return _running; }
		private set { _running = value; }
	}
	
	private bool _paused;
	
	public bool paused {
		get { return _paused; }
		private set { _paused = value; }
	}
	
	public float delaySeconds {
		get { return delay/1000; }
		set { delay = value * 1000; }
	}
	
	// ---- static methods ----
	
	public static Timer Create() {
		var timer = CreateGameObjectTimer();
		timer.destroyGameObject = true;
		return timer;
	}
	
	public static Timer Create(float delay) {
		return Create(delay, 0);
	}
	
	public static Timer Create(float delay, uint repeatCount) {
		var timer = CreateGameObjectTimer();
		timer.delay = delay;
		timer.repeatCount = repeatCount;
		timer.destroyGameObject = true;
		
		return timer;
	}
	
	public static void Callback(Action<Timer> callback, float delay) {
		Callback(callback, delay, 1);
	}
	
	public static void Callback(Action<Timer> callback, float delay, uint repeatCount) {
		var timer = CreateGameObjectTimer();
		timer.OnTimerTick += new TimerEventHandler(callback);
		timer.delay = delay;
		timer.repeatCount = repeatCount;
		timer.destroyGameObject = true;
		timer.Play();
	}
	
	protected static Timer CreateGameObjectTimer() {
		// create new GameObject
		GameObject go = new GameObject("Timer_"+(numTimers++));
		go.hideFlags = HideFlags.HideAndDontSave;
		
		// add and return Timer script
		return go.AddComponent<Timer>();
	}
	
	// ---- public methods ----
	
	public void Play() {
		Play(delaySeconds);
	}
	
	public void Play(float waitTime) {
		if (running) return;
		
		// already finished, you need to reset the timer in order to start it again
		if (repeatCount > 0 && currentCount >= repeatCount && !autoReset) {
			DispatchEvent(OnTimerComplete);
			
		} else {
			running = true;
			paused = false;
			prevTime = Time.time;
			
			InvokeRepeating("timerHandler", waitTime, delaySeconds);
		}
	}
	
	// Pause keeps track of the time between ticks and resumes the timer accordingly
	public void Pause() {
		if (paused == true) return;
		
		pauseTime = Time.time;
		paused = true;
		running = false;
		
		CancelInvoke("timerHandler");
	}
	
	public void Resume() {
		if (paused) Play(delaySeconds - (pauseTime - prevTime));
	}
	
	public void TogglePause() {
		if (!paused) Pause();
		else Resume();
	}
	
	public void Stop() {
		if (!running) return;
		
		running = false;
		paused = false;
		CancelInvoke("timerHandler");
	}
	
	public void Reset() {
		Stop();
		currentCount = 0;
	}
	
	public void OnApplicationQuit() {
		if (destroyGameObject) DestroyImmediate(gameObject);
	}
	
	// ---- protected methods ----
	
	protected void timerHandler() {
		currentCount++;
		prevTime = Time.time;
		
		DispatchEvent(OnTimerTick);
		
		if (repeatCount > 0 && currentCount >= repeatCount) {
			DispatchEvent(OnTimerComplete);
			
			if (autoReset) Reset();
			else Stop();
		}
	}
	
	protected void DispatchEvent(TimerEventHandler evt) {
		if (evt != null) evt(this);
	}
	
	// ---- delegates ----
	
	public delegate void TimerEventHandler(Timer target);
	
}