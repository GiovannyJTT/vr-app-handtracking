using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DBTfm;
using DBTfm.Types;
using APPTfm;
using UnityEngine.SceneManagement;
using APPVoices;
using APPVoices.Types;

public class GFB_Tutorial_Manager : MonoBehaviour
{
	private enum GFB_STATE
	{
		INIT = 0,
		LOADING_NEXT_SET = 1,
		PLAYING = 2,
		UNLOADING_CURRENT_SET = 3,
		RESETTING_BUTTON = 4,
		COMPLETED = 5
	}

	private static GFB_STATE gfb_state;

	public GameObject instrText3D;
	public Text instrTextCanvas;
	public AudioClip aclip;
	public GameObject tbNext;
	public GameObject goalDetector;

	private MeshRenderer goalDetectorMeshRenderer;

	public GameObject[] sets_GO;
	private GFB_ItemsSet[] sets;

	private static int currentSet;
	private static int nextSet;
	private AudioSource asource;
	private MyButtonNext bNext;
	private static HRTManager hrtm;
	private static int nGoalsTotal;
	private static TextMesh tm;
	private static Text tc;


	/// <summary>
	/// Tutorial objects
	/// </summary>
	public GameObject messageText3D;
	public Text messageTextCanvas;
	private string tutMessageOculus;
	private string tutMessageNvidia;
	private string playMessageOculus;
	private string playMessageNvidia;
	private double messageSeconds;
	private DB_GameSession gs;

	void Start ()
	{
		//Update app (because we can execute in debug mode, or real mode in such case we catch the values from the ui)
		APP_Manager.Instance.updateCurrentGameInApp ((int)GAME_ORDINAL.SECOND);
		APP_Manager.Instance.updateCurrentGSessionInAPP_and_InDB (GAME_SESION_STATE.TUTORIAL, (int)GAME_ORDINAL.SECOND);

		tm = instrText3D.GetComponent<TextMesh> ();
		tc = instrTextCanvas;

		bNext = tbNext.GetComponentInChildren<MyButtonNext> ();
		tbNext.SetActive (false);

		goalDetectorMeshRenderer = goalDetector.GetComponent<MeshRenderer> ();
		asource = this.GetComponent<AudioSource> ();

		//[0] to force the jump to the next set;
		//[1] For wait the reset process of the button.
		//[2] For tutorial
		hrtm = new HRTManager ();
		hrtm.ResetSamplingFrequency ();
		hrtm.CreateTimers (3);

		//initial game values
		currentSet = 0;
		nextSet = 0;
		nGoalsTotal = 0;
		fillSets ();
		gfb_state = GFB_STATE.INIT;

		///Tutorial init values
		tutMessageOculus = "Ejemplo: golpear los objetos y\nanotar el mayor número de goles\nantes de que termine el tiempo.";
		tutMessageNvidia = "Ejemplo: golpear los objetos y anotar el mayor número de goles\nantes de que termine el tiempo.";
		playMessageOculus = "¡Es tu turno!";
		playMessageNvidia = "¡Es tu turno!";
		messageSeconds = 10.0d;
		hrtm.Timers [2].InitCounting ();
		gs = APP_Manager.Instance.getCurrentGameSession ();
		showTutMessage (gs);
	}

	private void showTutMessage (DB_GameSession gs)
	{
		if (gs != null) {
			if (gs.hci_type == HCI_TYPE.LM_AND_NVIDIA) {
				messageTextCanvas.GetComponent<Text> ().text = tutMessageNvidia;
			} else {
				messageText3D.GetComponent<TextMesh> ().text = tutMessageOculus;
			}	
		} else {
			messageTextCanvas.GetComponent<Text> ().text = tutMessageNvidia;
		}

		//voice
		APP_Voices.Instance.playFootballVoice ((int)APP_VOICES_FOOTBALL.FOOTBALLEXAMPLE);
	}

	private void showPlayMessage (DB_GameSession gs)
	{
		if (gs != null) {
			if (gs.hci_type == HCI_TYPE.LM_AND_NVIDIA) {
				messageTextCanvas.GetComponent<Text> ().text = playMessageNvidia;
			} else {
				messageText3D.GetComponent<TextMesh> ().text = playMessageOculus;
			}	
		} else {
			messageTextCanvas.GetComponent<Text> ().text = playMessageNvidia;
		}

		//voice
		APP_Voices.Instance.playFootballVoice ((int)APP_VOICES_FOOTBALL.ITSYOURNTURN);
	}

	private void fillSets ()
	{
		sets = new GFB_ItemsSet[sets_GO.Length];

		int[] ids_itemsSet = new int[12] {
			(int)IDS_ITEMS.ID_BALLS_SET_L,
			(int)IDS_ITEMS.ID_BALLS_SET_R,
			(int)IDS_ITEMS.ID_CAPSULES_SET_L,
			(int)IDS_ITEMS.ID_CAPSULES_SET_R,
			(int)IDS_ITEMS.ID_ICOSAHEDRONS_SET_L,
			(int)IDS_ITEMS.ID_ICOSAHEDRONS_SET_R,
			(int)IDS_ITEMS.ID_PRISMS_SET_L,
			(int)IDS_ITEMS.ID_PRISMS_SET_R,
			(int)IDS_ITEMS.ID_CUBES_SET_L,
			(int)IDS_ITEMS.ID_CUBES_SET_R,
			(int)IDS_ITEMS.ID_PYRAMIDS_SET_L,
			(int)IDS_ITEMS.ID_PYRAMIDS_SET_R
		};

		for (int i = 0, k = 0; i < sets_GO.Length; i++, k += 2) {
			int nChilds = sets_GO [i].transform.childCount;
			GameObject[] childs = new GameObject[nChilds];
			for (int j = 0; j < nChilds; j++) {
				childs [j] = sets_GO [i].transform.GetChild (j).gameObject;
			}
			sets [i] = new GFB_ItemsSet (ids_itemsSet [k], ids_itemsSet [k + 1], childs, instrText3D, instrTextCanvas, asource, aclip);
		}
	}

	private void gfb_Fsm ()
	{
		switch (gfb_state) {
		case GFB_STATE.INIT:
			currentSet = 0;
			nextSet = 0;
			gfb_state = GFB_STATE.LOADING_NEXT_SET;
			break;

		case GFB_STATE.LOADING_NEXT_SET:
			if (nextSet < sets.Length) {
				sets [nextSet].initItems ();

				//Very Important Update
				currentSet = nextSet;
				nextSet++;

				gfb_state = GFB_STATE.PLAYING;

			} else {
				tm.text = "Total Goles " + nGoalsTotal.ToString ();
				tm.text += "\nTotal Tiempo " + ((int)sets [currentSet].getItemAccumulatedSecs ()).ToString () + " s";

				tc.text = "Total Goles " + nGoalsTotal.ToString ();
				tc.text += "\nTotal Tiempo " + ((int)sets [currentSet].getItemAccumulatedSecs ()).ToString () + " s";

				APP_Manager.Instance.gameCompletedInDB (sets [currentSet].getItemAccumulatedSecs (), nGoalsTotal);

				gfb_state = GFB_STATE.COMPLETED;
			}
			break;

		case GFB_STATE.PLAYING:
			if (sets [currentSet].getSetState () == GFB_ItemsSet.GFB_SET_STATE.END) {
				tm.text = "Goles " + GFB_ItemsSet.nGoalsObtained.ToString () + "\n\n¡Pulsa el botón!";
				tc.text = "Goles " + GFB_ItemsSet.nGoalsObtained.ToString () + "\n\n¡Pulsa el botón!";

				nGoalsTotal += GFB_ItemsSet.nGoalsObtained;

				tbNext.SetActive (true);
				goalDetectorMeshRenderer.enabled = false;

				hrtm.Timers [0].InitCounting ();
				gfb_state = GFB_STATE.UNLOADING_CURRENT_SET;
			}
			break;

		case GFB_STATE.UNLOADING_CURRENT_SET:
			hrtm.Timers [0].EndCounting ();
			if (hrtm.Timers [0].GetLastPeriodSecs () < 8.0f) {
				if (bNext.ToggleState) {
					sets [currentSet].destroyAllItems ();
					hrtm.Timers [1].InitCounting ();
					gfb_state = GFB_STATE.RESETTING_BUTTON;
				}
			} else {
				//force jump
				sets [currentSet].destroyAllItems ();
				bNext.ToggleState = false;    //reset stat to avoid human factors
				tbNext.SetActive (false);
				gfb_state = GFB_STATE.LOADING_NEXT_SET;
			}
			break;
		case GFB_STATE.RESETTING_BUTTON:
			hrtm.Timers [1].EndCounting ();
			if (hrtm.Timers [1].GetLastPeriodSecs () > bNext.secondsReset + 0.35f) {
				bNext.ToggleState = false;    //reset stat to avoid human factors
				tbNext.SetActive (false);
				gfb_state = GFB_STATE.LOADING_NEXT_SET;
			}
			break;

		case GFB_STATE.COMPLETED:
			break;
		}
	}

	void Update ()
	{
		gfb_Fsm ();
		sets [currentSet].gfb_itemFsm ();

		//tutorial exit
		hrtm.Timers [2].EndCounting ();
		if (hrtm.Timers [2].GetLastPeriodSecs () > messageSeconds && hrtm.Timers [2].GetLastPeriodSecs () < messageSeconds + 0.1d) {
			showPlayMessage (gs);
		}
		if (hrtm.Timers [2].GetLastPeriodSecs () > messageSeconds + 2.0f) {
			hrtm.DestroyTimers ();
			SceneManager.LoadScene ("tfm_game_football");
		}
	}
}
