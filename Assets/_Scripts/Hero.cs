using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	static public Hero S; // singleton

	//these fields control the movement of the ship
	public float speed = 30;
	public float rollMult = -45;
	public float pitchMult = 30;

	//ship status information
	public float shieldLevel = 1;
	public bool ________________;

	public Bounds bounds;

	void Awake() {
		S = this; // set the singleton
		bounds = Utils.CombineBoundsOfChildren (this.gameObject);
	}

	void Update () {
		//pull in information from the Input Class
		float xAxis = Input.GetAxis ("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");

		//change transform.position based on the axis
		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;

		//rotate the ship to make it feel more dynamic
		transform.rotation = Quaternion.Euler (yAxis * pitchMult, xAxis * rollMult, 0);
	}
}
