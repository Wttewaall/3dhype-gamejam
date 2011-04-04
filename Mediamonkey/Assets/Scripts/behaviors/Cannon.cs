using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {
	
	public GameObject ballPrefab;
	public AudioClip audio_fire;
	
	protected GameObjectPool balls;
	protected Transform spawnPoint;
	protected ShuffleBag<Color> bag;
	
	// ---- inherited handlers ----
	
	void Awake() {
		// create cannonball pool
		balls = new GameObjectPool(ballPrefab, 5, initBallAction, false);
		
		// shufflebag of colors: chance of 1/2 red, 1/3 green, 1/6 blue
		bag = new ShuffleBag<Color>();
		bag.Add(new Color(1, 0, 0), 3);
		bag.Add(new Color(0, 1, 0), 2);
		bag.Add(new Color(0, 0, 1), 1);
	}
	
	void Start () {
		// get references
		spawnPoint = transform.Find("spawnPoint");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			fire();
		}
	}
	
	// ---- public methods ----
	
	public void fire() {
		// spawn ball from pool
		GameObject ball = balls.spawn(spawnPoint.position, spawnPoint.rotation);
		
		// add initial velocity
		ball.rigidbody.velocity = transform.up * 10;
		
		// add a bit of rotation
		System.Random random = new System.Random();
		ball.rigidbody.angularVelocity = new Vector3(random.Next(-1, 1), random.Next(-1, 1), random.Next(-1, 1));
		
		audio.PlayOneShot(audio_fire);
	}
	
	// ---- protected methods ----
	
	// this method will be called when the ball has been instantialized
	protected void initBallAction(GameObject target) {
		
		// parent to the cannon, or some other Transform as a test
		target.transform.parent = transform;
		
		// set color: chance of 1/2 red, 1/3 green, 1/6 blue
		target.renderer.material.SetColor("_Color", bag.Next());
	}
}
