using UnityEngine;
using System.Collections;
using DBTfm.Types;
using APPTfm;

public class GFB_GoalDetector : MonoBehaviour
{
	private MeshRenderer blinkingSurface;
	public AudioClip aclip1;
	private AudioSource asource;
	private static HRTManager hrtm;

	void Start ()
	{
		blinkingSurface = this.GetComponent<MeshRenderer> ();
		blinkingSurface.enabled = false;
		asource = this.GetComponent<AudioSource> ();

		///TImer[0] for blinking surface
		hrtm = new HRTManager ();
		hrtm.ResetSamplingFrequency ();
		hrtm.CreateTimers (1);
	}

	void OnTriggerEnter (Collider other)
	{
		asource.PlayOneShot (aclip1);
		blinkingSurface.enabled = true;

		GFB_ItemsSet.nGoalsObtained++;

		///an item can be overtaken by another depending on the velocity output speed when the user hits the item
		///to know wich of the two sides is the original side of the item we use get_InitialPosition_ofThisItem
		if (ITEM_POSITION.LEFT == other.gameObject.GetComponent<GFB_ItemCollision> ().get_InitialPosition_ofThisItem ()) {
			GFB_ItemsSet.nGoalsObtained_L++;
//			Debug.Log("L " + GFB_ItemsSet.nGoalsObtained_L);

			APP_Manager.Instance.insertOrUpdateCurrentItemAttemptInDB (
				other.gameObject.GetComponent<GFB_ItemCollision> ().get_ItemsSetOwner_ID_ofThisItem (),
				GFB_ItemsSet.nGoalsObtained_L,
				GFB_ItemsSet.getItemsSetLastPeriodSecs ());

		} else {
			GFB_ItemsSet.nGoalsObtained_R++;
//			Debug.Log("R " + GFB_ItemsSet.nGoalsObtained_R);

			APP_Manager.Instance.insertOrUpdateCurrentItemAttemptInDB (
				other.gameObject.GetComponent<GFB_ItemCollision> ().get_ItemsSetOwner_ID_ofThisItem (),
				GFB_ItemsSet.nGoalsObtained_R,
				GFB_ItemsSet.getItemsSetLastPeriodSecs ());
		}

		hrtm.Timers [0].InitCounting ();
	}

	/// <summary>
	/// Raises the trigger stay event. Used when the collider isTrigger=true;
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerStay (Collider other)
	{
		hrtm.Timers [0].EndCounting ();
		if (hrtm.Timers [0].GetLastPeriodSecs () > 0.1d) {
			GameObject.Destroy (other.GetComponent<Rigidbody> ());

			if (other.GetComponent<MeshCollider> () != null)
				GameObject.Destroy (other.GetComponent<MeshCollider> ());
			if (other.GetComponent<SphereCollider> () != null)
				GameObject.Destroy (other.GetComponent<SphereCollider> ());
			if (other.GetComponent<CapsuleCollider> () != null)
				GameObject.Destroy (other.GetComponent<CapsuleCollider> ());
			if (other.GetComponent<BoxCollider> () != null)
				GameObject.Destroy (other.GetComponent<BoxCollider> ());
			
			blinkingSurface.enabled = false;
		}
	}

	/// <summary>
	/// Raises the trigger exit event. Used when the collider isTrigger=true;
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerExit (Collider other)
	{
		blinkingSurface.enabled = false;
	}
}
