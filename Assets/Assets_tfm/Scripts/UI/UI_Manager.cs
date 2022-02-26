using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using APPTfm;
using DBTfm;
using DBTfm.Types;
using UITfm.Types;

namespace UITfm
{
	namespace Types
	{
		public enum UI_STATE
		{
			INIT = 0,
			SELECTING_CODE = 1,
			NEW_CODE = 2,
			EXISTING_CODE = 3,
			CREATING_PERSON = 4,
			MODIFYING_PERSON = 5,
			SELECTING_GSNUMBER = 6,
			EXISTING_GSNUMBER = 7,
			NEW_GSNUMBER = 8,
			CREATING_GSESSION = 9,
			MODIFYING_GSESSION = 10,
			DISABLED = 11
		}

		public enum UI_INPUT_PANEL
		{
			CODE = 0,
			NAME = 1,
			AGE = 2,
			SEX = 3
		}

		public enum UI_INPUT_SUBPANEL
		{
			CODE = 0,
			NAME = 1,
			AGE = 2,
			SEX = 3
		}

		public enum UI_INPUT_BUTTON
		{
			CREATE = 0,
			INSERT = 1,
			CANCEL = 2,
			MODIFY = 3,
			CONTINUE = 4,
			EXIT = 5
		}

		public enum UI_FB_STATE
		{
			IDLE = 0,
			SHOWING_FB_MESSAGE = 1
		}

		public enum UI_MAIN_PANEL
		{
			INFO = 0,
			INPUT = 1,
			SESSION = 2
		}

		public enum UI_GSESSION_PANEL
		{
			CURRENT_PERSON = 0,
			SESSION_NUMBER = 1,
			GAME = 2,
			HCI = 3
		}

		public enum UI_GSESSION_SUBPANEL
		{
			GSNUMBER = 0,
			GSGIMAGES = 1,
			GSHCI = 2
		}

		public enum UI_GSGIMAGE
		{
			GSGI_MUSEUM = 0,
			GSGI_FOOTBALL = 1,
			GSGI_CLOCK = 2
		}

		public enum UI_GSESSION_BUTTON
		{
			BACK = 0,
			CREATEGS = 1,
			CANCELGS = 2,
			MODIFYGS = 3,
			INSERTGS = 4,
			PLAYGS = 5
		}
	}

	public class UI_Manager : MonoBehaviour
	{
		private static UI_Manager _instance = null;

		public static UI_Manager Instance {
			get {
				if (_instance == null) {
					GameObject go = new GameObject ("UI_Manager_GO");
					GameObject.DontDestroyOnLoad (go);
					go.AddComponent<UI_Manager> ();
				}
				return _instance;
			}
		}

		public static UI_FB_STATE ui_fb_state;
		public Text textInputFeedBack;
		public Text textGSFeedBack;
		private static HRTManager hrtm;

		public static UI_STATE ui_state;

		public Text textInfoDevices;
		public Text textCode;
		public Text textAge;

		public GameObject[] inputPanels;
		public GameObject[] inputSubPanels;
		public GameObject[] buttons;
		public GameObject[] mainPanels;
		public GameObject[] gSessionPanels;
		public GameObject[] gSessionSubPanels;
		public GameObject[] gSessionButtons;
		public GameObject[] gSGImages;

		private APP_Info dev_info;

		void Awake ()
		{
//			Debug.Log("UI Awake");

			_instance = this;
			dev_info = new APP_Info ();
			//Create Timers
			//[0] Timer for count seconds until restore the UI_FB_STATE.
			hrtm = new HRTManager ();
			hrtm.ResetSamplingFrequency ();
			hrtm.CreateTimers (1);
		}

		void Start ()
		{
//			Debug.Log("UI Start");
			initUI ();

			if (!DB_Manager.Instance.checkDBUriFile ()) {
                textInputFeedBack.text = DB_Manager.getDBURI();
				setAllInputPanels (false);
				setExclusiveInputButton ((int)UI_INPUT_BUTTON.EXIT, true);

				ui_state = UI_STATE.INIT;
			}

			updateInfoPanel (null);
		}

		public void initUI ()
		{
			mainPanels [(int)UI_MAIN_PANEL.INFO].SetActive (true);
			mainPanels [(int)UI_MAIN_PANEL.INPUT].SetActive (true);
			mainPanels [(int)UI_MAIN_PANEL.SESSION].SetActive (false);

			setExclusiveInputPanel ((int)UI_INPUT_PANEL.CODE, true);
			setAllInputButtons (false);
			ui_state = UI_STATE.SELECTING_CODE;
			textInputFeedBack.text = "Datos del Participante";
			ui_fb_state = UI_FB_STATE.IDLE;
		}

		public void updateInfoPanel (DB_GameSession gs)
		{
			textInfoDevices.text = dev_info.getDevicesInfo (gs);
		}

		public void showInputFBMessage (string m)
		{
			textInputFeedBack.GetComponent<Text> ().text = m;
			hrtm.Timers [0].InitCounting ();
			ui_fb_state = UI_FB_STATE.SHOWING_FB_MESSAGE;
		}

		void inputFeedBackText_Fsm ()
		{
			switch (ui_fb_state) {
			case UI_FB_STATE.IDLE:
				break;
			case UI_FB_STATE.SHOWING_FB_MESSAGE:
				hrtm.Timers [0].EndCounting ();
				if (hrtm.Timers [0].GetLastPeriodSecs () > 1.0f) {
					hrtm.Timers [0].InitCounting ();    //reset
					textInputFeedBack.text = "Datos del Participante";
					ui_fb_state = UI_FB_STATE.IDLE;
				}
				break;
			}
		}


		/// <summary>
		/// Sets the input button.
		/// </summary>
		/// <param name="bt">Bt.</param>
		/// <param name="b">If set to <c>true</c> b.</param>

		public void setInputButton (int bt, bool b)
		{
			buttons [bt].SetActive (b);
		}

		public void setExclusiveInputButton (int bt, bool b)
		{
			for (int i = 0; i < buttons.Length; i++)
				if (i == bt)
					setInputButton (i, b);
				else
					setInputButton (i, !b);
		}

		public void setAllInputButtons (bool b)
		{
			for (int i = 0; i < buttons.Length; i++)
				setInputButton (i, b);
		}


		private void setInputPanel (int p, bool b)
		{
			inputPanels [p].SetActive (b);
		}

		public void setExclusiveInputPanel (int p, bool b)
		{
			for (int i = 0; i < inputPanels.Length; i++) {
				if (i == p)
					setInputPanel (i, b);
				else
					setInputPanel (i, !b);
			}
		}

		public void setAllInputPanels (bool b)
		{
			for (int i = 0; i < inputPanels.Length; i++)
				setInputPanel (i, b);
		}

		public void setInputSubPanel (int sp, bool b)
		{
			switch (sp) {
			case (int) UI_INPUT_SUBPANEL.NAME:
				inputSubPanels [sp].GetComponent<InputField> ().interactable = b;
				break;
			case (int) UI_INPUT_SUBPANEL.SEX:
				foreach (Toggle t in inputSubPanels[(int)UI_INPUT_SUBPANEL.SEX].GetComponent<ToggleGroup>().GetComponentsInChildren<Toggle>())
					t.interactable = b;
				break;
			default:
				inputSubPanels [sp].SetActive (b);
				break;
			}
		}

		public void setExclusiveInputSubPanel (int sp, bool b)
		{
			for (int i = 0; i < inputSubPanels.Length; i++) {
				if (i == sp)
					setInputSubPanel (i, b);
				else
					setInputSubPanel (i, !b);
			}
		}

		public void setAllSubPanels (bool b)
		{
			for (int i = 0; i < inputSubPanels.Length; i++)
				setInputSubPanel (i, b);
		}

		public DB_Person getUIPerson ()
		{
			SEX s = SEX.EMPTY;
			foreach (Toggle t in inputSubPanels[(int)UI_INPUT_SUBPANEL.SEX].GetComponent<ToggleGroup>().GetComponentsInChildren<Toggle>()) {
				if (t.isOn) {
					s = (t.GetComponentInChildren<Text> ().text == "Hombre") ? SEX.MALE : SEX.FEMALE;
					break;
				}
			}

			return
				new DB_Person (
				-1,
				int.Parse (textCode.text),
				inputSubPanels [(int)UI_INPUT_SUBPANEL.NAME].GetComponent<InputField> ().text,
				int.Parse (textAge.text),
				s
			);
		}

		public void setUIPerson (DB_Person p)
		{
			///Very importante update
			APP_Manager.Instance.setCurrentPerson (p);

			textCode.text = p.codeStr;
			inputSubPanels [(int)UI_INPUT_SUBPANEL.NAME].GetComponent<InputField> ().text = p.name;
			textAge.text = p.ageStr;

			foreach (Toggle t in inputSubPanels[(int)UI_INPUT_SUBPANEL.SEX].GetComponent<ToggleGroup>().GetComponentsInChildren<Toggle>()) {
				if ((SEX.MALE == p.sex && t.GetComponentInChildren<Text> ().text == "Hombre")
				    ||
				    (SEX.FEMALE == p.sex && t.GetComponentInChildren<Text> ().text == "Mujer"))
					t.isOn = true;
				else
					t.isOn = false;
			}
		}

		public void resetAllInputPanels (int code, Color c, bool interactable)
		{
			textCode.text = code.ToString ("D3");
			textCode.color = c;
			inputSubPanels [(int)UI_INPUT_SUBPANEL.NAME].GetComponent<InputField> ().text = "";
			inputSubPanels [(int)UI_INPUT_SUBPANEL.NAME].GetComponent<InputField> ().textComponent.color = c;
			inputSubPanels [(int)UI_INPUT_SUBPANEL.NAME].GetComponent<InputField> ().interactable = interactable;
			textAge.text = "-1";
			textAge.color = c;
			foreach (Toggle t in inputSubPanels[(int)UI_INPUT_SUBPANEL.SEX].GetComponent<ToggleGroup>().GetComponentsInChildren<Toggle>()) {
				t.transform.Find ("Background").transform.Find ("Checkmark").GetComponent<Image> ().color = c;
				t.interactable = interactable;
				t.isOn = false;
			}
		}

		public void configureInputPanelsExistingCode (DB_Person p)
		{
			resetAllInputPanels (p.code, Color.black, false);

			setUIPerson (p);

			setAllInputPanels (true);
			setExclusiveInputSubPanel ((int)UI_INPUT_PANEL.CODE, true);
			setExclusiveInputButton ((int)UI_INPUT_BUTTON.MODIFY, true);
			setInputButton ((int)UI_INPUT_BUTTON.CONTINUE, true);
		}

		public void configureInputPanelsNewCode (int code)
		{
			resetAllInputPanels (code, Color.red, true);

			setExclusiveInputPanel ((int)UI_INPUT_PANEL.CODE, true);
			setExclusiveInputButton ((int)UI_INPUT_BUTTON.CREATE, true);
		}

		/// <summary>
		/// Sets the input button.
		/// </summary>
		/// <param name="bt">Bt.</param>
		/// <param name="b">If set to <c>true</c> b.</param>
		public void setGSButton (int bt, bool b)
		{
			gSessionButtons [bt].SetActive (b);
		}

		public void setExclusiveGSButton (int bt, bool b)
		{
			for (int i = 0; i < gSessionButtons.Length; i++)
				if (i == bt)
					setGSButton (i, b);
				else
					setGSButton (i, !b);
		}

		public void setAllGSButtons (bool b)
		{
			for (int i = 0; i < gSessionButtons.Length; i++)
				setGSButton (i, b);
		}

		public void setGSPanel (int gsp, bool b)
		{
			gSessionPanels [gsp].SetActive (b);
		}

		public void setExclusiveGSPanel (int gsp, bool b)
		{
			for (int i = 0; i < gSessionPanels.Length; i++)
				if (i == gsp)
					setGSPanel (i, b);
				else
					setGSPanel (i, !b);
		}

		public void setAllGSPanels (bool b)
		{
			for (int i = 0; i < gSessionPanels.Length; i++)
				setGSPanel (i, b);
		}

		/// <summary>
		/// Sets the GSG image.
		/// </summary>
		/// <param name="i">The index.</param>
		/// <param name="b">If set to <c>true</c> b.</param>
		public void setGSGImage (int i, bool b)
		{
			gSGImages [i].SetActive (b);
		}

		public void setExclusiveGSGImage (int im, bool b)
		{
			for (int i = 0; i < gSGImages.Length; i++)
				if (i == im)
					setGSGImage (i, b);
				else
					setGSGImage (i, !b);
		}

		public void setAllGSGImages (bool b)
		{
			for (int i = 0; i < gSGImages.Length; i++)
				setGSGImage (i, b);
		}

		public void setGSHCIInteractable (bool b)
		{
			foreach (Toggle t in gSessionSubPanels [(int)UI_GSESSION_SUBPANEL.GSHCI].GetComponent<ToggleGroup>().GetComponentsInChildren<Toggle>()) {
				t.interactable = b;
			}
		}

		public void setGSSubPanel (int gssp, bool b)
		{
			gSessionSubPanels [gssp].SetActive (b);
		}

		public void setExclusiveGSSubPanel (int gssp, bool b)
		{
			for (int i = 0; i < gSessionSubPanels.Length; i++)
				if (i == gssp)
					setGSSubPanel (i, b);
				else
					setGSSubPanel (i, !b);
		}

		public void setAllGSSubPanels (bool b)
		{
			for (int i = 0; i < gSessionSubPanels.Length; i++)
				setGSSubPanel (i, true);
		}

		public void initAllGSPanelsValues (int sn, Color c)
		{
			DB_Person p = APP_Manager.Instance.getCurrentPerson ();
			gSessionPanels [(int)UI_GSESSION_PANEL.CURRENT_PERSON].GetComponentInChildren<Text> ().text =
				p.codeStr + ", " + p.name + ", " + p.ageStr + ", " + ((p.sex == SEX.MALE) ? "Hombre" : "Mujer");
			gSessionPanels [(int)UI_GSESSION_PANEL.CURRENT_PERSON].GetComponentInChildren<Text> ().color = c;

			gSessionPanels [(int)UI_GSESSION_PANEL.SESSION_NUMBER].transform.Find ("TextGSNumber").GetComponent<Text> ().text = sn.ToString ();
			gSessionPanels [(int)UI_GSESSION_PANEL.SESSION_NUMBER].transform.Find ("TextGSNumber").GetComponent<Text> ().color = c;

			gSessionSubPanels [(int)UI_GSESSION_SUBPANEL.GSGIMAGES].SetActive (false);
		}

		public void initGSPanels (int sn, Color c)
		{
			initAllGSPanelsValues (sn, c);

			setExclusiveGSPanel ((int)UI_GSESSION_PANEL.CURRENT_PERSON, true);
			setGSPanel ((int)UI_GSESSION_PANEL.SESSION_NUMBER, true);
		}

		public void initGSButtons ()
		{
			setExclusiveGSButton ((int)UI_GSESSION_BUTTON.BACK, true);
		}

		public void configureGSPanelsExistingGSNumber (DB_GameSession gs)
		{

			///Panels
			initGSPanels (gs.session_number, Color.black);
			setAllGSPanels (true);

			///SubPanels
			setAllGSSubPanels (true);

			///Buttons
			setExclusiveGSButton ((int)UI_GSESSION_BUTTON.MODIFYGS, true);
			setGSButton ((int)UI_GSESSION_BUTTON.BACK, true);
			setGSButton ((int)UI_GSESSION_BUTTON.PLAYGS, true);

			///Toggles
			setGSHCIInteractable (false);

			setUIGSession (gs);
		}

		public void configureGSPanelsNewGSNumber (int sn)
		{
			///Panels
			initGSPanels (sn, Color.blue);

			///SubPanels
			setExclusiveGSPanel ((int)UI_GSESSION_PANEL.CURRENT_PERSON, true);
			setGSPanel ((int)UI_GSESSION_PANEL.SESSION_NUMBER, true);

			///Buttons
			setExclusiveGSButton ((int)UI_GSESSION_BUTTON.CREATEGS, true);
			setGSButton ((int)UI_GSESSION_BUTTON.BACK, true);

			///Toggles
			setGSHCIInteractable (true);
		}

		public DB_GameSession getUIGSession ()
		{
			HCI_TYPE hci = HCI_TYPE.NONE;
			foreach (Toggle t in gSessionSubPanels [(int)UI_GSESSION_SUBPANEL.GSHCI].GetComponent<ToggleGroup>().GetComponentsInChildren<Toggle>()) {
				if (t.isOn) {
					if (t.GetComponentInChildren<Text> ().text == "Nvidia")
						hci = HCI_TYPE.LM_AND_NVIDIA;
					else
						hci = HCI_TYPE.LM_AND_OCULUS;
				}
			}

			return new DB_GameSession (
				-1,
				APP_Manager.Instance.getCurrentPerson ().id,
				APP_Manager.Instance.getCurrentGame ().id,
				int.Parse (gSessionPanels [(int)UI_GSESSION_PANEL.SESSION_NUMBER].transform.Find ("TextGSNumber").GetComponent<Text> ().text),
				DateTime.Now,
				hci,
				GAME_SESION_STATE.TUTORIAL,
				APP_Manager.Instance.getCurrentGame ().limit_seconds,
				0
			);
		}

		public void setUIGSession (DB_GameSession gs)
		{
			///Very important update
			APP_Manager.Instance.setCurrentGameSession (gs);

			gSessionPanels [(int)UI_GSESSION_PANEL.SESSION_NUMBER].transform.Find ("TextGSNumber").GetComponent<Text> ().text = APP_Manager.Instance.getCurrentGameSession ().session_numberStr;
			gSessionSubPanels [(int)UI_GSESSION_SUBPANEL.GSGIMAGES].SetActive (true);

			setExclusiveGSGImage (gs.session_number % UI_Manager.Instance.gSGImages.Length, true);

			foreach (Toggle t in gSessionSubPanels [(int)UI_GSESSION_SUBPANEL.GSHCI].GetComponent<ToggleGroup>().GetComponentsInChildren<Toggle>()) {
				if ((HCI_TYPE.LM_AND_NVIDIA == gs.hci_type && t.GetComponentInChildren<Text> ().text == "Nvidia")
				    ||
				    (HCI_TYPE.LM_AND_OCULUS == gs.hci_type && t.GetComponentInChildren<Text> ().text == "Oculus"))
					t.isOn = true;
				else
					t.isOn = false;
			}
		}

		public void showGSFBMessage (string m)
		{
			textGSFeedBack.GetComponent<Text> ().text = m;
			hrtm.Timers [0].InitCounting ();
			ui_fb_state = UI_FB_STATE.SHOWING_FB_MESSAGE;
		}

		void GSFeeBackText_Fsm ()
		{
			switch (ui_fb_state) {
			case UI_FB_STATE.IDLE:
				break;
			case UI_FB_STATE.SHOWING_FB_MESSAGE:
				hrtm.Timers [0].EndCounting ();
				if (hrtm.Timers [0].GetLastPeriodSecs () > 1.0f) {
					hrtm.Timers [0].InitCounting ();    //reset
					textGSFeedBack.text = "Sesión de Juego";
					ui_fb_state = UI_FB_STATE.IDLE;
				}
				break;
			}
		}

		void Update ()
		{
//			Debug.Log (ui_state.ToString ());
			if (UI_STATE.CREATING_PERSON == ui_state || UI_STATE.MODIFYING_PERSON == ui_state)
				inputFeedBackText_Fsm ();
			else
				GSFeeBackText_Fsm ();
		}
	}

}
