using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// TFM - Player Controller controls the player applying force vectors to move it.
/// </summary>
public class PlayerControl : MonoBehaviour
{
	/// <summary>
	/// The force. In newtons.
	/// </summary>
	private float force = 10.0f;
	// > 9.8 newtons of the gravity
	//Good combination for keyboard: force=20.0f; sqrMaxVelocity=0.1f; mass=1; angularDrag=0.05f;

	/// <summary>
	/// The filter oculus input. Increase/Decrease the values obteained from the oculus headpose.
	/// </summary>
	private float scaleOculusInput = 1.0f;

	/// <summary>
	/// The sqr max velocity. In meters/second. Is the Bound to limit the velocity applying a force in reverse to velocity vector.
	/// </summary>
	private float sqrMaxVelocity = 0.125f;
	//m/s

	/// <summary>
	/// The move h. Movement in X axis
	/// </summary>
	private float moveH;

	/// <summary>
	/// The move v. Movement in Z axis
	/// </summary>
	private float moveV;

	/// <summary>
	/// My OVR cam game object. Is the object that indicates the forward direction and the right direction to update the new direction of the force vectors to move the player and the stareo Cameras.
	/// </summary>
	public GameObject myOVRCamGameObject;

	/// <summary>
	/// The first rot. To update OVRCamerRig when is back to moving player mode.
	/// </summary>
	private bool firstRot;

	/// <summary>
	/// The eye left game oject. To update OVRCamerRig when is back to moving player mode (rotate in reverse sense to the rotation o myOVRcameraRig)
	/// </summary>
	public GameObject eyeLeftGameOject;

	/// <summary>
	/// The eye right game oject.To update OVRCamerRig when is back to moving player mode (rotate in reverse sense to the rotation o myOVRcameraRig)
	/// </summary>
	public GameObject eyeRightGameOject;

	/// <summary>
	/// The eye center game object.To update OVRCamerRig when is back to moving player mode (rotate in reverse sense to the rotation o myOVRcameraRig)
	/// </summary>
	public GameObject eyeCenterGameObject;


	/// <summary>
	/// The cam. The camera in the space of the myOVRCamGameObject (or a simple camera to run in desktop mode without oculus).
	/// </summary>
	public Camera cam;

	/// <summary>
	/// The movement vector. moveH vector from right vector of camera. moveV vector from forward vector of camera.
	/// </summary>
	private Vector3 movement;

	/// <summary>
	/// The force to apply at current frame.
	/// </summary>
	private Vector3 forceApplying;

	/// <summary>
	/// The force to apply at next frame.
	/// </summary>
	private Vector3 forceApplyingNext;

	private Rigidbody rb;

	public bool useApplause;

	void Start ()
	{
		this.transform.position = new Vector3 (myOVRCamGameObject.transform.position.x, this.transform.position.y, myOVRCamGameObject.transform.position.z);
		moveH = 0.0f;
		moveV = 0.0f;
		rb = this.GetComponent<Rigidbody> ();
		firstRot = true;
		useApplause = true;
	}

	private void getInput ()
	{
		if (OVRManager.display != null && OVRManager.display.isPresent) {
			/// <summary>
			/// HeadPose orientation is for pitch, roll, and way. Display.acceleration is the acceleration of the Oculus moving in the room.
			/// </summary>
			moveH = -OVRManager.display.GetHeadPose ().orientation.z * scaleOculusInput;
			moveV = OVRManager.display.GetHeadPose ().orientation.x * scaleOculusInput;

			//limits H
			if(moveH > 0.0f) {
				moveH = (moveH < 0.075f)? 0.0f : moveH;
			}
			if(moveH < 0.0f){
				moveH = (moveH > -0.075f)? 0.0f : moveH;
			}

			//limits V
			if(moveV > 0.0f) {
				moveV = (moveV < 0.15f)? 0.0f : moveV;
			}
//			if(moveV < 0.0f){
//				moveV = (moveV > -0.2f)? 0.0f : moveV;
//			}

		} else {
			if (Application.platform == RuntimePlatform.Android ||
			    Application.platform == RuntimePlatform.IPhonePlayer) {
				moveH = Input.acceleration.x;    //accelerometer
				moveV = Input.acceleration.y * 0.5f - Input.acceleration.z * 0.5f;    //45 degrees inclination
			}
			//keys
			else {
				moveH = Input.GetAxis ("Horizontal");
				moveV = Input.GetAxis ("Vertical");
			}
		}
	}

	/// <summary>
	/// Rotates the view of the parent Game Object of the two stereo cameras of myOVRCameraRig. The rotation is a interpolation of the rotation of the oculus headpose.
	/// </summary>
	private void rotateView ()
	{
		if (OVRManager.display != null && OVRManager.display.isPresent) {

			//rotate the eyes in reverse sense to avoid jumping rotations
			if (firstRot) {
				firstRot = false;

				eyeLeftGameOject.transform.rotation = 
					Quaternion.Euler (
					eyeLeftGameOject.transform.rotation.eulerAngles.x,
					-OVRManager.display.GetHeadPose ().orientation.eulerAngles.y,
					eyeLeftGameOject.transform.rotation.eulerAngles.z
				);

				eyeRightGameOject.transform.rotation = 
					Quaternion.Euler (
					eyeRightGameOject.transform.rotation.eulerAngles.x,
					-OVRManager.display.GetHeadPose ().orientation.eulerAngles.y,
					eyeRightGameOject.transform.rotation.eulerAngles.z
				);

				eyeCenterGameObject.transform.rotation =
					Quaternion.Euler(
					eyeCenterGameObject.transform.rotation.eulerAngles.x,
					-OVRManager.display.GetHeadPose ().orientation.eulerAngles.y,
					eyeCenterGameObject.transform.rotation.eulerAngles.z
					);
			}

			myOVRCamGameObject.transform.rotation =
				Quaternion.Euler (
				myOVRCamGameObject.transform.rotation.eulerAngles.x,
				OVRManager.display.GetHeadPose ().orientation.eulerAngles.y,
				myOVRCamGameObject.transform.rotation.eulerAngles.z);    //no smooth
		}

		//keys
		else {
			
		}
	}

	private void getMovement ()
	{
		if (OVRManager.display != null && OVRManager.display.isPresent) {
			movement = (myOVRCamGameObject.transform.right * moveH + myOVRCamGameObject.transform.forward * moveV).normalized;
		}
		//keys
		else {
			if (moveH != 0.0f && moveV != 0.0f)
				movement = (cam.transform.right * moveH + cam.transform.forward * moveV).normalized;
			else
				movement = cam.transform.right * moveH + cam.transform.forward * moveV;
		}
	}

	/// <summary>
	/// Moves the player. Limiting its velocity applying reverse force.
	/// </summary>
	private void movePlayer ()
	{
		if (rb.velocity.magnitude > sqrMaxVelocity) {
			forceApplying = -rb.velocity.normalized * force * Time.deltaTime;
			forceApplyingNext = -rb.velocity.normalized * force * (Time.deltaTime + Time.deltaTime);
		} else {
			forceApplying = movement * force * Time.deltaTime;
			forceApplyingNext = movement * force * (Time.deltaTime + Time.deltaTime);
		}

		if (OVRManager.display != null && OVRManager.display.isPresent) {
			rb.AddForce (forceApplying, ForceMode.Force);   //No smooth (direct controll with head's user)
		}
		//keys
		else {
			rb.AddForce (Vector3.Lerp (forceApplying, forceApplyingNext, 0.8f), ForceMode.Force);   //smooth
		}
	}


	/// <summary>
	/// Updates the kinemactic. Based on aplause gesture.
	/// </summary>
	void UpdateKinemactic ()
	{
		if (useApplause) {
			if (ApplauseDetection.stateAppl == ApplauseDetection.ApplauseState.ON) {
				rb.isKinematic = true;
			} else {
				rb.isKinematic = false;
			}
		} else {
			if (Input.GetKeyDown (KeyCode.Space)) {
				rb.isKinematic = !rb.isKinematic;
			}
		}
	}


	/// <summary>
	/// Fixeds the update. The update of physics will be performed with a fixed rate (althought not draw at same rate).
	/// </summary>
	void FixedUpdate ()
	{
//		UpdateKinemactic();
//
//		if (!rb.isKinematic) {
//			getInput ();
//			//rotateView ();
//			getMovement ();
//			movePlayer ();
//		} else {
//			firstRot = true;
//		}
	}
}
