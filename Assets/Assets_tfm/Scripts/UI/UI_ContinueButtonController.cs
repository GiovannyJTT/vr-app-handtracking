using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm;
using UITfm.Types;
using DBTfm;
using DBTfm.Types;

public class UI_ContinueButtonController : MonoBehaviour {
	private Button b;
	void Start () {
		b = this.GetComponent<Button>();

		b.onClick.AddListener(
			() => updateUI()
			);
	}

	private void updateUI () {		
		switch (UI_Manager.ui_state) {
		case UI_STATE.EXISTING_CODE:
			UI_Manager.Instance.mainPanels[(int)UI_MAIN_PANEL.INFO].SetActive(true);
			UI_Manager.Instance.mainPanels[(int)UI_MAIN_PANEL.INPUT].SetActive(false);
			UI_Manager.Instance.mainPanels[(int)UI_MAIN_PANEL.SESSION].SetActive(true);

			UI_Manager.Instance.initGSPanels(-1, Color.black);
			UI_Manager.Instance.initGSButtons();

			UI_Manager.ui_state = UI_STATE.SELECTING_GSNUMBER;
			break;
		}
	}
}
