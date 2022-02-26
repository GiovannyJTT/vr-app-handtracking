using UnityEngine;
using System.Collections;

public class GRT_RotateGuide : MonoBehaviour {
	public Vector3 rotVector;
	public GameObject targetReference;
	private Rigidbody targetRB;

	void Start(){
		targetRB = targetReference.GetComponent<Rigidbody>();
	}

	void Update () {
		if(targetRB.isKinematic)
			this.transform.Rotate (rotVector * 40.0f * Time.deltaTime);
	}
}
