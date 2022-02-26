using UnityEngine;
using System.Collections;

/// <summary>
/// Rotate the object in kinematic mode. Needs to be attached to a Game object with rigid body;
/// </summary>
public class GRT_RotateKinematic : MonoBehaviour {	
	public Vector3 rotVector;
	private Rigidbody rb;

	void Start(){
		this.rb = this.GetComponent<Rigidbody>();
	}

	void Update () {
		if (this.rb.isKinematic){
			this.transform.Rotate (rotVector * 40.0f * Time.deltaTime);
		}
	}
}
