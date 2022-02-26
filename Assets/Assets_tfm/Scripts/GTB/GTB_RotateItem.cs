using UnityEngine;
using System.Collections;

/// <summary>
/// Rotate the object in kinematic mode. Needs to be attached to a Game object with rigid body;
/// </summary>
public class GTB_RotateItem : MonoBehaviour {	
	private Rigidbody rb;
	private float amount = 50f;

	void Start(){
		this.rb = this.GetComponent<Rigidbody>();
	}

	void Update ()
	{
		if (this.rb.isKinematic &&
			this.GetComponent<GTB_Graspable> ().graspable_state == GTB_Graspable.GTB_GRASPABLEITEM_STATE.INITIAL_POSITION) {
			this.transform.Rotate (this.transform.up.normalized * amount * Time.deltaTime);
		}
	}
}
