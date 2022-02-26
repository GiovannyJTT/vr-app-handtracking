using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DBTfm;
using DBTfm.Types;
using UITfm;
using UITfm.Types;
using APPTfm;

public class UI_InsertButtonController : MonoBehaviour
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
		DB_Person p = UI_Manager.Instance.getUIPerson ();
		switch (UI_Manager.ui_state) {
		case UI_STATE.CREATING_PERSON:
			if (p.code != -1 && p.name != "" && p.age != -1 && p.sex != SEX.EMPTY) {
				DB_Manager.Instance.insertPerson (p);
				//DB_Person insertedPerson = DB_Manager.Instance.getPersonsList().Find(x => x.code == p.code);
				DB_Person insertedPerson = DB_Manager.Instance.getOnePerson(p.code);
				if(insertedPerson != null){
					APP_Manager.Instance.setCurrentPerson(insertedPerson);
				} else {
					Debug.Log("Error: insertPerson failed");
				}
				UI_Manager.Instance.configureInputPanelsExistingCode (APP_Manager.Instance.getCurrentPerson());
				UI_Manager.ui_state = UI_STATE.EXISTING_CODE;
			} else {
				UI_Manager.Instance.showInputFBMessage("Faltan datos");
			}

			break;
		case UI_STATE.MODIFYING_PERSON:
			if (p.code != -1 && p.name != "" && p.age != -1 && p.sex != SEX.EMPTY) {
				DB_Manager.Instance.updatePerson (p);
				//DB_Person updatedPerson = DB_Manager.Instance.getPersonsList().Find(x => x.code == p.code);
				DB_Person updatedPerson = DB_Manager.Instance.getOnePerson(p.code);
				if(updatedPerson != null){
					APP_Manager.Instance.setCurrentPerson(updatedPerson);
				} else {
					Debug.Log("Error: updatePerson failed");
				}
				UI_Manager.Instance.configureInputPanelsExistingCode (APP_Manager.Instance.getCurrentPerson());
				UI_Manager.ui_state = UI_STATE.EXISTING_CODE;
			} else {
				UI_Manager.Instance.showInputFBMessage("Faltan datos");
			}
			break;
		}
	}
}
