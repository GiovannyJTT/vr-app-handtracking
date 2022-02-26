using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DBTfm;
using APPTfm;
using APPVoices;
using APPVoices.Types;

/// <summary>
/// GameReactionTime item. Static properties avoid duplicate information.
/// </summary>
public class GRT_Item
{
	/// <summary>
	/// GameReactionTime Item state.
	/// </summary>
	public enum GRT_ITEM_STATE
	{
		TURNING,
		TURNED,
		INCENTIVIZING,
		LINED
	}

	public GRT_ITEM_STATE item_state;

	/// <summary>
	/// The Initial rotation of the target object.
	/// </summary>
	private static Quaternion rot;
	private static float marginRads;

	/// <summary>
	/// The name, the target, the targetLighting and targetReached, are no statics because each item will have their own values.
	/// </summary>
	private int id_item;
	private string name;
	private GameObject target;
	private Quaternion targetRotOrig;
	private GameObject targetLighting;
	private bool targetReached;

	public List<GameObject> guides;
	public List<GameObject> guidesLighting;

	public string instrString;
	//no static because use the different names of the models

	/// <summary>
	/// The incent string and won string are static because is the same for all the items.
	/// </summary>
	public static string incentString;
	public string wonString;

	/// <summary>
	/// The text3d to display instructions in the GameReactionTime.
	/// </summary>
	public static GameObject text3dInstr;
	private static TextMesh tm;
	public static Text textCanvas;


	/// <summary>
	/// The High resolution timer manager to control all timers.
	/// </summary>
	private static HRTManager hrtm;

	//feedback audio when item is aligned
	private static AudioSource asource;
	private static AudioClip aclipitemok;

	/// <summary>
	/// Initializes a new instance of the <see cref="GRT_Item"/> class.
	/// </summary>
	/// <param name="n">Name.</param>
	/// <param name="t">Target GameObject.</param>
	/// <param name="tL">Target lighting.</param>
	/// <param name="g">Guide GameObject.</param>
	/// <param name="gL">Guide Lighting.</param>
	/// <param name="t3d">Text 3D.</param>
	/// <param name="tc">Text Canvas.</param>
	public GRT_Item (int id_i, string n, GameObject t, GameObject tL, List<GameObject> g, List<GameObject> gL, GameObject t3d, Text tc, AudioSource aSource, AudioClip aClipItemOK)
	{
		this.id_item = id_i;
		this.name = n; 
		this.target = t;
		this.targetRotOrig = t.transform.rotation;
		this.targetLighting = tL;
		this.targetReached = false;

		this.guides = new List<GameObject> (g);    //copy
		this.guidesLighting = new List<GameObject> (gL);

		this.target.SetActive (false);
		this.targetLighting.SetActive (false);
		for (int i = 0; i < g.Count; i++) {
			this.guides [i].SetActive (false);
			this.guidesLighting [i].SetActive (false);
		}

		instrString = "Coloca " + name + "\nmirando hacia ti.\n";
		incentString = "Fíjate en las figuras.\n\n";
		this.wonString = "¡Bien!\nObserva " + name + ".\n";

		text3dInstr = t3d;
		textCanvas = tc;

		//Load needed initial elements
		tm = text3dInstr.GetComponent<TextMesh> ();

		//feeback audio when item is aligned
		asource = aSource;
		aclipitemok = aClipItemOK;

		//Compute Initial values
		marginRads = 0.174532925199389f;   //10f * 3.141592653589f / 180.0f;
		rot = Quaternion.Euler (-90.0f, 180.0f, 0.0f);

		//Create Timers
		//[0] For keep Reaction Time of each item.
		//[1] For incentivize the player with messages in then 3d world.
		hrtm = new HRTManager ();
		hrtm.ResetSamplingFrequency ();
		hrtm.CreateTimers (2);
	}

	bool isInMargins ()
	{
		return (
		  (target.transform.rotation.x > guides [0].transform.rotation.x - marginRads) &&
		  (target.transform.rotation.x < guides [0].transform.rotation.x + marginRads) &&

		  (target.transform.rotation.y > guides [0].transform.rotation.y - marginRads) &&
		  (target.transform.rotation.y < guides [0].transform.rotation.y + marginRads) &&

		  (target.transform.rotation.z > guides [0].transform.rotation.z - marginRads) &&
		  (target.transform.rotation.z < guides [0].transform.rotation.z + marginRads)
		);
	}

	void setCountingString ()
	{
		tm.text = instrString + ((int)hrtm.Timers [0].GetLastPeriodSecs ()).ToString () + " s";
		textCanvas.text = instrString + ((int)hrtm.Timers [0].GetLastPeriodSecs ()).ToString () + " s";
	}

	void setIncentString ()
	{
		tm.text = incentString + ((int)hrtm.Timers [0].GetLastPeriodSecs ()).ToString () + " s";
		textCanvas.text = incentString + ((int)hrtm.Timers [0].GetLastPeriodSecs ()).ToString () + " s";

		//voice
		APP_Voices.Instance.playMuseumVoice ((int)APP_VOICES_MUSEUM.LOOKATTHEFIGURES);
	}

	void setWonString ()
	{
		tm.text = ((int)hrtm.Timers [0].GetLastPeriodSecs ()).ToString () + " s\n" + wonString;
		textCanvas.text = ((int)hrtm.Timers [0].GetLastPeriodSecs ()).ToString () + " s\n" + wonString;
	}

	/// <summary>
	/// Finite State Machine for control the transitions between the diferent states of the Item.
	/// </summary>
	public void grt_Item_Fsm ()
	{
		switch (item_state) {
		case GRT_ITEM_STATE.TURNING:
			//Debug.Log("Turning");
			targetReached = false;
			target.transform.rotation = rot;
			tm.text = instrString;
			textCanvas.text = instrString;

			hrtm.Timers [0].InitCounting ();    //Reset timer of the ReactionTime
			hrtm.Timers [1].InitCounting ();    //Reset timer of incentivization
			item_state = GRT_ITEM_STATE.TURNED;
			break;

		case GRT_ITEM_STATE.TURNED:
			//Debug.Log("Turned");
			hrtm.Timers [0].EndCounting ();
			if (isInMargins ()) {
				targetReached = true;
				resetTargetRot ();

				//feedback audio when item is aligned
				asource.Stop ();
				asource.PlayOneShot (aclipitemok);

				//DB: To store ItemReactionTime in database use GetLastPeriodSecs here
				APP_Manager.Instance.insertOrUpdateCurrentItemAttemptInDB (this.id_item, 1, hrtm.Timers [0].GetLastPeriodSecs ());    //only one attempt no repeats

				hrtm.Timers [0].Accumulate ();    //Accumulate the ItemReactionTime to the TotalReactionTime

				//keep message in the 3d world and jump
				setWonString ();

				item_state = GRT_ITEM_STATE.LINED;
			} else {
				setCountingString ();

				hrtm.Timers [1].EndCounting ();
				if (hrtm.Timers [1].GetLastPeriodSecs () > 15.0f) {
					hrtm.Timers [1].InitCounting ();    //Reset timer of incentivization
					item_state = GRT_ITEM_STATE.INCENTIVIZING;
				}
			}
			break;

		case GRT_ITEM_STATE.INCENTIVIZING:
			//Debug.Log("Incentivizing");
			hrtm.Timers [0].EndCounting ();
			if (isInMargins ()) {
				targetReached = true;
				resetTargetRot ();

				//DB: To store ItemReactionTime in database use GetLastPeriodSecs here
				APP_Manager.Instance.insertOrUpdateCurrentItemAttemptInDB (this.id_item, 1, hrtm.Timers [0].GetLastPeriodSecs ());    //only one attempt no repeats

				hrtm.Timers [0].Accumulate ();    //Accumulate the ItemReactionTime to the TotalReactionTime

				//keep message in the 3d world and jump
				setWonString ();

				item_state = GRT_ITEM_STATE.LINED;
			} else {
				setIncentString ();

				hrtm.Timers [1].EndCounting ();
				if (hrtm.Timers [1].GetLastPeriodSecs () > 1.0f) {
					hrtm.Timers [1].InitCounting ();    //Reset timer of incentivization
					item_state = GRT_ITEM_STATE.TURNED;
				}
			}
			break;

		case GRT_ITEM_STATE.LINED:
			//Debug.Log ("LINED " + name);
			break;
		}
	}

	public double getItemAccumulatedSecs ()
	{
		return hrtm.Timers [0].GetAccumulatedSecs ();
	}

	public void setTargetKinematic (bool b)
	{
		if (target.GetComponent<Rigidbody> () != null)
			target.GetComponent<Rigidbody> ().isKinematic = b;
	}

	public void setTargetActive (bool b)
	{
		target.SetActive (b);
		targetLighting.SetActive (b);
	}

	/// <summary>
	/// Sets the guide sphere points of the guide models.
	/// </summary>
	public void setGuidesSpherePoints (bool b)
	{
		for (int j = 0; j < guides.Count; j++)
			if (guides [j].GetComponent<MeshRenderer> () != null)
				guides [j].GetComponent<MeshRenderer> ().enabled = b;
	}

	public void setGuidesActive (bool b)
	{
		for (int j = 0; j < guides.Count; j++) {
			guides [j].SetActive (b);
			guidesLighting [j].SetActive (b);
		}
	}

	public void setTargetReached (bool b)
	{
		this.targetReached = b;
	}

	public bool getTargetReached ()
	{
		return this.targetReached;
	}

	public void resetTargetRot ()
	{
		this.target.transform.rotation = this.targetRotOrig;
	}

}