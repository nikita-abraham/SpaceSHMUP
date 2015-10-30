﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this is actually outside of the Utils class
public enum BoundsTest {
	center, //is the center of the GameObject on screen?
	onScreen, //are the bounds entirely on screen?
	offScreen //are the bounds entirely off screen
}

public class Utils : MonoBehaviour {
	//============================== Bounds Functions ==============================//
	//creates bounds that encapsulate the town Bounds passed in 
	public static Bounds BoundsUnion(Bounds b0, Bounds b1){
		//if the size of one of the bounds is Vectos3.zero, ignore that one
		if (b0.size == Vector3.zero && b1.size != Vector3.zero) {
			return (b1);
		} else if (b0.size != Vector3.zero && b1.size == Vector3.zero) {
			return (b0);
		} else if (b0.size == Vector3.zero && b1.size == Vector3.zero) {
			return (b0);
		}
		//Stretch b0 to include the b1.min and b1.max
		b0.Encapsulate (b1.min);
		b0.Encapsulate (b1.max);
		return (b0);
	}

	public static Bounds CombineBoundsOfChildren (GameObject go) {
		// create an empty Bounds b
		Bounds b = new Bounds (Vector3.zero, Vector3.zero);
		//of this GameObject has a Renderer Component ...
		if (go.renderer != null) {
			//expand b to contain the Renderer's Bounds
			b = BoundsUnion (b, go.renderer.bounds);
		}
		//if this GameObject has a collider component...
		if (go.collider != null) {
			//expand b to contain the Collider's Bounds
			b = BoundsUnion (b, go.collider.bounds);
		}
		//recursively iterate through each child of this gameObject.transform
		foreach (Transform t in go.transform) {
			//expand b to contain their Bounds as well
			b = BoundsUnion (b, CombineBoundsOfChildren (t.gameObject)); 
		}
		return(b);
	}

	//Make a static read-only public property camBounds
	static public Bounds camBounds {
		get {
			//if _camBounds hasn't been set yet
			if (_camBounds.size == Vector3.zero) {
				//SetCameraBounds using the default Camera
				SetCameraBounds ();
			}
			return (_camBounds);
		}
	}

	//This is the private static field that camBounds uses
	static private Bounds _camBounds;

	//this function is used by camBounds to set _camBounds and can also be called directly
	public static void SetCameraBounds (Camera cam=null) {
		//if no Camera was passed in, use the main Camera
		if (cam == null)
			cam = Camera.main;
		// this makes a couples of important assumptions about the camera
		// 1. the camera is orthographic
		// 2. the camera is at a rotation of R [0,0,0]

		//make Vector3s at the topLeft and bottomRight of the Screen coords
		Vector3 topLeft = new Vector3 (0, 0, 0);
		Vector3 bottomRight = new Vector3 (Screen.width, Screen.height, 0);

		//Convert these world coordinates 
		Vector3 boundTLN = cam.ScreenToWorldPoint (topLeft);
		Vector3 boundBRF = cam.ScreenToWorldPoint (bottomRight); 

		//adjust their zs to be at the near and far Camera clippling planes
		boundTLN.z += cam.nearClipPlane;
		boundBRF.z += cam.farClipPlane;

		//find the center of the Bounds
		Vector3 center = (boundTLN + boundBRF) / 2f;
		_camBounds = new Bounds (center, Vector3.zero);
		//expand _camBounds to encapsulate the extents.
		_camBounds.Encapsulate (boundTLN);
		_camBounds.Encapsulate (boundBRF);
	}

	//checks to see whether the Bounds bnd are within the camBounds
	public static Vector3 ScreenBoundsCheck (Bounds bnd, BoundsTest test = BoundsTest.center) {
		return (BoundsInBoundsCheck (camBounds, bnd, test));
	}

	//checks to see whther Bounds lilB are within Bounds bigB
	public static Vector3 BoundsInBoundsCheck (Bounds bigB, Bounds lilB, BoundsTest test = BoundsTest.onScreen) {
		//the behavior of this function is different based on the BoundsTest
		//that has been selected

		//get the center of lilB
		Vector3 pos = lilB.center;

		//initialize the offset at [0,0,0]
		Vector3 off = Vector3.zero;

		switch(test) {
			//the center test determines what off (offset) would have to be applied
			//to lilB to move its center back inside bigB
		case BoundsTest.center:
			if (bigB.Contains (pos)){
				return (Vector3.zero);
			}

			if (pos.x > bigB.max.x) {
				off.x = pos.x - bigB.max.x;
			} else if (pos.x < bigB.min.x) {
				off.x = pos.x - bigB.min.x;
			}

			if (pos.y > bigB.max.y) {
				off.y = pos.y - bigB.max.y;
			} else if (pos.y < bigB.min.y) {
				off.y = pos.y - bigB.min.y;
			}

			if (pos.z > bigB.max.z) {
				off.z = pos.z - bigB.max.z;
			} else if (pos.z < bigB.min.z) {
				off.z = pos.z - bigB.min.z;
			}
			return (off);

		//the onScreen test determines what off would have to applied to 
		//keep all of the lilB inside bigB
		case BoundsTest.onScreen:
			if (bigB.Contains (lilB.min) && bigB.Contains (lilB.max)) {
				return (Vector3.zero);
			}

			if (lilB.max.x > bigB.max.x) {
				off.x = lilB.max.x - bigB.max.x;
			} else if (lilB.min.x < bigB.min.x) {
				off.x = lilB.min.x - bigB.min.x;
			}
			if (lilB.max.y > bigB.max.y) {
				off.y = lilB.max.y - bigB.max.y;
			} else if (lilB.min.y < bigB.min.y) {
				off.y = lilB.min.y - bigB.min.y;
			}

			if (lilB.max.z > bigB.max.z) {
				off.z = lilB.max.z - bigB.max.z;
			} else if (lilB.min.z < bigB.min.z) {
				off.z = lilB.min.z - bigB.min.z;
			}
			return (off);

		//the offScreen test determines what off would need to be applied to move
		//any tiny part of lilB inside of bigB
		case BoundsTest.offScreen:
			bool cMin = bigB.Contains(lilB.min);
			bool cMax = bigB.Contains(lilB.max);
			if (cMin || cMax) {
				return (Vector3.zero);
			}

			if (lilB.min.x > bigB.max.x) {
				off.x = lilB.min.x - bigB.max.x;
			} else if (lilB.max.x < bigB.min.x) {
				off.x = lilB.max.x - bigB.min.x;
			}
			if (lilB.min.y > bigB.max.y) {
				off.y = lilB.min.y - bigB.max.y;
			} else if (lilB.max.y < bigB.min.y) {
				off.y = lilB.max.y - bigB.min.y;
			}
			if (lilB.min.z > bigB.max.z) {
				off.z = lilB.min.z - bigB.max.z;
			} else if (lilB.max.z < bigB.min.z) {
				off.z = lilB.max.z - bigB.min.z;
			}
			return (off);
		}

		return(Vector3.zero);
	}

	//============================== Transform Functions ==============================//

	//this function will iteratively climb up the transform.parent tree
	//until it either finds a parent with a tag != "Untagged" or no parent

	public static GameObject FindTaggedParent (GameObject go) {
		//if this gameObject has a tag
		if (go.tag != "Untagged") {
			//then return this gameObject
			return(go);
		}
		//if there is no parent of this Transform
		if (go.transform.parent == null) {
			//we've reached the top of the hierarchy with no interesting tag
			//so return null
			return (null);
		}
		//otherwise, recursively climb up the tree
		return (FindTaggedParent (go.transform.parent.gameObject));
	}
	//this version of the function handles things if a Transform is passed in 
	public static GameObject FindTaggedParent (Transform t) {
		return (FindTaggedParent (t.gameObject));
	}

	//============================== Materials Functions ==============================//

	//returns a list of all Materials on this GameObject or its children
	static public Material [] GetAllMaterials(GameObject go) {
		List<Material> mats = new List<Material> ();
		if (go.renderer != null) {
			mats.Add (go.renderer.material);
		}
		foreach (Transform t in go.transform) {
			mats.AddRange (GetAllMaterials (t.gameObject));
		}
		return (mats.ToArray ());
	}
}
