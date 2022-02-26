using UnityEngine;
using System.Collections;
using Leap;

public class ApplauseDetection : MonoBehaviour
{
	public static bool applausing;

	public enum ApplauseState
	{
		ON,
		OFF}
	;

	public static ApplauseState stateAppl;
	private static ApplauseState stateApplLast;

	/// <summary>
	/// The Controller for get frames from Leap System.
	/// </summary>
	private Controller c;

	void Start ()
	{
		applausing = false;    //needs star in false
		stateAppl = ApplauseState.ON;   //needs start in ON mode
		stateApplLast = ApplauseState.OFF;    //needs start in OFF mode
		c = new Controller ();
	}

	void UpdateStateAppl ()
	{
		switch (stateAppl) {
		case ApplauseState.OFF:
			if (applausing && stateApplLast != ApplauseState.ON) {    //avoid bucle
				stateApplLast = stateAppl;    //keep last
				stateAppl = ApplauseState.ON;    //update
			} else {
				if (!applausing) {
					stateApplLast = stateAppl;    //keep last
				}
			}
			break;
		case ApplauseState.ON:
			if (applausing && stateApplLast != ApplauseState.OFF) {    //avoid bucle
				stateApplLast = stateAppl;    //keep last
				stateAppl = ApplauseState.OFF;    //update
			} else {
				if (!applausing) {
					stateApplLast = stateAppl;    //kepp last
				}
			}
			break;
		}
	}

	void FixedUpdate ()
	{
		Frame f = c.Frame ();
		HandList hl = f.Hands;
		if (hl.Count == 2) {   //iff 2 hands
			if ((hl [0].IsLeft && hl [1].IsRight) || (hl [0].IsRight && hl [1].IsLeft)) {
				//Debug.Log("2 hands");

				if (hl [0].GrabStrength < 0.25f && hl [1].GrabStrength < 0.25f) {   //totally opened hand (strength == 0)
					//Debug.Log("2 opened");

					//Debug.Log(hl [0].PalmPosition.DistanceTo (hl [1].PalmPosition).ToString() );

					if (hl [0].PalmPosition.DistanceTo (hl [1].PalmPosition) < 100.0f) {    //mm
						//Debug.Log("2 in distance less than 70mm");

						float angleInRadians = hl [0].PalmNormal.Normalized.AngleTo (hl [1].PalmNormal.Normalized);
						//Debug.Log(angleInRadians.ToString());

						if (angleInRadians > 2.35619449f) {    //opposite directions, >135º
							//Debug.Log ("Applausing ON " + angleInRadians.ToString());
							applausing = true;
						} else
							applausing = false;
					} else
						applausing = false;
				} else
					applausing = false;
			} else
				applausing = false;
		}

		UpdateStateAppl ();
	}
}
