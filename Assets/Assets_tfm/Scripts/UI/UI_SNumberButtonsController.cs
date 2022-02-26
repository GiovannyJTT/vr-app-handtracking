using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm;
using UITfm.Types;
using DBTfm;
using DBTfm.Types;
using APPTfm;

namespace UITfm
{
	public class UI_SNumberButtonsController : MonoBehaviour
	{
		public GameObject sNumberButtonPrefab;
		//to write in feedback panel
		public static Button[] bArray;

		void Start ()
		{
			bArray = new Button[6];
			for (int i = 0; i < bArray.Length; i++) {
				int snumber = i;
				bArray [i] = Instantiate (sNumberButtonPrefab).GetComponent<Button> ();
				bArray [i].transform.SetParent (this.transform);

				bArray [i].name = (snumber).ToString ();
				bArray [i].GetComponentInChildren<Text> ().text = snumber.ToString ();

				//colors
				if (i < 3) {
					bArray [i].GetComponentInChildren<Text> ().color = Color.blue;
				} else {
					bArray [i].GetComponentInChildren<Text> ().color = Color.green;
				}

				bArray [i].onClick.AddListener (() => updateUI (snumber));    //the parameter passed must be a new temp variable
			}
		}

		private void toggleGSConfigByGSNumber (int sn, DB_GameSession gs)
		{
			if (gs != null) {
				UI_Manager.Instance.configureGSPanelsExistingGSNumber (gs);
				UI_Manager.ui_state = UI_STATE.EXISTING_GSNUMBER;
			} else {
				UI_Manager.Instance.configureGSPanelsNewGSNumber (sn);
				UI_Manager.ui_state = UI_STATE.NEW_GSNUMBER;
			}
		}

		private void updateUI (int sn)
		{

			APP_Manager.Instance.setCurrentGame (DB_Manager.Instance.getOneGame ((sn % 3) + 1));    //+1 because the ids of the bd start in 1
//			Debug.Log(APP_Manager.Instance.getCurrentGame().goal_description);

			DB_GameSession gs = DB_Manager.Instance.getOneGSession (
				                    APP_Manager.Instance.getCurrentPerson ().id,
				                    APP_Manager.Instance.getCurrentGame ().id,
				                    sn
			                    );

			UI_Manager.Instance.updateInfoPanel (gs);

			switch (UI_Manager.ui_state) {
			case UI_STATE.SELECTING_GSNUMBER:
				toggleGSConfigByGSNumber (sn, gs);
				break;
			case UI_STATE.EXISTING_GSNUMBER:
				toggleGSConfigByGSNumber (sn, gs);
				break;
			case UI_STATE.NEW_GSNUMBER:
				toggleGSConfigByGSNumber (sn, gs);
				break;
			}
		}
	}
}
