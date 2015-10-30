using UnityEngine;
using System.Collections;

public class Enemy_2 : Enemy {
	//enemy_2 uses a Sin wave to modify a 2-point linear interpolation 

	public Vector3[] points;
	public float birthTime;
	public float lifeTime = 10;
	//determines how much the Sine wave will affect movement
	public float sinEccentricity = 0.6f;

	void Start() {
		//initialize the points
		points = new Vector3[2];

		//find Utils.camBounds
		Vector3 cbMin = Utils.camBounds.min;
		Vector3 cbMax = Utils.camBounds.max;

		Vector3 v = Vector3.zero;
		//pick any point on the left side of the screen
		v.x = cbMin.x - Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points [0] = v;

		//pick any point on the right side of the screen
		v = Vector3.zero;
		v.x = cbMax.x + Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points [1] = v;

		//possibly swap sides
		if (Random.value < 0.5f) {
			//setting the .x of each point to its negative will move it to the other side
			points[0].x *= -1;
			points[1].x *= -1;
		}

		//set the birthTime to the current Time
		birthTime = Time.time;
	}

	public override void Move () {
		//bezier curves work based on a u value between 0 and 1
		float u = (Time.time - birthTime) / lifeTime;

		//if u>1 then it has been longer than lifeTime since birthTime
		if (u > 1) {
			//this enemy_2 has finished its life
			Destroy (this.gameObject);
			return;
		}

		//adjust u by adding an easing curve based on sine wave
		u = u + sinEccentricity * (Mathf.Sin (u * Mathf.PI * 2));

		//interpolate the two linear interpolation points
		pos = (1 - u) * points [0] + u * points [1];
	}

}
