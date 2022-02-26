using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm.Types;

namespace UITfm
{
	public class UI_CreateButtonController : MonoBehaviour
	{
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
			case UI_STATE.NEW_CODE:
				UI_Manager.Instance.resetAllInputPanels(int.Parse(UI_Manager.Instance.textCode.text), Color.red, true);

				UI_Manager.Instance.setAllInputPanels(true);
				UI_Manager.Instance.setExclusiveInputSubPanel((int)UI_INPUT_SUBPANEL.CODE, false);
				UI_Manager.Instance.setExclusiveInputButton((int)UI_INPUT_BUTTON.INSERT, true);
				UI_Manager.Instance.setInputButton((int)UI_INPUT_BUTTON.CANCEL, true);
				UI_Manager.ui_state = UI_STATE.CREATING_PERSON;
				break;
			}
		}
	}
}
