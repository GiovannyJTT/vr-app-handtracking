using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm;
using UITfm.Types;

public class UI_ModifyButtonController : MonoBehaviour {
	
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
		case UI_STATE.EXISTING_CODE:
			UI_Manager.Instance.setAllInputPanels(true);
			UI_Manager.Instance.setExclusiveInputSubPanel((int)UI_INPUT_SUBPANEL.CODE, false);
			UI_Manager.Instance.setExclusiveInputButton((int)UI_INPUT_BUTTON.CANCEL, true);
			UI_Manager.Instance.setInputButton((int)UI_INPUT_BUTTON.INSERT, true);
			UI_Manager.ui_state = UI_STATE.MODIFYING_PERSON;
			break;
		}
	}
}
