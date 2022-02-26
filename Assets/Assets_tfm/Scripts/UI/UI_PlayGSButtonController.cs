using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UITfm;
using UITfm.Types;
using UnityEngine.SceneManagement;
using APPTfm;
using DBTfm;
using DBTfm.Types;
using System;

public class UI_PlayGSButtonController : MonoBehaviour {
	private Button b;
	private string[] sceneNames;
	void Start () {
		b = this.GetComponent<Button>();

		b.onClick.AddListener(
			() => updateUI()
			);

		sceneNames = new string[3];
		sceneNames[0] = "tfm_game_museum_tutorial";
		sceneNames[1] = "tfm_game_football_tutorial";
		sceneNames[2] = "tfm_game_tower_tutorial";
	}

	private void updateGSessionInDB(){
		DB_GameSession gs = APP_Manager.Instance.getCurrentGameSession();
		if(gs != null){
			gs.setCurrentDate();
			gs.state = GAME_SESION_STATE.TUTORIAL;
			APP_Manager.Instance.setCurrentGameSession(gs);
			DB_Manager.Instance.updateGSession(gs);
		} else {
			Debug.Log("There is no getCurrentGameSession");
		}
	}

	private void updateUI () {
		switch (UI_Manager.ui_state) {
		case UI_STATE.EXISTING_GSNUMBER:
			UI_Manager.ui_state = UI_STATE.DISABLED;
			updateGSessionInDB();
			SceneManager.LoadScene(sceneNames[(APP_Manager.Instance.getCurrentGameSession().session_number) % sceneNames.Length]);
			break;
		}
	}
}
