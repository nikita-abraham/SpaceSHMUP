using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	[SerializeField]
	private WeaponType _type;
	//this public property masks the field _type and takes action when it is set 

	public WeaponType type {
		get {
			return (_type);
		}
		set {
			SetType (value);
		}
	}

	void Awake () {
		//Test to see whether this has passed off screen every 2 seconds
		InvokeRepeating ("CheckOffScreen", 2f, 2f);
	}

	public void SetType (WeaponType eType) {
		//set the _type
		_type = eType;
		WeaponDefinition def = Main.GetWeaponDefinition (_type);
		renderer.material.color = def.projectileColor; 
	}

	void CheckOffScreen () {
		if (Utils.ScreenBoundsCheck (collider.bounds, BoundsTest.offScreen) != Vector3.zero) {
			Destroy (this.gameObject);
		}
	}

}
