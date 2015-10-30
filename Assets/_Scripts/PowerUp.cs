using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	//this is an unusual but handy use of Vector2s. x holds a min value
	//and y a max value for a Random.Range() that will be called later

	public Vector2 rotMinMax = new Vector2(15,90);
	public Vector2 driftMinMax = new Vector2 (.25f, 2);
	public float lifeTime = 6f; // seconds the PowerUp exists
	public float fadeTime = 4f; //seconds it will then fade
	public bool _______________;
	public WeaponType type; //the type of the powerup
	public GameObject cube; //reference to the cube child
	public TextMesh letter; //reference to the TextMesh
	public Vector3 rotPerSecond; // Euler rotation speed
	public float birthTime; 

	void Awake() {
		//find the cube reference
		cube = transform.Find ("Cube").gameObject;
		//find the TextMesh
		letter = GetComponent<TextMesh> ();

		//set a random velocity
		Vector3 vel = Random.onUnitSphere; // Get random XYZ velocity 
		// Random.onUnitySphere gives you a vector point that is somewhere else
		// the surface of the sphere with a radius of 1m around the origin
		vel.z = 0; // flatten the vel of the XY plane
		vel.Normalize (); // make the length of the vel 1
		//normalizing a Vector3 makes it length 1m
		vel *= Random.Range (driftMinMax.x, driftMinMax.y);
		//above sets the velocity length to something between x and y 
		//values of driftMinMax
		rigidbody.velocity = vel;

		//set the rotation of this GameObject to R: [0,0,0]
		transform.rotation = Quaternion.identity;
		//Quaternion.identity is equal to no rotation

		//set up the rotPerSecond for the Cube child using rotMinMax x & y
		rotPerSecond = new Vector3 (Random.Range (rotMinMax.x, rotMinMax.y), 
		                            Random.Range (rotMinMax.x, rotMinMax.y), 
		                            Random.Range (rotMinMax.x, rotMinMax.y));

		//checkOffScreen every two seconds
		InvokeRepeating ("CheckOffScreen", 2f, 2f);

		birthTime = Time.time;
	}

	void Update () {
		//manually rotate the Cube child every Update()
		//Multiplying it by Time.time causes the rotation to be time-based
		cube.transform.rotation = Quaternion.Euler (rotPerSecond * Time.time);

		//fade out the PowerUp over time
		//given the default value, a PowerUp will exist for 10 seconds
		//and then fade out over 4 seconds

		float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
		//for lifetime seconds, u will be <= 0. Then it will transition to 1
		//over fadeTime seconds
		//if u >= 1, destroy this PowerUp
		if (u >= 1) {
			Destroy (this.gameObject);
			return;
		}
		//use u to determine the alpha value of the Cube and Letter
		if (u > 0) {
			Color c = cube.renderer.material.color;
			c.a = 1f - u;
			cube.renderer.material.color = c;
			//fade the Letter too, just not as much
			c = letter.color;
			c.a = 1f - (u * 0.5f);
			letter.color = c;
		}
	}

	//this SetType() differs from those on Weapon and Projectile
	public void SetType(WeaponType wt) {
		//grab the WeaponDefinition from Main
		WeaponDefinition def = Main.GetWeaponDefinition (wt);
		//set the color of the Cube child
		cube.renderer.material.color = def.color;
		//letter.color = def.color; // we could colorize the letter too
		letter.text = def.letter; //set the letter that is shown
		type = wt; //finally actually set the type
	}

	public void AbsorbedBy (GameObject target) {
		//this function is called by the Hero class when a PowerUp is collected
		//we could tween into the target and shrink in size, 
		//but for now, just destroy this.gameObject
		Destroy (this.gameObject);
	}

	void CheckOffScreen () {
		//if the PowerUp has drifted entirely off screen ...
		if (Utils.ScreenBoundsCheck(cube.collider.bounds, BoundsTest.offScreen) 
		    != Vector3.zero) {
				//... then destroy this GameObject
				Destroy (this.gameObject); 
		}
	}
}


