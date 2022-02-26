using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DBTfm;
using DBTfm.Types;
using UITfm;
using UITfm.Types;
using APPTfm;

public class UI_InsertGSButtonController : MonoBehaviour
{
	private Button b;

	void Start ()
	{
		b = this.GetComponent<Button> ();
		b.onClick.AddListener (
			() => updateUI ()
		);
	}

	public void updateUI ()
	{
		///Load from UI because n insertion can be done in creation new or modifiying existing session modes.
		DB_GameSession gs = UI_Manager.Instance.getUIGSession ();

		switch (UI_Manager.ui_state) {
		case UI_STATE.CREATING_GSESSION:
			if (gs.session_number != -1 && gs.hci_type != HCI_TYPE.NONE) {
				DB_Manager.Instance.insertGSession (gs);
				DB_GameSession insertedGSession = DB_Manager.Instance.getOneGSession(gs.id_person, gs.id_game, gs.session_number);
				if(insertedGSession != null){
					APP_Manager.Instance.setCurrentGameSession(insertedGSession);
					UI_Manager.Instance.configureGSPanelsExistingGSNumber (APP_Manager.Instance.getCurrentGameSession());
					UI_Manager.Instance.updateInfoPanel(APP_Manager.Instance.getCurrentGameSession());
					UI_Manager.ui_state = UI_STATE.EXISTING_GSNUMBER;
				} else {
					Debug.Log("Error: insertGSession failed");
				}
			} else {
				UI_Manager.Instance.showGSFBMessage("Faltan datos");
			}
			break;
		case UI_STATE.MODIFYING_GSESSION:
			if (gs.session_number != -1 && gs.hci_type != HCI_TYPE.NONE) {
				DB_Manager.Instance.updateGSession (gs);
				DB_GameSession updatedGSession = DB_Manager.Instance.getOneGSession(gs.id_person, gs.id_game, gs.session_number);
				if(updatedGSession != null){
					APP_Manager.Instance.setCurrentGameSession (updatedGSession);
					UI_Manager.Instance.configureGSPanelsExistingGSNumber (APP_Manager.Instance.getCurrentGameSession());
					UI_Manager.Instance.updateInfoPanel(APP_Manager.Instance.getCurrentGameSession());
					UI_Manager.ui_state = UI_STATE.EXISTING_GSNUMBER;
				} else {
					Debug.Log("Error: updateGSession failed");
				}
			} else {
				UI_Manager.Instance.showGSFBMessage("Faltan datos");
			}
			break;
		}
	}
}
