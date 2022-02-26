using UnityEngine;
using System.Collections;

public class GTB_BlinkyAreaCollider : MonoBehaviour
{
	private enum GTB_BLINKY_STATE
	{
		OFF = 0,
		START_BLINKY = 1,
		ON = 2
	}

	private GTB_BLINKY_STATE blinky_state;

	private MeshRenderer mr;
	private HRTManager hrtm;

	// Use this for initialization
	void Start ()
	{
		mr = this.GetComponent<MeshRenderer> ();
		mr.enabled = false;
		//[0] seconds before reset the color of the blinky area collider
		hrtm = new HRTManager ();
		hrtm.ResetSamplingFrequency ();
		hrtm.CreateTimers (2);
	}

	void OnTriggerEnter (Collider cld)
	{
		if (blinky_state == GTB_BLINKY_STATE.OFF) {
			blinky_state = GTB_BLINKY_STATE.START_BLINKY;
		}
	}

	void OnCollisionEnter (Collision cls)
	{
		if (blinky_state == GTB_BLINKY_STATE.OFF) {
			blinky_state = GTB_BLINKY_STATE.START_BLINKY;
		}
	}

	void Update ()
	{
		switch (blinky_state) {
		case GTB_BLINKY_STATE.OFF:
			break;
		case GTB_BLINKY_STATE.START_BLINKY:
			hrtm.Timers[0].InitCounting();
			mr.enabled = true;
			blinky_state = GTB_BLINKY_STATE.ON;
			break;
		case GTB_BLINKY_STATE.ON:
			hrtm.Timers[0].EndCounting();
			if(hrtm.Timers[0].GetLastPeriodSecs() > 0.25f){
				mr.enabled = false;
				blinky_state = GTB_BLINKY_STATE.OFF;
			}
			break;
		}
	}
}
