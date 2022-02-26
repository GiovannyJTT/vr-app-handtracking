using UnityEngine;
using System.Collections;
using DBTfm.Types;

public class GFB_ItemCollision : MonoBehaviour
{
	private enum GFB_ITEM_STATE
	{
		PREPARED = 0,
		BEATEN = 1,
		NOT_FEASIBLE = 3,
		NO_PHYSICAL = 4
	}

	private GFB_ITEM_STATE item_state;
	private int id_ItemsSetOwner;
	private ITEM_POSITION initial_item_position;
	//no static beacause each item have its own state

	private HRTManager hrtm;
	private Rigidbody rb;
	private Vector3 posOrig;


	void Start ()
	{
		this.posOrig = this.transform.position;
		this.rb = this.GetComponent<Rigidbody> ();
		this.id_ItemsSetOwner = -1;   //not assigned yet
		this.initial_item_position = ITEM_POSITION.NONE;

		//[0] for count the time to destroy a bounced back item.
		this.hrtm = new HRTManager ();
		this.hrtm.ResetSamplingFrequency ();
		this.hrtm.CreateTimers (1);

		this.item_state = GFB_ITEM_STATE.PREPARED;
	}

	private bool enoughDistanceFromOrigin ()
	{
		return Vector3.Distance (posOrig, this.transform.position) > GFB_ItemsSet.distanceToConsiderBeaten;
	}

	public int get_ItemsSetOwner_ID_ofThisItem(){
		return this.id_ItemsSetOwner;
	}

	public ITEM_POSITION get_InitialPosition_ofThisItem(){
		return this.initial_item_position;
	}

	void Update ()
	{
		switch (item_state) {
		case GFB_ITEM_STATE.PREPARED:
			if (rb != null) {
				if (enoughDistanceFromOrigin ()) {
//					Debug.Log("Beaten " + this.name);

					///Very Important Update
					this.id_ItemsSetOwner = GFB_ItemsSet.get_CurrentItemsSetOwner_ID();
					this.initial_item_position = GFB_ItemsSet.get_CurrentItemPosition();
					item_state = GFB_ITEM_STATE.BEATEN;
				}
			}
			break;
		case GFB_ITEM_STATE.BEATEN:
			if (rb != null) {
				///too slow
				if (rb.velocity.sqrMagnitude < 0.1f) {
					hrtm.Timers [0].InitCounting ();
					item_state = GFB_ITEM_STATE.NOT_FEASIBLE;
				}
			} else {
				///GFB_GoalInteriorDetector DESTROYS the rigidbody when the collision with the football goal occurs
				item_state = GFB_ITEM_STATE.NO_PHYSICAL;
			}
			break;
		case GFB_ITEM_STATE.NOT_FEASIBLE:
			hrtm.Timers [0].EndCounting ();
			if (hrtm.Timers [0].GetLastPeriodSecs () > 2.0f) {
				GameObject.Destroy (this.gameObject);
				item_state = GFB_ITEM_STATE.NO_PHYSICAL;
			}
			break;
		case GFB_ITEM_STATE.NO_PHYSICAL:
			///GFB_RotateTransform ROTATES the item that now not have a rigidbody to avoid new collisions that can perturb the user
			break;
		}
	}


	/// <summary>
	/// Raises the trigger enter event. AreaColliders are the colliders that enclose a feasible area in wich the item might reach the goal.
	/// </summary>
	/// <param name="c">C.</param>
	void OnTriggerEnter (Collider c)
	{
		if (item_state == GFB_ITEM_STATE.BEATEN &&
		   (c.name == "AreaCollider_Front" ||
		   c.name == "AreaCollider_Back" ||
		   c.name == "AreaCollider_Left" ||
		   c.name == "AreaCollider_Right" ||
		   c.name == "AreaCollider_Up")
		   ) {
//			Debug.Log("NOTFEASIBLE " + this.name);
			hrtm.Timers [0].InitCounting ();
			item_state = GFB_ITEM_STATE.NOT_FEASIBLE;
		}
	}
}
