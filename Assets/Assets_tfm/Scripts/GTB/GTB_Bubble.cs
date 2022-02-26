using UnityEngine;
using System.Collections;

public class GTB_Bubble : MonoBehaviour {
	private Rigidbody rb;
	private Vector3 initialPos;
	private float amount;

	///Awake function is called ever on the scene awake, Start function only is called at start of the scene iff the gameobject is active in hierarchy
	void Awake () {
		this.amount = 325f;    //300 is good for 4kg, 0 drag, and 0.05 angular drag
		this.initialPos = this.transform.position;
		this.rb = this.GetComponent<Rigidbody>();
	}

	public void init(){
//		Debug.Log(this.gameObject.name + (this.rb == null ? " rb is null" : " rb is not null"));
		this.gameObject.SetActive(true);
		if(!this.rb.isKinematic)
			this.rb.AddForce(new Vector3(-0.5f, 0f, -0.5f).normalized * this.amount, ForceMode.Force);
	}

	public void end(){
		this.transform.position = this.initialPos;
		this.gameObject.SetActive(false);
	}

	void OnCollisionEnter(Collision cls){
		if(!this.rb.isKinematic)
			this.rb.AddForce(cls.contacts[0].normal.normalized * this.amount, ForceMode.Force);
	}
}
