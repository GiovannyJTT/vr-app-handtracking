using UnityEngine;
using System.Collections;
using APPTfm;
using UnityEngine.SceneManagement;
using DBTfm;
using DBTfm.Types;
using UnityEngine.UI;
using APPVoices;
using APPVoices.Types;

public class GRT_Tutorial_Manager : MonoBehaviour
{
	private HRTManager hrtm;
	public GameObject instOculus;
	public GameObject instNvidia;

	private string tutMessageOculus;
	private string tutMessageNvidia;

	private string playMessageOculus;
	private string playMessageNvidia;

	private double messageSeconds;

	private DB_GameSession gs;

	void Start ()
	{
		//Update app (because we can execute in debug mode, or real mode in such case we catch the values from the ui)
		APP_Manager.Instance.updateCurrentGameInApp ((int)GAME_ORDINAL.FIRST);
		APP_Manager.Instance.updateCurrentGSessionInAPP_and_InDB (GAME_SESION_STATE.TUTORIAL, (int)GAME_ORDINAL.FIRST);
		
		//Create Timers
		//[0] Seconds to show the scene tutorial. Then will jump to the game.
		hrtm = new HRTManager ();
		hrtm.ResetSamplingFrequency ();
		hrtm.CreateTimers (1);
		hrtm.Timers [0].InitCounting ();

		tutMessageOculus = "Ejemplo: girar suavemente la esfera y\ncolocar el objeto mirando hacia ti.";
		tutMessageNvidia = "Ejemplo: girar suavemente la esfera y colocar el objeto mirando hacia ti.";

		playMessageOculus = "¡Es tu turno!";
		playMessageNvidia = "¡Es tu turno!";

		messageSeconds = 10.0d;

		gs = APP_Manager.Instance.getCurrentGameSession ();

		showTutMessage (gs);
	}

	private void showTutMessage (DB_GameSession gs)
	{
		if (gs != null) {
			if (gs.hci_type == HCI_TYPE.LM_AND_NVIDIA) {
				instNvidia.GetComponent<Text> ().text = tutMessageNvidia;
			} else {
				instOculus.GetComponent<TextMesh> ().text = tutMessageOculus;
			}	
		} else {
			instNvidia.GetComponent<Text> ().text = tutMessageNvidia;
		}

		//audio
		APP_Voices.Instance.playMuseumVoice ((int)APP_VOICES_MUSEUM.MUSEUMEXAMPLE);
	}

	private void showPlayMessage (DB_GameSession gs)
	{
		if (gs != null) {
			if (gs.hci_type == HCI_TYPE.LM_AND_NVIDIA) {
				instNvidia.GetComponent<Text> ().text = playMessageNvidia;
			} else {
				instOculus.GetComponent<TextMesh> ().text = playMessageOculus;
			}	
		} else {
			instNvidia.GetComponent<Text> ().text = playMessageNvidia;
		}

		//audio
		APP_Voices.Instance.playMuseumVoice ((int)APP_VOICES_MUSEUM.ITSYOURTURN);
	}

	void Update ()
	{
		hrtm.Timers [0].EndCounting ();
		if (hrtm.Timers [0].GetLastPeriodSecs () > messageSeconds && hrtm.Timers [0].GetLastPeriodSecs () < messageSeconds + 0.1d) {
			showPlayMessage (gs);
		}

		//tutorial exit
		if (hrtm.Timers [0].GetLastPeriodSecs () > messageSeconds + 2.0f) {
			hrtm.DestroyTimers ();
			SceneManager.LoadScene ("tfm_game_museum");
		}
	}
}
