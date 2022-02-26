using UnityEngine;
using System.Collections;
using Leap;
using APPTfm;
using DBTfm;
using DBTfm.Types;

public class APP_Tutorial_PlaybackSpeed : MonoBehaviour
{
	private HandController hc;
	public float speedFor120fps_nvidia;
	public float speedFor75fps_oculus;
	public float speedFor60fps;

	void Start ()
	{
		hc = this.GetComponent<HandController> ();

		DB_GameSession gs = APP_Manager.Instance.getCurrentGameSession ();

		if (gs != null) {
			if (gs.hci_type == HCI_TYPE.LM_AND_NVIDIA) {
				hc.recorderSpeed = speedFor120fps_nvidia;   //120 fps
//				Debug.Log ("recorderSpeed for 120fps");
			} else {
				hc.recorderSpeed = speedFor75fps_oculus;     //75 fps
//				Debug.Log ("recorderSpeed for 75fps");
			}
		} else {
//			Debug.Log ("recorderSpeed for 60fps");
			hc.recorderSpeed = speedFor60fps;    //60 fps
		}
	}
	
}
