using UnityEngine;
using System.Collections;

/// <summary>
/// TFM
/// </summary>
public class CollisionSound : MonoBehaviour {
	public AudioClip aclip1;    //low freqs
	public AudioClip aclip2;    //high frqs

	private AudioSource asource;
	private float lowPitchRange = 1.0f;
	private float highPitchRange = 1.5f;
	private float velToVol = 0.2f;

	/// <summary>
	/// The velocity clip split. !.5f worsk very well for a mass of 1 kg, 0 Drag, and 0.05 angular drag drag 
	/// </summary>
	private float velocityClipSplit = 1.5f;

	void Awake(){
		asource = this.GetComponent<AudioSource>();
	}

	void OnCollisionEnter (Collision col){
		asource.pitch = Random.Range(lowPitchRange, highPitchRange);
		float hitVol = col.relativeVelocity.magnitude * velToVol;
		if (col.relativeVelocity.magnitude < velocityClipSplit)
			asource.PlayOneShot(aclip1, hitVol);
		else
			asource.PlayOneShot(aclip2, hitVol);
		asource.Play();
	}
}
