using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// TFM - Identify gesture and print string in a 3D Text.
/// It Needs be attached to a 3DText
/// </summary>
public class GesturesIdentification : MonoBehaviour {
	/// <summary>
	/// The g. contains a string with gesture name recognited
	/// </summary>
	private string gText;
	private TextMesh tm;

	private Controller c;
	
	void Start () {
		gText = "---";
		tm = this.GetComponent<TextMesh>();
		tm.text = gText;
		c = new Controller();
	}

	void Update () {
		Frame f = c.Frame();
		GestureList gs = f.Gestures();
		foreach(Gesture g in gs){
			switch(g.Type){
			case Gesture.GestureType.TYPE_CIRCLE:
				CircleGesture cir = new CircleGesture(g);
				if(cir.Pointable.Direction.AngleTo(cir.Normal) <= 1.5707963267f) gText = "Circle CW";
				else gText = "Circle CCW";
				break;

			case Gesture.GestureType.TYPE_SWIPE:
				SwipeGesture swp = new SwipeGesture(g);
				if(c.PolicyFlags == Controller.PolicyFlag.POLICY_OPTIMIZE_HMD){
					if (swp.Direction.x > 0) gText = "Swipe Left";
					else if(swp.Direction.x < 0) gText = "Swipe Right";
				}
				else {
					if (swp.Direction.x < 0) gText = "Swipe Left";
					else if(swp.Direction.x > 0) gText = "Swipe Right";
				}
				break;

			case Gesture.GestureType.TYPE_SCREEN_TAP:
				gText = "ScreenTap";
				break;
			case Gesture.GestureType.TYPE_KEY_TAP:
				gText = "KeyTaps";
				break;
			case Gesture.GestureType.TYPE_INVALID:
				gText = "---";
				break;
			}

			tm.text = gText;
			tm.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
		}
	}
}
