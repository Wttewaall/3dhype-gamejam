using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Cannon : MonoBehaviour {
	
	public GameObject ballPrefab;
	public Projector target;
	public AudioClip audio_fire;
	
	protected GameObjectPool ammoPool;
	protected Transform spawnPoint;
	protected ShuffleBag<Color> bag;
	
	// ---- inherited handlers ----
	
	void Awake() {
		// create cannonball pool
		ammoPool = new GameObjectPool(ballPrefab, 5, initBallAction, false);
		
		// shufflebag of colors: chance of 1/2 red, 1/3 green, 1/6 blue
		bag = new ShuffleBag<Color>();
		bag.Add(Color.red, 3);
		bag.Add(Color.green, 2);
		bag.Add(Color.blue, 1);
		
		// listen for mouse click
		MouseManager.mouseClick += mouseClickHandler;
	}
	
	void Start () {
		// get references
		spawnPoint = transform.Find("spawnPoint");
	}
	
	void OnGUI() {
		GUILayout.Label("-- Ammo Pool --");
		GUILayout.Label("    active: "+ammoPool.numActive);
		GUILayout.Label("available: "+ammoPool.numAvailable);
	}
	
	// ---- public methods ----
	
	public void fire() {
		// spawn ball from pool
		GameObject ball = ammoPool.spawn(spawnPoint.position, spawnPoint.rotation);
		
		// set color: chance of 1/2 red, 1/3 green, 1/6 blue
		ball.renderer.material.SetColor("_Color", bag.Next());
		
		// add initial velocity
		ball.rigidbody.velocity = transform.up * 10;
		
		// add a bit of rotation
		System.Random random = new System.Random();
		ball.rigidbody.angularVelocity = new Vector3(random.Next(-1, 1), random.Next(-1, 1), random.Next(-1, 1));
		
		audio.PlayOneShot(audio_fire);
		Statistics.bulletsFired++;
	}
	
	// ---- protected methods ----
	
	// this method will be called when the ball has been instantialized
	protected void initBallAction(GameObject target) {
		
		// set pool so the ball can return to it on destuction
		AutoDestruct ad = target.GetComponent<AutoDestruct>();
		if (ad) ad.pool = ammoPool;
	}
	
	// ---- event handlers ----
	
	protected void mouseClickHandler(int buttonID) {
		if (buttonID == MouseButton.LEFT) fire();
	}
}
