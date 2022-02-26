using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm;
using UITfm.Types;
using APPTfm;

public class UI_BackButtonController : MonoBehaviour
{

	private Button b;

	void Start ()
	{
		b = this.GetComponent<Button> ();

		b.onClick.AddListener (
			() => updateUI ()
		);
	}

	private void jumpBack(){
		UI_Manager.Instance.configureInputPanelsExistingCode (APP_Manager.Instance.getCurrentPerson ());

		UI_Manager.Instance.mainPanels [(int)UI_MAIN_PANEL.INFO].SetActive (true);
		UI_Manager.Instance.mainPanels [(int)UI_MAIN_PANEL.INPUT].SetActive (true);
		UI_Manager.Instance.mainPanels [(int)UI_MAIN_PANEL.SESSION].SetActive (false);

		UI_Manager.Instance.updateInfoPanel(null);

		UI_Manager.ui_state = UI_STATE.EXISTING_CODE;
	}

	private void updateUI ()
	{
		switch (UI_Manager.ui_state) {
		case UI_STATE.SELECTING_GSNUMBER:
//			Debug.Log("back from selecting gsnumber");
			jumpBack();
			break;
		case UI_STATE.EXISTING_GSNUMBER:
//			Debug.Log("back from existing gsnumber");
			jumpBack();
			break;
		case UI_STATE.NEW_GSNUMBER:
//			Debug.Log("back from new gsnumber");
			jumpBack();
			break;
		}
	}

}
