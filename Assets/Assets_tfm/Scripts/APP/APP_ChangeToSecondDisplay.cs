using UnityEngine;
using System.Collections;
/// <summary>
/// Changes the display of the camera.
/// </summary>
public class APP_ChangeToSecondDisplay : MonoBehaviour {
	void Start () {
		// Display.displays[0] is the primary, default display and is always ON.
		if(Display.displays.Length > 1){
			//Only can activate on awake and can't deactivate it.
			Display.displays[1].Activate();
			this.GetComponent<Camera>().targetDisplay = 1;
			this.GetComponent<Camera>().enabled = true;
		}
	}
}
