using UnityEngine;
using System.Collections;

public class stereoTest : MonoBehaviour {
	Camera c;
	void Start () {
		c = Camera.main;
		c.stereoSeparation = 2f;
		c.stereoConvergence = 1f;
	}

	void Update () {
	
	}
}
