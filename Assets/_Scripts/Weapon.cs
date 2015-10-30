using UnityEngine;
using System.Collections;

	//this is an enum of the various possible weapon types
	//it also includes a "shield" type to allow a shield power-up
	//items marked [NI] below are not implemented in this book

	public enum WeaponType {
		none, //the default /no weapons
		blaster, //a simple blaster
		spread, //two shots simultaneously
		phaser, //shots that move in waves [NI]
		missile, //homing missiles [NI]
		laser, //damage over time [NI]
		shield //raise shieldLevel
	}

	//the WeaponDefinition class allows you to set the properties
	//of a specific weapon in the Inspector. Main has an array
	//of WeaponDefinitons that makes this possible.
	//[System.Seriallzable] tells Unity to try to view WeaponsDefinition
	//in the Inspector Pane. It doesn't work for everything, but it 
	//will work for simple classes like this!

	[System.Serializable]
	public class WeaponDefinition {
		public WeaponType type = WeaponType.none;
		public string letter; //the letter shown on the power-up
		public Color color = Color.white; //color of collar and power-up
		public GameObject projectilePrefab; //prefab for projectiles
		public Color projectileColor = Color.white;
		public float damageOnHit = 0; // amount of damage caused
		public float countinuousDamage = 0; //amount of damage per second (laser)
		public float delayBetweenShots = 0;
		public float velocity = 20; //speed of projectile
	}

	//note: weapon prefabs, colors, and so on are set in the class Main

public class Weapon : MonoBehaviour {

	static public Transform PROJECTILE_ANCHOR;
	 
	public bool ____________________;
	[SerializeField]
	private WeaponType _type = WeaponType.blaster;
	public WeaponDefinition def;
	public GameObject collar;
	public float lastShot; // time last shot was fired

	void Awake () {
		collar = transform.Find ("Collar").gameObject;
	}

	void Start() {
		//call SetType() properly for the default _type
		SetType (_type);

		if (PROJECTILE_ANCHOR == null) {
			GameObject go = new GameObject ("_Projectile_Anchor");
			PROJECTILE_ANCHOR = go.transform;
		}

		//find the fireDelegate of the parent 
		GameObject parentGO = transform.parent.gameObject;
		if (parentGO.tag == "Hero") {
			Hero.S.fireDelegate += Fire;
		}
	}

	public WeaponType type {
		get { return(_type); }
		set { SetType (value); }
	}

	public void SetType(WeaponType wt) {
		_type = wt;
		if (type == WeaponType.none) {
			this.gameObject.SetActive (false);
			return;
		} else {
			this.gameObject.SetActive (true);
		}
		def = Main.GetWeaponDefinition (_type);
		collar.renderer.material.color = def.color;
		lastShot = 0; // you can always fire immediately after _type is set
	}

	public void Fire() {
		//If this.gameObject is inactive, return
		if (!gameObject.activeInHierarchy)
			return; 
		// if it hasnt been enough time between shots,return
		if (Time.time - lastShot < def.delayBetweenShots) {
			return;
		}
		Projectile p;
		switch (type) {
		case WeaponType.blaster:
			p = MakeProjectile ();
			p.rigidbody.velocity = Vector3.up * def.velocity;
			break;

		case WeaponType.spread: 
			p = MakeProjectile ();
			p.rigidbody.velocity = Vector3.up * def.velocity;
			p = MakeProjectile ();
			p.rigidbody.velocity = new Vector3 (-.2f, 0.9f, 0) * def.velocity;
			p = MakeProjectile ();
			p.rigidbody.velocity = new Vector3 (.2f, 0.9f, 0) * def.velocity;
			break;
		}
	}

	public Projectile MakeProjectile () {
		GameObject go = Instantiate (def.projectilePrefab) as GameObject;
		if (transform.parent.gameObject.tag == "Hero") {
			go.tag = "ProjectileHero";
			go.layer = LayerMask.NameToLayer ("ProjectileHero");
		} else {
			go.tag = "ProjectileEnemy";
			go.layer = LayerMask.NameToLayer ("ProjectileEnemy");
		}
		go.transform.position = collar.transform.position;
		go.transform.parent = PROJECTILE_ANCHOR;
		Projectile p = go.GetComponent<Projectile> ();
		p.type = type;
		lastShot = Time.time;
		return (p);
	}

}


