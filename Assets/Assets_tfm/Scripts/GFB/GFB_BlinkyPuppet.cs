using UnityEngine;
using System.Collections;

public class GFB_BlinkyPuppet : MonoBehaviour {
	private enum GFB_BLINKYPUPPET_STATE {
		OFF = 0,
		ON = 1
	}
	private GFB_BLINKYPUPPET_STATE blinkypuppet_state;

	private MeshRenderer mr;
	private HRTManager hrtm;
	private Color origColor;
	private Color blinkyColor;

	void Start () {
		mr = this.GetComponent<MeshRenderer>();
		blinkypuppet_state = GFB_BLINKYPUPPET_STATE.OFF;
		origColor = mr.material.color;
		blinkyColor = Color.blue;

		//[0] blinky seconds
		hrtm = new HRTManager();
		hrtm.ResetSamplingFrequency();
		hrtm.CreateTimers(1);
	}

	void OnCollisionEnter(Collision cls){
		blinkypuppet_state = GFB_BLINKYPUPPET_STATE.ON;
		mr.material.color = blinkyColor;
		hrtm.Timers[0].InitCounting();
	}

	void Update () {
		switch(this.blinkypuppet_state){
			case GFB_BLINKYPUPPET_STATE.OFF:
			break;
			case GFB_BLINKYPUPPET_STATE.ON:
				hrtm.Timers[0].EndCounting();
				if(hrtm.Timers[0].GetLastPeriodSecs() > 0.1f){    //100ms
					mr.material.color = origColor;
					blinkypuppet_state = GFB_BLINKYPUPPET_STATE.OFF;
				}
			break;
		}
	}
}
