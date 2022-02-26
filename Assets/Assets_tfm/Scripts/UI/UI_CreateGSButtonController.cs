using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm.Types;
using APPTfm;

namespace UITfm
{
	public class UI_CreateGSButtonController : MonoBehaviour
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
			case UI_STATE.NEW_GSNUMBER:
				UI_Manager.Instance.setAllGSPanels(true);
				UI_Manager.Instance.setExclusiveGSSubPanel((int)UI_GSESSION_SUBPANEL.GSNUMBER, false);

				UI_Manager.Instance.setExclusiveGSGImage( (APP_Manager.Instance.getCurrentGame().ordinal-1) % UI_Manager.Instance.gSGImages.Length, true);

				UI_Manager.Instance.setGSHCIInteractable(true);
				UI_Manager.Instance.setExclusiveGSButton((int)UI_GSESSION_BUTTON.CANCELGS, true);
				UI_Manager.Instance.setGSButton((int)UI_GSESSION_BUTTON.INSERTGS, true);
				
				UI_Manager.ui_state = UI_STATE.CREATING_GSESSION;
				break;
			}
		}
	}
}
