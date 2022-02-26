using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using APPTfm;
using DBTfm;
using DBTfm.Types;
using UnityEngine.SceneManagement;
using APPVoices;
using APPVoices.Types;

public class GTB_Manager : MonoBehaviour
{
	private enum GTB_STATE
	{
		INIT = 0,
		LOADING_NEXT_GRASPABLESSET = 1,
		PLAYING = 2,
		UNLOADING_CURRENT_GRASPABLESSET = 3,
		RESETTING_BUTTON = 4,
		COMPLETED = 5
	}

	private GTB_STATE gtb_state;

	public GameObject instrText3D;
	public Text instrTextCanvas;
	public GameObject tbNext;
	public AudioClip aclip;
	public GameObject[] gSets_GO;
	public GameObject[] heightDetector;
	public int[] targetHeight;
	public GameObject towerBaseDetector;
	public GameObject[] towerAxisDetectors;
	public GameObject[] towerAxisDetectorsMeshes;
	public GameObject towerDetectorsRoot;
	public GameObject LineRenderer_GO;

	public GameObject[] handControllers;
	public GameObject[] bubbles;

	private MyButtonNext bNext;
	private static TextMesh tm;
	private static Text tc;
	private AudioSource asource;
	private static HRTManager hrtm;

	private GTB_GraspablesSet[] gSets;
	private static int currentGSet;
	private static int nextGSet;



	void Start ()
	{
		//Update app (because we can execute in debug mode, or real mode in such case we catch the values from the ui)
		APP_Manager.Instance.updateCurrentGameInApp ((int)GAME_ORDINAL.THIRD);
		APP_Manager.Instance.updateCurrentGSessionInAPP_and_InDB (GAME_SESION_STATE.PLAYING, (int)GAME_ORDINAL.THIRD);

		tm = instrText3D.GetComponent<TextMesh> ();
		tc = instrTextCanvas;
		bNext = tbNext.GetComponentInChildren<MyButtonNext> ();
		tbNext.SetActive (false);
		asource = this.GetComponent<AudioSource> ();

		//[0] to force the jumping to the next Level;
		//[1] to wait the reset process of the button.
		//[2] seconds befor jump to next scene (tfm_ui)
		hrtm = new HRTManager ();
		hrtm.ResetSamplingFrequency ();
		hrtm.CreateTimers (3);


		//initial values
		currentGSet = 0;
		nextGSet = 0;
		fillGraspablesSets ();
		gtb_state = GTB_STATE.INIT;
	}

	private void setGraspableForOculus (GameObject go)
	{
		Rigidbody rb = go.GetComponent<Rigidbody> ();
		if (rb != null) {
			rb.mass = 2f;
			rb.drag = 17f;
			rb.angularDrag = 17f;
		}
	}

	private void setGraspableForNvidia (GameObject go)
	{
		Rigidbody rb = go.GetComponent<Rigidbody> ();
		if (rb != null) {
			rb.mass = 7f;
			rb.drag = 20f;
			rb.angularDrag = 20f;
		}
	}

	private void fillGraspablesSets ()
	{
		gSets = new GTB_GraspablesSet[gSets_GO.Length];

		int[] ids_GraspablesSet = new int[2] {
			(int)IDS_ITEMS.ID_GRASPABLES_SET_NEAR,
			(int)IDS_ITEMS.ID_GRASPABLES_SET_FAR,
		};

		for (int i = 0; i < gSets_GO.Length; i++) {
			int nChilds = gSets_GO [i].transform.childCount;
			GameObject[] childs = new GameObject[nChilds];
			for (int j = 0; j < nChilds; j++) {
				childs [j] = gSets_GO [i].transform.GetChild (j).gameObject;
				if (APP_Manager.Instance.getCurrentGameSession ().hci_type == HCI_TYPE.LM_AND_OCULUS) {
					setGraspableForOculus (childs [j]);
				} else {
					setGraspableForNvidia (childs [j]);
				}
			}

			gSets [i] = new GTB_GraspablesSet (
				(i == 0) ? ids_GraspablesSet [0] : ids_GraspablesSet [1],
				childs,
				heightDetector [i],
				targetHeight [i],
				towerBaseDetector,
				towerAxisDetectors,
				towerAxisDetectorsMeshes,
				instrText3D,
				instrTextCanvas,
				asource,
				aclip,
				handControllers,
				bubbles [i]);
		}
	}

	private void gtb_Fsm ()
	{
		switch (gtb_state) {
		case GTB_STATE.INIT:
			currentGSet = 0;
			nextGSet = 0;
			gtb_state = GTB_STATE.LOADING_NEXT_GRASPABLESSET;
			break;

		case GTB_STATE.LOADING_NEXT_GRASPABLESSET:
			if (nextGSet < gSets.Length) {
				//near, 4
				if (nextGSet == 0) {
					towerDetectorsRoot.transform.localPosition = new Vector3 (-2.5f, 2.5f, -0.775f);
					LineRenderer_GO.GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (0f, 0.4f, -0.075f));
					LineRenderer_GO.GetComponent<LineRenderer> ().SetPosition (1, new Vector3 (0f, 0.6f, -0.075f));
				}
				//far, 6
				if (nextGSet == 1) {
					towerDetectorsRoot.transform.localPosition = new Vector3 (-2.5f, 2.5f, -0.3f);
					LineRenderer_GO.GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (0f, 0.4f, -0.03f));
					LineRenderer_GO.GetComponent<LineRenderer> ().SetPosition (1, new Vector3 (0f, 0.7f, -0.03f));
				}


				gSets [nextGSet].initGraspables ();

				//Very Important Update
				currentGSet = nextGSet;
				nextGSet++;

				gtb_state = GTB_STATE.PLAYING;

			} else {
				tm.text = "Total Apilados " + GTB_GraspablesSet.nTotalStacked.ToString ();
				tm.text += "\nTotal Tiempo " + ((int)gSets [currentGSet].getGSetAccumulatedSecs ()).ToString () + " s";
				tm.text += "\n\n¡Gracias por participar!";

				tc.text = "Total Apilados " + GTB_GraspablesSet.nTotalStacked.ToString ();
				tc.text += "\nTotal Tiempo " + ((int)gSets [currentGSet].getGSetAccumulatedSecs ()).ToString () + " s";
				tc.text += "\n\n¡Gracias por participar!";

				APP_Manager.Instance.gameCompletedInDB (gSets [currentGSet].getGSetAccumulatedSecs (), GTB_GraspablesSet.nTotalStacked);
				hrtm.Timers [2].InitCounting ();
				gtb_state = GTB_STATE.COMPLETED;
			}
			break;

		case GTB_STATE.PLAYING:
			if (gSets [currentGSet].getGSetState () == GTB_GraspablesSet.GTB_GRASPABLESET_STATE.END) {
				tm.text = "Apilados " + GTB_GraspablesSet.tower.Count.ToString () + "\n¡Pulsa el botón!";
				tc.text = "Apilados " + GTB_GraspablesSet.tower.Count.ToString () + "\n¡Pulsa el botón!";

				tbNext.SetActive (true);
				heightDetector [currentGSet].GetComponent<MeshRenderer> ().enabled = false;

				//voice
				APP_Voices.Instance.playTowerVoice ((int)APP_VOICES_TOWER.PRESSTHEBUTTON);

				hrtm.Timers [0].InitCounting ();
				gtb_state = GTB_STATE.UNLOADING_CURRENT_GRASPABLESSET;
			}
			break;

		case GTB_STATE.UNLOADING_CURRENT_GRASPABLESSET:
			hrtm.Timers [0].EndCounting ();
			if (hrtm.Timers [0].GetLastPeriodSecs () < 8.0f) {
				if (bNext.ToggleState) {
					gSets [currentGSet].destroyAllGraspables ();
					GTB_GraspablesSet.setAllTowerAxisDetectorMeshes (false);
					hrtm.Timers [1].InitCounting ();
					gtb_state = GTB_STATE.RESETTING_BUTTON;
				}
			} else {
				//force jump
				gSets [currentGSet].destroyAllGraspables ();
				bNext.ToggleState = false;    //reset stat to avoid human factors
				tbNext.SetActive (false);
				gtb_state = GTB_STATE.LOADING_NEXT_GRASPABLESSET;
			}
			break;
		case GTB_STATE.RESETTING_BUTTON:
			hrtm.Timers [1].EndCounting ();
			if (hrtm.Timers [1].GetLastPeriodSecs () > bNext.secondsReset + 0.35f) {
				bNext.ToggleState = false;    //reset stat to avoid human factors
				tbNext.SetActive (false);
				gtb_state = GTB_STATE.LOADING_NEXT_GRASPABLESSET;
			}
			break;

		case GTB_STATE.COMPLETED:
			//return to the ui scene
			hrtm.Timers [2].EndCounting ();
			if (hrtm.Timers [2].GetLastPeriodSecs () > 3.0f) {
				hrtm.DestroyTimers ();
                //DUMP ATTEMPTS TO BD
                DB_Manager.Instance.dumpItemAttempts_ToDB();
				SceneManager.LoadScene ("tfm_ui");
			}
			break;
		}
	}

	void Update ()
	{
		gtb_Fsm ();
		gSets [currentGSet].gtb_GSetFsm ();
	}
}
