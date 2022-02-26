using UnityEngine;
using System.Collections;

/// <summary>
/// Will Rotates the Transform of the item that will enter into the football goal and its Rigidbody has been destroyed.
/// </summary>
public class GFB_RotateTransform : MonoBehaviour {	
	public Vector3 rotVector;
	private Rigidbody rb;
	private Transform trans;
	void Start(){
		rb = this.GetComponent<Rigidbody>();
		trans = this.GetComponent<Transform>();
	}

	void Update () {
		if(rb == null)
			trans.RotateAround(Vector3.up, 20.0f * Time.deltaTime);
	}
}
