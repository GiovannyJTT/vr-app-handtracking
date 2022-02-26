using UnityEngine;
using System.Collections;

/// <summary>
/// TFM - Uses raycast to enable/disable MyToggleButtons. Cast a ray from main camera to collider of MyToggleButton.
/// </summary>
public class MyButtonToggleRaycast : MonoBehaviour {
	public Camera cameraFrom;

	void Update ()
	{
		if (Input.GetMouseButtonDown (0) && cameraFrom != null && !OVRManager.display.isPresent) { //also works for touch
			RaycastHit hitInfo = new RaycastHit ();
			bool colision = Physics.Raycast (cameraFrom.ScreenPointToRay (Input.mousePosition), out hitInfo);
			if (colision && hitInfo.transform.gameObject == this.gameObject) {
				MyButtonToggle mtb = this.GetComponentInParent<MyButtonToggle>();
				if(!mtb.getIsOn()) mtb.ButtonTurnsOn();
				else mtb.ButtonTurnsOff();
			}
		}
	}
}