using UnityEngine;
using System.Collections;
using DBTfm.Types;
using APPTfm;
using APPVoices;
using APPVoices.Types;

public class GTB_Graspable : MonoBehaviour
{
	public enum GTB_GRASPABLEITEM_STATE
	{
		INITIAL_POSITION = 0,
		IRRELEVANT_COLLISIONS = 1,
		COLLIDING_WITH_TOWERBASE = 2,
		COLLIDING_WITH_TOWERTOP = 3,
		COLLIDING_WITH_TOWERAXISDETECTOR = 4,
		STACKED = 5
	}

	[HideInInspector]
	public GTB_GRASPABLEITEM_STATE graspable_state;
	private Rigidbody rb;
	private MeshRenderer mr;
	public AudioClip aclipOk;
	public AudioClip aclipFail;
	private AudioSource asource;

	private Material[] origMats;
	public Material[] grayMats;

	private static float distanceToConsiderWithinAxis;
	private int heightOfThisItem;
	private Vector3 initialPosOfThisItem;
	private float distanceToConsiderMovedFromOriginalPos;

	void Start ()
	{
		rb = this.GetComponent<Rigidbody>();
		mr = this.GetComponent<MeshRenderer>();
		origMats = mr.materials;
		setMaterials(grayMats);
		asource = this.GetComponent<AudioSource>();
	
		distanceToConsiderWithinAxis = 0.02f;    //0.015m, 1.5cm
		heightOfThisItem = -1;
		initialPosOfThisItem = this.transform.position;
		distanceToConsiderMovedFromOriginalPos = 0.02f;    //0.02m, 2cm
	
		graspable_state = GTB_GRASPABLEITEM_STATE.INITIAL_POSITION;
	}

	private void setMaterials(Material[] m){
		mr.materials = m;
	}

	private bool isTowerBase (Collider cld)
	{
		return cld == GTB_GraspablesSet.towerBaseDetector.GetComponent<Collider> ();
	}

	private bool isTowerTop (Collider cld)
	{
		if(GTB_GraspablesSet.tower.Count > 0)
			return cld == GTB_GraspablesSet.tower.Peek(). GetComponent<Collider> ();
		else
			return false;
	}

	private bool isTowerAxisDetector (Collider cld, int index)
	{
		return cld == GTB_GraspablesSet.towerAxisDetectors [index].GetComponent<Collider> ();
	}

	private bool isFrontFaceUp(){
		if(this.transform.up.y > 0.8f){
			if(GTB_GraspablesSet.tower.Count == 0)
				return this.transform.up.y > 0.998f;
			else
				return this.transform.up.y > 0.995f;
		}			
		else {
			return false;
		}
	}

	private bool isBackFaceUp(){
		if(this.transform.up.y < -0.8){
			if(GTB_GraspablesSet.tower.Count == 0)
				return this.transform.up.y < -0.998f;
			else
				return this.transform.up.y < -0.995f;
		}
		else {
			return false;
		}
	}

	private bool isInHorizontal(){
//		Debug.Log(this.transform.up.y.ToString());
		return isFrontFaceUp() || isBackFaceUp();
	}

	private bool isWithinTowerAxis(int indexAxisDetector){
		if(GTB_GraspablesSet.tower.Count == 0){
			return Vector3.Distance(this.transform.position, GTB_GraspablesSet.towerAxisDetectors[indexAxisDetector].transform.position) < distanceToConsiderWithinAxis*0.75f;
		} else {
			return Vector3.Distance(this.transform.position, GTB_GraspablesSet.towerAxisDetectors[indexAxisDetector].transform.position) < distanceToConsiderWithinAxis;
		}
	}

	private bool isInInitialPosition(){
		return Vector3.Distance(this.initialPosOfThisItem, this.transform.position) < distanceToConsiderMovedFromOriginalPos;
	}

	private void graspColliderEnter_Fsm (Collider cld)
	{
		switch (this.graspable_state) {
		case GTB_GRASPABLEITEM_STATE.INITIAL_POSITION:
			//attended in update function
		break;
		case GTB_GRASPABLEITEM_STATE.IRRELEVANT_COLLISIONS:
			if (GTB_GraspablesSet.tower.Count == 0) {
				if (isTowerBase (cld)) {
					if (GTB_GraspablesSet.tower.Count < GTB_GraspablesSet.targetHeight){
						GTB_GraspablesSet.setTowerAxisDetector (GTB_GraspablesSet.tower.Count, true);     ///Enabling the first spherical towerAxisDetector
					}
					this.graspable_state = GTB_GRASPABLEITEM_STATE.COLLIDING_WITH_TOWERBASE;
				}
			} else {
				if (isTowerTop (cld)) {
					if (GTB_GraspablesSet.tower.Count < GTB_GraspablesSet.targetHeight){
						GTB_GraspablesSet.setTowerAxisDetector (GTB_GraspablesSet.tower.Count, true);    ///Enabling the next spherical towerAxisDetector
					}

					if(GTB_GraspablesSet.tower.Count == 1){
						//voice
						APP_Voices.Instance.first_CENTERIT = true;
					}

					this.graspable_state = GTB_GRASPABLEITEM_STATE.COLLIDING_WITH_TOWERTOP;
				}
				///else continue with irrelevant collisions
			}
			break;
		case GTB_GRASPABLEITEM_STATE.COLLIDING_WITH_TOWERBASE:
			if(isTowerAxisDetector(cld, GTB_GraspablesSet.tower.Count)){
				graspable_state = GTB_GRASPABLEITEM_STATE.COLLIDING_WITH_TOWERAXISDETECTOR;
			}
		break;
		case GTB_GRASPABLEITEM_STATE.COLLIDING_WITH_TOWERTOP:
			if (isTowerAxisDetector (cld, GTB_GraspablesSet.tower.Count)) {
				this.heightOfThisItem = GTB_GraspablesSet.tower.Count;
				this.graspable_state = GTB_GRASPABLEITEM_STATE.COLLIDING_WITH_TOWERAXISDETECTOR;
			}
			break;
		case GTB_GRASPABLEITEM_STATE.COLLIDING_WITH_TOWERAXISDETECTOR:
			//attended in update function
			break;
		case GTB_GRASPABLEITEM_STATE.STACKED:
			//attended in update function
			break;
		}

//		Debug.Log ("Enter " + GTB_GraspablesSet.tower.Count.ToString () + "/" + GTB_GraspablesSet.targetHeight.ToString () + " " + this.graspable_state.ToString ());
	}

	/// <summary>
	/// Grasps the collision exit fsm. ONLY FOR DETECT IF THE TOWER NOT RESISTS.
	/// </summary>
	/// <param name="cld">Cld.</param>
	private void graspColliderExit_Fsm (Collider cld)
	{
		switch (this.graspable_state) {
		case GTB_GRASPABLEITEM_STATE.STACKED:
			if(!this.rb.isKinematic && cld == GTB_GraspablesSet.towerAxisDetectors[this.heightOfThisItem].GetComponent<Collider>()){
//				Debug.Log ("Exit " + cld.name + " from " + this.name);

				//feeback graphics
				this.setMaterials (grayMats);

				//feedback audio
				asource.Stop();
				asource.volume = 0.5f;
				asource.pitch = 1f;
				asource.PlayOneShot(aclipFail);

				//update state
				this.graspable_state = GTB_GRASPABLEITEM_STATE.IRRELEVANT_COLLISIONS;
			}
			break;
		}
	}

	//collisions with spherical towerDetector
	void OnTriggerEnter (Collider cld)
	{
		graspColliderEnter_Fsm (cld);
	}

	//collisions with other GTB_GraspableCollision.cs
	void OnCollisionEnter (Collision cls)
	{
		graspColliderEnter_Fsm (cls.collider);
	}

	void OnTriggerExit(Collider cld){
		graspColliderExit_Fsm(cld);
	}

	void OnCollisionExit(Collision cls){
		graspColliderExit_Fsm(cls.collider);
	}

	/// <summary>
	/// Stores or updates the current stacked graspable in the Database.
	/// Only one graspable item is active in the scene until it is stacked.
	/// This function needs be called just before update the graspable_state to "STACKED".
	/// </summary>
	private void stackedInDB(){
		GTB_GraspablesSet.nTotalStacked++;

		if(ITEM_POSITION.NEAR == GTB_GraspablesSet.get_CurrentGraspablePosition()){
			GTB_GraspablesSet.nTotalStacked_N++;

			APP_Manager.Instance.insertOrUpdateCurrentItemAttemptInDB(
			GTB_GraspablesSet.get_CurrentGraspablesSetOwner_ID(),
			GTB_GraspablesSet.nTotalStacked_N,
			GTB_GraspablesSet.getGraspablesSetLastPeriodSecs()
			);
		} else {
			GTB_GraspablesSet.nTotalStacked_F++;

			APP_Manager.Instance.insertOrUpdateCurrentItemAttemptInDB(
			GTB_GraspablesSet.get_CurrentGraspablesSetOwner_ID(),
			GTB_GraspablesSet.nTotalStacked_F,
			GTB_GraspablesSet.getGraspablesSetLastPeriodSecs()
			);
		}
	}

	void Update ()
	{
		switch (this.graspable_state) {
		case GTB_GRASPABLEITEM_STATE.INITIAL_POSITION:
			if(!isInInitialPosition()){
				this.graspable_state = GTB_GRASPABLEITEM_STATE.IRRELEVANT_COLLISIONS;
			}
		break;
		case GTB_GRASPABLEITEM_STATE.COLLIDING_WITH_TOWERAXISDETECTOR:
			if (isInHorizontal ()){
				if (isWithinTowerAxis (GTB_GraspablesSet.tower.Count)) {
					//feeback graphics
					this.setMaterials (origMats);
					this.GetComponent<Rigidbody>().isKinematic = true;

					//update tower
					this.heightOfThisItem = GTB_GraspablesSet.tower.Count;
//					Debug.Log(this.heightOfThisItem);
					GTB_GraspablesSet.tower.Push (this.gameObject);
					if(GTB_GraspablesSet.tower.Count < GTB_GraspablesSet.targetHeight){
						GTB_GraspablesSet.setTowerAxisDetectorMesh (GTB_GraspablesSet.tower.Count, true);
					}

					//feedback audio
					asource.Stop();
					asource.volume = 0.5f;
					asource.pitch = 1f;
					asource.PlayOneShot(aclipOk);

					//database
					stackedInDB();

					//update state
					this.graspable_state = GTB_GRASPABLEITEM_STATE.STACKED;
				}
				else {
					APP_Voices.Instance.playTowerVoice((int)APP_VOICES_TOWER.CENTERIT);
				}
			}
			break;
		case GTB_GRASPABLEITEM_STATE.STACKED:
			//nothing to do (version 0)
			break;
		}
	}
}
