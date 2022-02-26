using UnityEngine;
using System.Collections;
using DBTfm;
using DBTfm.Types;
using APPTfm;

public class APP_CameraController : MonoBehaviour
{
	private DB_GameSession gs;
	public GameObject camOculus;
	public GameObject camNvidia;

	private bool firstPoseReset;
	private HRTManager hrtm;

	void Start ()
	{
		gs = APP_Manager.Instance.getCurrentGameSession ();
		if (gs != null)
			toggleCam (gs.hci_type);
		else
			toggleCam (HCI_TYPE.LM_AND_NVIDIA);

		firstPoseReset = true;
		//Create Timers
		//[0] seconds to recenter the oculu1.s pose
//		hrtm = new HRTManager ();
//		hrtm.ResetSamplingFrequency ();
//		hrtm.CreateTimers (1);
//		hrtm.Timers [0].InitCounting ();
	}

	private void recenterOculusPose ()
	{
		if (OVRManager.tracker.isPresent) {
			OVRManager.capiHmd.RecenterPose ();
		}
	}

	public void toggleCam (HCI_TYPE hci)
	{
		if (hci == HCI_TYPE.LM_AND_NVIDIA) {
			camOculus.SetActive (false);
			camNvidia.SetActive (true);
		} else {
			camOculus.SetActive (true);
			camNvidia.SetActive (false);
		}
	}

//	void Update ()
//	{
//		hrtm.Timers [0].EndCounting ();
//		if (gs != null && gs.hci_type == HCI_TYPE.LM_AND_OCULUS && firstPoseReset && hrtm.Timers [0].GetLastPeriodSecs () > 0.25f) {    //250ms after start
//			firstPoseReset = false;
//			recenterOculusPose ();
//		}
//	}
}
