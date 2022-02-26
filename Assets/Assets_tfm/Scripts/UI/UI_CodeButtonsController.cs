using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DBTfm;
using UITfm.Types;

namespace UITfm
{
	public class UI_CodeButtonsController : MonoBehaviour
	{
		public GameObject codeButtonPrefab;
		//to write in feedback panel
		private static Button[] bArray;

		void Start ()
		{
			bArray = new Button[100];
			for (int i = 0; i < bArray.Length; i++) {
				int code = i;
				bArray [i] = Instantiate (codeButtonPrefab).GetComponent<Button> ();
				bArray [i].transform.SetParent (this.transform);

				bArray [i].name = (code).ToString ("D3");
				bArray [i].GetComponentInChildren<Text> ().text = code.ToString ("D3");

				bArray [i].onClick.AddListener (() => updateUI (code));    //the parameter passed must be a new temp variable
			}
		}

		private void toggleConfigByCode (int code, DB_Person p)
		{
			if (p != null) {
				UI_Manager.Instance.configureInputPanelsExistingCode (p);
				UI_Manager.ui_state = UI_STATE.EXISTING_CODE;
			} else {
				UI_Manager.Instance.configureInputPanelsNewCode (code);
				UI_Manager.ui_state = UI_STATE.NEW_CODE;
			}
		}

		private void updateUI (int code)
		{
			DB_Person p = DB_Manager.Instance.getOnePerson(code);

			switch (UI_Manager.ui_state) {
			case UI_STATE.SELECTING_CODE:
				toggleConfigByCode (code, p);
				break;
			case UI_STATE.EXISTING_CODE:
				toggleConfigByCode (code, p);
				break;
			case UI_STATE.NEW_CODE:
				toggleConfigByCode (code, p);
				break;
			}
		}
	}
}
