using UnityEngine;
using System.Collections;

public class GFB_PuppetPendulum : MonoBehaviour
{
	private float limitRightAngle = 67.5f;
	private float limitLeftAngle = -67.5f;
	private float currentZRot = 0.0f;
	private float deltaAngle = 100.0f;
	private Transform puppetBaseTransform;

	private enum pendulumDir
	{
		LEFT = 0,
		RIGHT = 1
	}

	private pendulumDir dir;

	void Start ()
	{
		puppetBaseTransform = this.GetComponent<Transform> ();
		dir = pendulumDir.RIGHT;
	}

	void Update ()
	{
		//Debug.Log(currentZRot);

		switch (dir) {
		case pendulumDir.RIGHT:
			currentZRot += deltaAngle * Time.deltaTime;
			if (currentZRot > limitRightAngle)
				dir = pendulumDir.LEFT;
			break;
		case pendulumDir.LEFT:
			currentZRot -= deltaAngle * Time.deltaTime;
			if (currentZRot < limitLeftAngle)
				dir = pendulumDir.RIGHT;
			break;
		}

		puppetBaseTransform.transform.rotation =
				Quaternion.Euler (
			puppetBaseTransform.transform.rotation.eulerAngles.x,
			puppetBaseTransform.transform.rotation.eulerAngles.y,
			currentZRot);
	}
}
