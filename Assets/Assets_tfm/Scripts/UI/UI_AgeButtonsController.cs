using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm;
using UITfm.Types;

namespace UITfm
{
	public class UI_AgeButtonsController : MonoBehaviour
	{
		public GameObject ageButtonPrefab;
		//to write in feedback panel
		public static Button[] bArray;

		void Start ()
		{
			bArray = new Button[87];
			for (int i = 0; i < bArray.Length; i++) {
				int age = (i + 4);
				bArray [i] = Instantiate (ageButtonPrefab).GetComponent<Button> ();
				bArray [i].transform.SetParent (this.transform);

				bArray [i].name = (age).ToString ();
				bArray [i].GetComponentInChildren<Text> ().text = age.ToString ();

				int index = i;
				bArray [i].onClick.AddListener (() => updateUI (index));    //the parameter passed must be a new temp variable
			}
		}

		private void updateUI (int i)
		{
			switch (UI_Manager.ui_state) {
			case UI_STATE.CREATING_PERSON:
				UI_Manager.Instance.textAge.text = bArray [i].GetComponentInChildren<Text> ().text;
				break;
			case UI_STATE.MODIFYING_PERSON:
				UI_Manager.Instance.textAge.text = bArray [i].GetComponentInChildren<Text> ().text;
				break;
			}
		}
	}
}
