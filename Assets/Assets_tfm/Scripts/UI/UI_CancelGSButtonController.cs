using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm;
using UITfm.Types;
using APPTfm;

public class UI_CancelGSButtonController : MonoBehaviour {
	
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
		case UI_STATE.CREATING_GSESSION:
			UI_Manager.Instance.setExclusiveGSPanel((int)UI_GSESSION_PANEL.CURRENT_PERSON, true);
			UI_Manager.Instance.setGSPanel((int)UI_GSESSION_PANEL.SESSION_NUMBER, true);
			UI_Manager.Instance.setExclusiveGSSubPanel((int)UI_GSESSION_SUBPANEL.GSNUMBER, true);
			UI_Manager.Instance.setExclusiveGSButton((int)UI_GSESSION_BUTTON.BACK, true);
			UI_Manager.ui_state = UI_STATE.SELECTING_GSNUMBER;
			break;
		case UI_STATE.MODIFYING_GSESSION:
			UI_Manager.Instance.configureGSPanelsExistingGSNumber(APP_Manager.Instance.getCurrentGameSession());
			UI_Manager.ui_state = UI_STATE.EXISTING_GSNUMBER;
			break;
		}
	}
}
