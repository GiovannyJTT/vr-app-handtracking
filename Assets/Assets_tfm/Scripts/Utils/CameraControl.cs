using UnityEngine;
using System.Collections;

/// <summary>
/// TFM - CameraControl needs to be attached to a OVRCameraRig or a Camera GameObject
/// </summary>
public class CameraControl : MonoBehaviour {
	/// <summary>
	///	to update the position's camera
	/// </summary>
	public GameObject player;    //a player reference
	private Vector3 offset;   //camera-player

	/// <summary>
	/// to update the rotation's camera
	/// </summary>
	private float rotY;
	private Rigidbody rbplayer;

	/// <summary>
	/// needs the player game object born in (0,0,0)
	/// </summary>
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		offset = this.transform.position - player.transform.position;
		rbplayer = player.GetComponent<Rigidbody>();
	}

	private void getInput ()
	{
		if(OVRManager.display == null || !OVRManager.display.isPresent){
			if (Application.platform == RuntimePlatform.Android ||
			    Application.platform == RuntimePlatform.IPhonePlayer) {
				rotY = Input.acceleration.x;    //accelerometer
			} else {
				//keys
				rotY = Input.GetAxis ("Horizontal");
			}
		}
	}

	private void rotateCam(){
		if(OVRManager.display == null || !OVRManager.display.isPresent){
			Quaternion rot = Quaternion.Euler(
			this.transform.rotation.eulerAngles.x,
			this.transform.rotation.eulerAngles.y + rotY * rbplayer.velocity.magnitude * 6f,
			this.transform.rotation.eulerAngles.z);

			this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rot, 0.95f);
		}
	}

	/// <summary>
	/// Fixeds the update. The update of physics will be performed with a fixed rate (althought not draw at same rate).
	/// </summary>
	void FixedUpdate ()
	{
		this.transform.position = player.transform.position + offset;

		getInput();
		rotateCam();
	}
}
