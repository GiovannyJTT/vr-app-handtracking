using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using APPTfm;
using DBTfm;
using DBTfm.Types;
using System;
using UnityEngine.SceneManagement;
using APPVoices;
using APPVoices.Types;

public class GRT_Manager : MonoBehaviour
{
	/// <summary>
	/// GameReactionTime state.
	/// </summary>
	private enum GRT_STATE
	{
		INIT = 0,
		LOADING_NEXT = 1,
		UNLOADING_CURRENT = 2,
		PREPARING_BUTTON = 3,
		RESETTING_BUTTON = 4,
		PLAYING = 5,
		COMPLETED = 6
	}

	private GRT_STATE grt_state;

	private List<GRT_Item> grt_items;
	private int current;
	private int next;

	/// <summary>
	/// The lists of Game Objects in the GameReactionTime.
	/// </summary>
	public List<string> names;
	public List<GameObject> targets;
	public List<GameObject> targetsLighting;
	public List<GameObject> guidesL;
	public List<GameObject> guidesLLighting;
	public List<GameObject> guidesR;
	public List<GameObject> guidesRLighting;

	/// <summary>
	/// The button GameObject that contain canvas for Nvidia version and quads for Oculus version. Also contains bNext structure.
	/// </summary>
	public GameObject toggleBNext;

	/// <summary>
	/// The structure that controlls the state (true, false), changes the colors, and make the spring effect.
	/// </summary>
	private MyButtonNext bNext;

	/// <summary>
	/// The text3d to show instructions in the GameReactionTime.
	/// </summary>
	public GameObject text3dInstr;
	private TextMesh tm;
	public Text textCanvas;

	//feedback audio when item is aligned
	public AudioClip aClipItemOK;
	private AudioSource aSource;

	/// <summary>
	/// The High resolution timer manager to control all timers.
	/// </summary>
	private HRTManager hrtm;

	private int[] ids_items;
	private int[] ids_items_voices;

	void Start ()
	{
		//Update app (because we can execute in debug mode, or real mode in such case we catch the values from the ui)
		APP_Manager.Instance.updateCurrentGameInApp ((int)GAME_ORDINAL.FIRST);
		APP_Manager.Instance.updateCurrentGSessionInAPP_and_InDB (GAME_SESION_STATE.PLAYING, (int)GAME_ORDINAL.FIRST);

		//Load needed initial elements
		tm = text3dInstr.GetComponent<TextMesh> ();
		bNext = toggleBNext.GetComponentInChildren<MyButtonNext> ();

		//feebback audio when item is aligned
		aSource = this.GetComponent<AudioSource> ();

		//Create grt_items
		grt_items = new List<GRT_Item> ();
		List<GameObject> lg = new List <GameObject> ();
		List<GameObject> lgl = new List <GameObject> ();

		ids_items = new int[] {
			(int)IDS_ITEMS.ID_HERMES,
			(int)IDS_ITEMS.ID_CRANIUM,
			(int)IDS_ITEMS.ID_CRATERA,
			(int)IDS_ITEMS.ID_OILLAMP
		};

		for (int i = 0; i < names.Count; i++) {
			lg.Add (guidesL [i]);
			lg.Add (guidesR [i]);
			lgl.Add (guidesLLighting [i]);
			lgl.Add (guidesRLighting [i]);
			grt_items.Add (new GRT_Item (ids_items [i], names [i], targets [i], targetsLighting [i], lg, lgl, text3dInstr, textCanvas, aSource, aClipItemOK));
			lg.Clear ();
			lgl.Clear ();
		}

		//Create Timers
		//[0] time in seconds to animate the transitions (rotate the models some seconds)
		//[1] time in seconds to wait after the item is aligned and appear the button 'next'.
		//[2] time in seconds wait the reset process of the button.
		//[3] time in seconds before jump to the next game.
		hrtm = new HRTManager ();
		hrtm.ResetSamplingFrequency ();
		hrtm.CreateTimers (4);

		//voices
		ids_items_voices = new int[] {
			(int)APP_VOICES_MUSEUM.HERMES,
			(int)APP_VOICES_MUSEUM.CRANIUM,
			(int)APP_VOICES_MUSEUM.CRATERA,
			(int)APP_VOICES_MUSEUM.OILLAMP
		};

		//Initial GameRT State (GRT_ITEM_STATE is controlled by grt_state)
		current = 0;
		next = 0;
		grt_state = GRT_STATE.INIT;
	}


	/// <summary>
	/// Finite State Machine for control the transitions between the diferent states of the GameReactionTime.
	/// </summary>
	public void grt_Fsm ()
	{
		switch (grt_state) {
		case GRT_STATE.INIT:
			//Debug.Log("Init");

			//Disable Button Next
			toggleBNext.SetActive (false);

			//Disable all items and their lightings
			for (int i = 0; i < grt_items.Count; i++) {
				grt_items [i].setTargetKinematic (true);
				grt_items [i].setTargetActive (false);

				grt_items [i].setGuidesSpherePoints (false);
				grt_items [i].setGuidesActive (false);
			}

			current = 0;
			next = 0;

			grt_state = GRT_STATE.LOADING_NEXT;    //Jump
			break;

		case GRT_STATE.LOADING_NEXT:
			//Debug.Log("Loading_next");

			//Enable next item (and its lighting)
			if (next < grt_items.Count) {
				grt_items [next].setTargetKinematic (false);
				grt_items [next].setTargetActive (true);
				grt_items [next].setTargetReached (false);    //Reset

				grt_items [next].setGuidesSpherePoints (false);
				grt_items [next].setGuidesActive (true);

				grt_items [next].item_state = GRT_Item.GRT_ITEM_STATE.TURNING;   //Initial GRT_ItemState state

				//Very Important Update
				current = next;
				next++;

				//voices
				APP_Voices.Instance.first_LOOKATTHEFIGURES = true;
				APP_Voices.Instance.playMuseumVoice (ids_items_voices [current]);

				//DB: after each item loading we need update the current item
				APP_Manager.Instance.updateCurrentItemInApp (ids_items [current]);
			
				grt_state = GRT_STATE.PLAYING;    //Jump
			} else {
				tm.text = "Tiempo Total: " + ((int)grt_items [current].getItemAccumulatedSecs ()).ToString () + " s";
				textCanvas.text = "Tiempo Total: " + ((int)grt_items [current].getItemAccumulatedSecs ()).ToString () + " s";

				//DB: To store TotalReactionTime in GameSession in database use getItemAccumulatedSecs here
				APP_Manager.Instance.gameCompletedInDB (grt_items [current].getItemAccumulatedSecs (), (current + 1));		

				hrtm.Timers [3].InitCounting ();
				grt_state = GRT_STATE.COMPLETED;   //Jump
			}
			break;

		case GRT_STATE.UNLOADING_CURRENT:
			//Debug.Log("Unloading_current");

			//Disable current item (and its lighting) after some seconds of animation (or pulse button) and jump
			if (current >= 0) {
				hrtm.Timers [0].EndCounting ();
				if (hrtm.Timers [0].GetLastPeriodSecs () > 20.0f) {
					//Mantain its isKinemactic=true to avoid collisions in the transition

					grt_items [current].setTargetActive (false);
					grt_items [current].setGuidesActive (false);

					toggleBNext.SetActive (false);

					grt_state = GRT_STATE.LOADING_NEXT;    //Jump
				} else {
					if (bNext.ToggleState) {
						//Mantain its isKinemactic=true to avoid collisions in the transition

						grt_items [current].setTargetActive (false);
						grt_items [current].setGuidesActive (false);
					
						hrtm.Timers [2].InitCounting ();    //Reset timer of wait for reset button process

						grt_state = GRT_STATE.RESETTING_BUTTON;    //Jump
					}
				}
			}
			break;
		
		case GRT_STATE.PREPARING_BUTTON:
			hrtm.Timers [1].EndCounting ();
			if (hrtm.Timers [1].GetLastPeriodSecs () > 10f) {
				tm.text = "¡Pulsa el botón\npara continuar!";
				textCanvas.text = "¡Pulsa el botón\npara continuar!";

				toggleBNext.SetActive (true);
				//voices
				APP_Voices.Instance.playMuseumVoice ((int)APP_VOICES_MUSEUM.PRESSTHEBUTTONTOCONTINUE);
				grt_state = GRT_STATE.UNLOADING_CURRENT;
			}
			break;

		case GRT_STATE.RESETTING_BUTTON:
			//Debug.Log("Resetting_button");

			hrtm.Timers [2].EndCounting ();
			if (hrtm.Timers [2].GetLastPeriodSecs () > bNext.secondsReset + 0.35f) {
				bNext.ToggleState = false;    //reset stat to avoid human factors
				toggleBNext.SetActive (false);
				grt_state = GRT_STATE.LOADING_NEXT;
			}
			break;

		case GRT_STATE.PLAYING:
			//Debug.Log("Playing");

			if (grt_items [current].item_state == GRT_Item.GRT_ITEM_STATE.LINED) {
				//Active isKinematic=true and RotateKinematic.cs rotates the target and the guides
				grt_items [current].setTargetKinematic (true);
				grt_items [current].setGuidesSpherePoints (true);

				hrtm.Timers [0].InitCounting ();    //Reset timer of rotation animation
				hrtm.Timers [1].InitCounting ();    //seconds to wait before appear the button
				grt_state = GRT_STATE.PREPARING_BUTTON;    //Jump
			}
			break;

		case GRT_STATE.COMPLETED:
			//Debug.Log("Completed");
			hrtm.Timers [3].EndCounting ();
			if (hrtm.Timers [3].GetLastPeriodSecs () > 2.0f) {
				APP_Manager.Instance.prepareNextGSession ();
				hrtm.DestroyTimers ();
                //DUMP ATTEMPTS TO BD
                DB_Manager.Instance.dumpItemAttempts_ToDB();
				SceneManager.LoadScene ("tfm_game_football_tutorial");
			}
			break;
		}
	}


	void Update ()
	{
		grt_Fsm ();   //it need be executed first
		grt_items [current].grt_Item_Fsm ();
	}
}
