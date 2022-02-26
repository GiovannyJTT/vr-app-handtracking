using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DBTfm;
using DBTfm.Types;
using APPTfm;	

public class GFB_ItemsSet
{
	public enum GFB_SET_STATE
	{
		LOADING_NEXT_ITEM = 0,
		PLAYING = 1,
		ALL_ITEMS_CONSUMED = 2,
		DEADLINE = 3,
		///ALL TIME CONSUMED AND NOT ALL ITEMS CONSUMED
		END = 4
	}

	public static GFB_SET_STATE set_state;

	private GameObject[] hittables;
	private static int current;
	private static int next;
	private static HRTManager hrtm;
	private static double remainingTime;
	private static GameObject instrText3D;
	private static Text instrTextCanvas;
	private static TextMesh tm;
	private static Text tc;
	private static string instString;
	private static string goalsString;
	private static AudioSource asource;
	private static AudioClip aclip;
	private int id_set_l;
	private int id_set_r;
	private static int id_CurrentItemsSetOwner;
	private static ITEM_POSITION currentItemPosition;

	public static int nGoalsObtained;
	public static int nGoalsObtained_L;
	public static int nGoalsObtained_R;
	public static float distanceToConsiderBeaten;
	public static float distanceToTheNext;

	public GFB_ItemsSet (int id_set_l, int id_set_r, GameObject[] hitas, GameObject it3d, Text itc, AudioSource aso, AudioClip acl)
	{
		this.hittables = hitas;
		this.id_set_l = id_set_l;
		this.id_set_r = id_set_r;
		APP_Manager.Instance.insertOrUpdateCurrentItemAttemptInDB(this.id_set_l, 0, 0.0d);
		APP_Manager.Instance.insertOrUpdateCurrentItemAttemptInDB(this.id_set_r, 0, 0.0d);

		id_CurrentItemsSetOwner = -1;
		currentItemPosition = ITEM_POSITION.NONE;

		instrText3D = it3d;
		instrTextCanvas = itc;

		tm = instrText3D.GetComponent<TextMesh> ();
		tc = instrTextCanvas;

		asource = aso;
		aclip = acl;

		instString = "";
		goalsString = "Goles ";

		nGoalsObtained = 0;   ///updated from GFB_GoalInteriorDetector.cs
		nGoalsObtained_L = 0;
		nGoalsObtained_R = 0;
		distanceToConsiderBeaten = 0.25f;    //0.25 m, 25 cm
		distanceToTheNext = 0.75f;    //0.75 m, 75 cm
		//distanceToTheNext must be less than distanceToConsiderBeaten with a minimum difference of 0.5

		//[0] to count the total remaining time.
		//[1] to wait for some seconds after the last item appears.
		hrtm = new HRTManager ();
		hrtm.ResetSamplingFrequency ();
		hrtm.CreateTimers (2);

		initItems ();
	}

	public void initItems ()
	{
		setAllItems (false);
		setAllItemsKinematicAndTrigger (false);

		current = 0;
		next = 0;

		nGoalsObtained = 0;
		nGoalsObtained_L = 0;
		nGoalsObtained_R = 0;

		id_CurrentItemsSetOwner = -1;
		currentItemPosition = ITEM_POSITION.NONE;

		asource.clip = aclip;
		asource.loop = true;
		asource.Play ();

		remainingTime = 15.0d;   ///seconds
		hrtm.Timers [0].InitCounting ();   //starts counting

		set_state = GFB_SET_STATE.LOADING_NEXT_ITEM;
	}

	private void setItem (int it, bool b)
	{
//		Debug.Log(hittables[it].name +" "+ b.ToString());
		hittables [it].SetActive (b);

		if(b){
			if(it % 2 == 0){
				///Left
				APP_Manager.Instance.updateCurrentItemInApp(id_set_l);
				id_CurrentItemsSetOwner = id_set_l;
				currentItemPosition = ITEM_POSITION.LEFT;
//				Debug.Log(APP_Manager.Instance.getCurrentItem().id);
			} else {
				///Right
				APP_Manager.Instance.updateCurrentItemInApp(id_set_r);
				id_CurrentItemsSetOwner = id_set_r;
				currentItemPosition = ITEM_POSITION.RIGHT;
//				Debug.Log(APP_Manager.Instance.getCurrentItem().id);
			}
		}
	}

	public static int get_CurrentItemsSetOwner_ID(){
		return id_CurrentItemsSetOwner;
	}

	public static ITEM_POSITION get_CurrentItemPosition(){
		return currentItemPosition;
	}

	private void setAllItems (bool b)
	{
		for (int i = 0; i < hittables.Length; i++) {
			setItem (i, b);
		}
	}

	private void setItemKinematicAndTrigger (int it, bool b)
	{
		hittables [it].GetComponent<Rigidbody> ().isKinematic = b;

		if(hittables [it].GetComponent<MeshCollider> () != null)
			hittables [it].GetComponent<MeshCollider> ().isTrigger = b;
		if(hittables [it].GetComponent<SphereCollider> () != null	)
			hittables [it].GetComponent<SphereCollider> ().isTrigger = b;
		if(hittables [it].GetComponent<CapsuleCollider> () != null	)
			hittables [it].GetComponent<CapsuleCollider> ().isTrigger = b;
		if(hittables [it].GetComponent<BoxCollider> () != null)
			hittables [it].GetComponent<BoxCollider> ().isTrigger = b;
	}

	private void setAllItemsKinematicAndTrigger (bool b)
	{
		for (int i = 0; i < hittables.Length; i++) {
			setItemKinematicAndTrigger (i, b);
		}
	}

	public static double getRemainingTime ()
	{
		hrtm.Timers [0].EndCounting ();
		return remainingTime - hrtm.Timers [0].GetLastPeriodSecs ();
	}

	public static double getItemsSetLastPeriodSecs(){
		hrtm.Timers [0].EndCounting ();
		return hrtm.Timers [0].GetLastPeriodSecs ();
	}

	private bool enoughDistanceToTheNext ()
	{
		return Vector3.Distance (hittables [current].transform.position, hittables [next].transform.position) > distanceToTheNext;
	}

	public GFB_SET_STATE getSetState ()
	{
		return set_state;
	}

	private void updateInstrText ()
	{
		tm.text = goalsString + nGoalsObtained.ToString ();
		tm.text += "\n\n" + instString + ((int)getRemainingTime ()).ToString () + " s";

		tc.text = goalsString + nGoalsObtained.ToString ();
		tc.text += "\n\n" + instString + ((int)getRemainingTime ()).ToString () + " s";
	}

	private void destroyNoFeasibles ()
	{
		for (int i = 0; i < hittables.Length; i++) {
			if (hittables [i] != null && hittables [i].GetComponent<Rigidbody> () != null){
				GameObject.Destroy (hittables [i]);
			}
		}
	}

	public void destroyAllItems ()
	{
		for (int i = 0; i < hittables.Length; i++) {
			if (hittables [i] != null) {
				GameObject.Destroy (hittables [i]);
			}
		}
	}

	public double getItemAccumulatedSecs ()
	{
		return hrtm.Timers [0].GetAccumulatedSecs ();
	}

	public void gfb_itemFsm ()
	{
		switch (set_state) {
		case GFB_SET_STATE.LOADING_NEXT_ITEM:
			setItem (next, true);

			//Very Important Update
			current = next;
			next++;

			set_state = GFB_SET_STATE.PLAYING;
			break;

		case GFB_SET_STATE.PLAYING:
			if (getRemainingTime () > 0.0f) {
				updateInstrText ();
				if (next < hittables.Length) {
					if (hittables [current] == null || enoughDistanceToTheNext ()) {
						set_state = GFB_SET_STATE.LOADING_NEXT_ITEM;
					}
				} else {
					asource.Stop ();
					hrtm.Timers[1].InitCounting();
					set_state = GFB_SET_STATE.ALL_ITEMS_CONSUMED;
				}
			} else {
				asource.Stop ();
				set_state = GFB_SET_STATE.DEADLINE;
			}
			break;

		case GFB_SET_STATE.ALL_ITEMS_CONSUMED:
			hrtm.Timers[1].EndCounting();
			if(hrtm.Timers[1].GetLastPeriodSecs() > 1.0f){
				hrtm.Timers[0].Accumulate();
				destroyNoFeasibles ();
				set_state = GFB_SET_STATE.END;
			}
			break;

		case GFB_SET_STATE.DEADLINE:
			hrtm.Timers[0].Accumulate();
			destroyNoFeasibles ();
			set_state = GFB_SET_STATE.END;
			break;

		case GFB_SET_STATE.END:
			break;
		}
	}

}
