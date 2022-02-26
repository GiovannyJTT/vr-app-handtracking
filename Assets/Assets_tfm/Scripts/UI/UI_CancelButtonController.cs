using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm;
using UITfm.Types;
using APPTfm;

public class UI_CancelButtonController : MonoBehaviour {
	
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
		case UI_STATE.CREATING_PERSON:
			UI_Manager.Instance.setExclusiveInputPanel((int)UI_INPUT_PANEL.CODE, true);
			UI_Manager.Instance.setExclusiveInputSubPanel((int)UI_INPUT_SUBPANEL.CODE, true);
			UI_Manager.Instance.setAllInputButtons(false);
			UI_Manager.ui_state = UI_STATE.SELECTING_CODE;
			break;
		case UI_STATE.MODIFYING_PERSON:
			UI_Manager.Instance.configureInputPanelsExistingCode(APP_Manager.Instance.getCurrentPerson());
			UI_Manager.ui_state = UI_STATE.EXISTING_CODE;
			break;
		}
	}
}
