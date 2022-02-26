using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm.Types;

namespace UITfm
{
	public class UI_ExitButtonController : MonoBehaviour
	{
		private Button b;

		void Start ()
		{
			b = this.GetComponent<Button> ();

			b.onClick.AddListener (
				() => updateUI ()
			);
		}

		private void updateUI ()
		{
			switch (UI_Manager.ui_state) {
			case UI_STATE.INIT:
				Application.Quit ();
				break;
			}
		}
	}
}