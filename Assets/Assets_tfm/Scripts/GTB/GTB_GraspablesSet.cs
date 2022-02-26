using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DBTfm;
using DBTfm.Types;
using APPTfm;
using System.Collections.Generic;
using APPVoices;
using APPVoices.Types;

public class GTB_GraspablesSet
{
	public enum GTB_GRASPABLESET_STATE
	{
		LOADING_NEXT_GRASPABLE = 0,
		PLAYING = 1,
		ALL_STACKED = 2,
		DEADLINE = 3,
		TOWER_EVALUATION = 4,
		TOWER_RESULTS = 5,
		///ALL TIME CONSUMED AND NOT ALL ITEMS CONSUMED
		END = 6
	}

	public static GTB_GRASPABLESET_STATE gSet_state;

	//	public enum GTB_TOWER_STATE
	//	{
	//		EMPTY = 0,
	//		STACKING = 1,
	//		COMPLETED = 2
	//	}
	//	public static GTB_TOWER_STATE tower_state;

	//Graspable Items
	private GameObject[] graspables;
	private int currentGraspable;
	private int nextGraspable;

	//Tower
	public static GameObject towerBaseDetector;
	public static GameObject[] towerAxisDetectors;
	public static GameObject[] towerAxisDetectorsMeshes;
	private GameObject heightDetector;
	private int heightRequired;
	public static int targetHeight;
	public static Stack<GameObject> tower;

	//Timers
	private static HRTManager hrtm;
	private static double remainingTime;

	//Feedback Text
	private static GameObject instrText3D;
	private static Text instrTextCanvas;
	private static TextMesh tm;
	private static Text tc;
	private static string instString;
	private static string towerString;

	//Feedback Audio
	private static AudioSource asource;
	private static AudioClip aclip;
	private GTB_BombTimerSound bombTimerSound;

	//database
	private int id_set_nearFar;
	private static int id_CurrentGraspablesSetOwner;
	private static ITEM_POSITION currentGraspablePosition;
	public static int nTotalStacked;
	//updated from GTB_Graspable.cs
	public static int nTotalStacked_N;
	public static int nTotalStacked_F;

	//handcontroller GO
	private static GameObject[] handControllers;
	private GameObject bubble;

	public GTB_GraspablesSet (int id_set_nearFar, GameObject[] grasps, GameObject heightDetector, int targetHeight, GameObject tBaseDetector, GameObject[] tDetectors, GameObject[] tDetectorsMeshes, GameObject it3d, Text itc, AudioSource aso, AudioClip acl, GameObject[] hControllers, GameObject bub)
	{
		//interaction objects
		this.graspables = grasps;
		this.heightDetector = heightDetector;
		this.heightRequired = targetHeight;

		//tower
		tower = new Stack<GameObject> ();
		towerBaseDetector = tBaseDetector;
		towerAxisDetectors = tDetectors;
		towerAxisDetectorsMeshes = tDetectorsMeshes;

		//[0] to count the total remaining time.
		//[1] seconds of evaluation of the tower, seconds to show results.
		hrtm = new HRTManager ();
		hrtm.ResetSamplingFrequency ();
		hrtm.CreateTimers (2);

		//database
		this.id_set_nearFar = id_set_nearFar;
		APP_Manager.Instance.insertOrUpdateCurrentItemAttemptInDB (this.id_set_nearFar, 0, 0.0d);

		//database initial values
		id_CurrentGraspablesSetOwner = -1;
		currentGraspablePosition = ITEM_POSITION.NONE;

		nTotalStacked = 0;
		nTotalStacked_N = 0;
		nTotalStacked_F = 0;

		//feedback text
		instrText3D = it3d;
		instrTextCanvas = itc;
		tm = instrText3D.GetComponent<TextMesh> ();
		tc = instrTextCanvas;
		instString = "";
		towerString = "Apilados ";

		//feeback audio
		asource = aso;
		aclip = acl;
		bombTimerSound = new GTB_BombTimerSound (asource, aclip);

		//handcontroller
		handControllers = hControllers;
		this.bubble = bub;
		this.bubble.SetActive (false);

		//interaction initial values
		this.heightDetector.SetActive (false);
		setAllGraspables (false);
		setAllGraspablesKinematic (true);
	}


	/// <summary>
	/// TOWER
	/// </summary>
	/// <returns><c>true</c>, if stacked was ised, <c>false</c> otherwise.</returns>
	/// <param name="gra">Gra.</param>
	private bool isStacked (int gra)
	{
		return graspables [gra].GetComponent<GTB_Graspable> ().graspable_state == GTB_Graspable.GTB_GRASPABLEITEM_STATE.STACKED;
	}

	public static void setTowerAxisDetector (int td, bool b)
	{
		towerAxisDetectors [td].SetActive (b);
	}

	public static void setAllTowerAxisDetectors (bool b)
	{
		for (int i = 0; i < towerAxisDetectors.Length; i++) {
			setTowerAxisDetector (i, b);
		}
	}

	public static void setExclusiveTowerAxisDetector (int td, bool b)
	{
		for (int i = 0; i < towerAxisDetectors.Length; i++) {
			if (i == td)
				setTowerAxisDetector (i, b);
			else
				setTowerAxisDetector (i, !b);
		}
	}

	public static void setTowerAxisDetectorMesh (int td, bool b)
	{
		if (td < towerAxisDetectorsMeshes.Length) {
			towerAxisDetectorsMeshes [td].SetActive (b);
		} else {
//			Debug.Log (td.ToString () + " Out of towerAxisDetectorsMeshes range");
		}
	}

	public static void setAllTowerAxisDetectorMeshes (bool b)
	{
		for (int i = 0; i < towerAxisDetectors.Length; i++) {
			setTowerAxisDetectorMesh (i, b);
		}
	}

	public static void setExclusiveTowerAxisDetectorMesh (int td, bool b)
	{
		for (int i = 0; i < towerAxisDetectorsMeshes.Length; i++) {
			if (i == td)
				setTowerAxisDetectorMesh (i, b);
			else
				setTowerAxisDetectorMesh (i, !b);
		}
	}

	private void towerKinematic (bool b)
	{
		GameObject[] t = tower.ToArray ();
		for (int i = 0; i < t.Length; i++) {
			t [i].GetComponent<Rigidbody> ().isKinematic = b;
		}
	}

	private bool towerHasResisted ()
	{
		bool hasResisted = true;
		GameObject[] t = tower.ToArray ();
		if (t.Length >= 2) {
			for (int i = t.Length - 1; i >= 0; i--) {
				if (t [i].GetComponent<GTB_Graspable> ().graspable_state != GTB_Graspable.GTB_GRASPABLEITEM_STATE.STACKED) {
					hasResisted = false;
					break;
				}
			}
		}
		return hasResisted;
	}

	/// <summary>
	/// Inits the graspables.
	/// </summary>
	public void initGraspables ()
	{
		//Tower
		tower.Clear ();
		towerBaseDetector.SetActive (true);
		setAllTowerAxisDetectors (false);
		setExclusiveTowerAxisDetectorMesh (0, true);
		this.heightDetector.SetActive (true);
		targetHeight = this.heightRequired;

		//Game
		currentGraspable = 0;
		nextGraspable = 0;
		setHandControllers (true);
		this.bubble.SetActive (false);

		//database
		id_CurrentGraspablesSetOwner = -1;
		currentGraspablePosition = ITEM_POSITION.NONE;

		//Feedback audio
		asource.clip = aclip;
		asource.loop = false;
		asource.Play ();

		//Initial values
		remainingTime = 120.0d;   //seconds
		hrtm.Timers [0].InitCounting ();   //starts counting	
		gSet_state = GTB_GRASPABLESET_STATE.LOADING_NEXT_GRASPABLE;
	}

	private void setGraspable (int gra, bool b)
	{
//		Debug.Log(graspables[gra].name +" "+ b.ToString());
		graspables [gra].SetActive (b);

		APP_Manager.Instance.updateCurrentItemInApp (this.id_set_nearFar);
		id_CurrentGraspablesSetOwner = this.id_set_nearFar;

		if (this.id_set_nearFar == (int)IDS_ITEMS.ID_GRASPABLES_SET_FAR) {
			currentGraspablePosition = ITEM_POSITION.FAR;
		} else {
			currentGraspablePosition = ITEM_POSITION.NEAR;
		}
	}

	public static int get_CurrentGraspablesSetOwner_ID ()
	{
		return id_CurrentGraspablesSetOwner;
	}

	public static ITEM_POSITION get_CurrentGraspablePosition ()
	{
		return currentGraspablePosition;
	}

	private void setAllGraspables (bool b)
	{
		for (int i = 0; i < graspables.Length; i++) {
			setGraspable (i, b);
		}
	}

	private void setGraspableKinematic (int gra, bool b)
	{
		graspables [gra].GetComponent<Rigidbody> ().isKinematic = b;
	}

	private void setAllGraspablesKinematic (bool b)
	{
		for (int i = 0; i < graspables.Length; i++) {
			setGraspableKinematic (i, b);
		}
	}

	public static double getRemainingTime ()
	{
		hrtm.Timers [0].EndCounting ();
		return remainingTime - hrtm.Timers [0].GetLastPeriodSecs ();
	}

	public static double getGraspablesSetLastPeriodSecs ()
	{
		hrtm.Timers [0].EndCounting ();
		return hrtm.Timers [0].GetLastPeriodSecs ();
	}

	public GTB_GRASPABLESET_STATE getGSetState ()
	{
		return gSet_state;
	}

	private void updateInstrText ()
	{
		tm.text = towerString + tower.Count.ToString ();
		tm.text += "\n\n" + instString + ((int)getRemainingTime ()).ToString () + " s";

		tc.text = towerString + tower.Count.ToString ();
		tc.text += "\n\n" + instString + ((int)getRemainingTime ()).ToString () + " s";
	}

	private void towerEvaluationText ()
	{
		if ((targetHeight == 4 && tower.Count >= 2) || (targetHeight == 6 && tower.Count >= 4)) {
			tm.text = towerString + tower.Count.ToString ();
			tm.text += "\n\n" + "Evaluando la resistencia";

			tc.text = towerString + tower.Count.ToString ();
			tc.text += "\n\n" + "Evaluando la resistencia";
		} else {
			tm.text = "\n\n" + "¡No hay suficientes!";
			tc.text = "\n\n" + "¡No hay suficientes!";
		}

		//voice
		APP_Voices.Instance.playTowerVoice ((int)APP_VOICES_TOWER.EVALUATINGTHERESISTANCE);
	}

	private void towerHasResistedText (bool b)
	{
		if ((targetHeight == 4 && tower.Count >= 2) || (targetHeight == 6 && tower.Count >= 4)) {
			if (b) {
				tm.text = "¡Ha resistido!";
				tc.text = "¡Ha resistido!";
			} else {
				tm.text = "¡No ha resistido!";
				tc.text = "¡No ha resistido!";
			}
		} else {
			tm.text = "¡No hay suficientes!";
			tc.text = "¡No hay suficientes!";
		}
	}

	public void destroyAllGraspables ()
	{
		for (int i = 0; i < graspables.Length; i++) {
			if (graspables [i] != null) {
				GameObject.Destroy (graspables [i]);
			}
		}
	}

	public void destroyNoFeasibles ()
	{
		for (int i = 0; i < graspables.Length; i++) {
			if (graspables [i].GetComponent<GTB_Graspable> ().graspable_state != GTB_Graspable.GTB_GRASPABLEITEM_STATE.STACKED) {
				GameObject.Destroy (graspables [i]);
			}
		}
	}

	public double getGSetAccumulatedSecs ()
	{
		return hrtm.Timers [0].GetAccumulatedSecs ();
	}

	private void setHandControllers (bool b)
	{
		for (int i = 0; i < handControllers.Length; i++) {
			handControllers [i].SetActive (b);
		}
	}

	private void updateSecondsSound ()
	{
		if (getRemainingTime () > 15.0f) {
			bombTimerSound.secondSound (hrtm.Timers [0].GetLastPeriodSecs ());
		} else {
			if (getRemainingTime () > 5.0f) {
				bombTimerSound.halfSecondSound (hrtm.Timers [0].GetLastPeriodms ());
			} else {
				bombTimerSound.quarterSecondSound (hrtm.Timers [0].GetLastPeriodms ());
			}
		}
	}

	private void towerEvaluation_Init ()
	{
		destroyNoFeasibles ();

		towerEvaluationText ();
		towerKinematic (false);
		setHandControllers (false);
		this.bubble.GetComponent<GTB_Bubble> ().init ();
	}

	private void towerEvaluation_End ()
	{
		towerKinematic (true);
		setHandControllers (true);
		this.bubble.GetComponent<GTB_Bubble> ().end ();
		bool hr = towerHasResisted ();
		towerHasResistedText (hr);    //need be called after the tower is in kinematic mode
		if (hr) {
			APP_Voices.Instance.playTowerVoice ((int)APP_VOICES_TOWER.RESISTEDYES);
		} else {
			APP_Voices.Instance.playTowerVoice ((int)APP_VOICES_TOWER.RESISTEDNO);
		}
	}

	public void gtb_GSetFsm ()
	{
		switch (gSet_state) {
		case GTB_GRASPABLESET_STATE.LOADING_NEXT_GRASPABLE:
			setGraspable (nextGraspable, true);

			//Very Important Update
			currentGraspable = nextGraspable;
			nextGraspable++;

			gSet_state = GTB_GRASPABLESET_STATE.PLAYING;
			break;

		case GTB_GRASPABLESET_STATE.PLAYING:
			if (getRemainingTime () > 0.0f) {
				if (nextGraspable < targetHeight && nextGraspable < graspables.Length) {
					if (isStacked (currentGraspable)) {
						gSet_state = GTB_GRASPABLESET_STATE.LOADING_NEXT_GRASPABLE;
					}
				} else {
					//Last graspable
					if (isStacked (currentGraspable)) {
						asource.Stop ();
						gSet_state = GTB_GRASPABLESET_STATE.ALL_STACKED;
					}
				}
				updateInstrText ();
				updateSecondsSound ();
			} else {
				asource.Stop ();
				gSet_state = GTB_GRASPABLESET_STATE.DEADLINE;
			}
			break;

		case GTB_GRASPABLESET_STATE.ALL_STACKED:
			hrtm.Timers [0].Accumulate ();
			heightDetector.SetActive (false);
			towerEvaluation_Init ();

			hrtm.Timers [1].InitCounting ();
			gSet_state = GTB_GRASPABLESET_STATE.TOWER_EVALUATION;
			break;

		case GTB_GRASPABLESET_STATE.DEADLINE:
			hrtm.Timers [0].Accumulate ();
			heightDetector.SetActive (false);
			towerEvaluation_Init ();

			hrtm.Timers [1].InitCounting ();
			gSet_state = GTB_GRASPABLESET_STATE.TOWER_EVALUATION;
			break;

		case GTB_GRASPABLESET_STATE.TOWER_EVALUATION:	
			hrtm.Timers [1].EndCounting ();
			if (hrtm.Timers [1].GetLastPeriodSecs () > 7.5f) {
				towerEvaluation_End ();

				hrtm.Timers [1].InitCounting ();   //reset
				gSet_state = GTB_GRASPABLESET_STATE.TOWER_RESULTS;
			}
			break;

		case GTB_GRASPABLESET_STATE.TOWER_RESULTS:
			hrtm.Timers [1].EndCounting ();
			if (hrtm.Timers [1].GetLastPeriodSecs () > 2f) {
				gSet_state = GTB_GRASPABLESET_STATE.END;
			}
			break;

		case GTB_GRASPABLESET_STATE.END:
			break;
		}
	}

}
