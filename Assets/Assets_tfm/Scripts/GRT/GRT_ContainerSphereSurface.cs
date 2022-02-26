using UnityEngine;
using System.Collections;

public class GRT_ContainerSphereSurface : MonoBehaviour
{
	public GameObject blinkingSurface;
	public AudioClip aclip1;
	private AudioSource asource;

	void Start ()
	{
		blinkingSurface.SetActive(false);
		asource = this.GetComponent<AudioSource>();
	}

	void OnCollisionEnter(Collision c){
		if(!asource.isPlaying && !blinkingSurface.activeInHierarchy)
			asource.PlayOneShot(aclip1);
	}

	/// <summary>
	/// Raises the collision stay event. Used when the collider isTrigger=false;
	/// </summary>
	/// <param name="c">C.</param>
	void OnCollisionStay(Collision c){
//		foreach(ContactPoint cp in c.contacts){
//			Debug.DrawRay(cp.point, cp.normal, Color.green);
//		}
		blinkingSurface.SetActive(true);
	}

	/// <summary>
	/// Raises the collision exit event. Used when the collider isTrigger=false;
	/// </summary>
	/// <param name="c">C.</param>
	void OnCollisionExit(Collision c){
//		foreach(ContactPoint cp in c.contacts){
//			Debug.DrawRay(cp.point, cp.normal, Color.blue);
//		}
		blinkingSurface.SetActive(false);
	}


//	/// <summary>
//	/// Raises the trigger stay event. Used when the collider isTrigger=true;
//	/// </summary>
//	/// <param name="other">Other.</param>
//		void OnTriggerStay (Collider other)
//	{
//		blinkingSurface.SetActive(true);
//	}
//
//	/// <summary>
//	/// Raises the trigger exit event. Used when the collider isTrigger=true;
//	/// </summary>
//	/// <param name="other">Other.</param>
//	void OnTriggerExit (Collider other)
//	{
//		blinkingSurface.SetActive(false);
//	}
}
