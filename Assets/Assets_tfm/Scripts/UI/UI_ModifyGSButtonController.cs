using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm;
using UITfm.Types;

public class UI_ModifyGSButtonController : MonoBehaviour {
	
	private Button b;
	void Start ()
	{
		b = this.GetComponent<Button>();

		b.onClick.AddListener(
			() => updateUI()
			);
	}

	private void updateUI(){
		switch (UI_Manager.ui_state) {
		case UI_STATE.EXISTING_GSNUMBER:
			UI_Manager.Instance.setAllGSPanels(true);
			UI_Manager.Instance.setExclusiveGSSubPanel((int) UI_GSESSION_SUBPANEL.GSNUMBER, false);
			UI_Manager.Instance.setExclusiveGSButton((int) UI_GSESSION_BUTTON.CANCELGS, true);
			UI_Manager.Instance.setGSButton((int) UI_GSESSION_BUTTON.INSERTGS, true);
			UI_Manager.Instance.setGSHCIInteractable(true);
			UI_Manager.ui_state = UI_STATE.MODIFYING_GSESSION;
			break;
		}
	}
}
