using UnityEngine;
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

}
