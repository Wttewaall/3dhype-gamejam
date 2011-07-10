using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Cannon : MonoBehaviour {
	
	public GameObject ammoPrefab;
	public float force = 200;
	public AudioClip audio_fire;
	
	protected float mass;
	protected GameObjectPool ammoPool;
	protected Transform spawnPoint;
	protected ShuffleBag<Color> bag;
	
	// ---- inherited handlers ----
	
	void Awake() {
		// get ammo mass
		mass = ammoPrefab.GetComponent<Rigidbody>().mass;
		
		// create cannonball pool
		ammoPool = new GameObjectPool(ammoPrefab, 5, InitBallAction, false);
		
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
	
	public void Fire() {
		// spawn ball from pool
		GameObject ball = ammoPool.Spawn(spawnPoint.position, spawnPoint.rotation);
		
		// set color: chance of 1/2 red, 1/3 green, 1/6 blue
		ball.renderer.material.SetColor("_Color", bag.Next());
		
		// add initial velocity
		ball.rigidbody.velocity = transform.up * (force/mass);
		
		// add a bit of rotation
		System.Random random = new System.Random();
		ball.rigidbody.angularVelocity = new Vector3(random.Next(-1, 1), random.Next(-1, 1), random.Next(-1, 1));
		
		// play sound in random pitch
		audio.pitch = Random.Range(0.9f, 1.1f);
		audio.PlayOneShot(audio_fire);
		
		// update statistics
		Statistics.bulletsFired++;
	}
	
	// ---- protected methods ----
	
	// this method will be called when the ball has been instantialized
	protected void InitBallAction(GameObject target) {
		
		// set pool so the ball can return to it on destruction
		AutoDestruct ad = target.GetComponent<AutoDestruct>();
		if (ad) ad.pool = ammoPool;
	}
	
	// ---- event handlers ----
	
	protected void mouseClickHandler(int buttonID) {
		if (buttonID == MouseButton.LEFT) Fire();
	}
}
