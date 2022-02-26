using UnityEngine;
using System.Collections;

public class GTB_BombTimerSound  {
	private AudioSource asource;
	private AudioClip aclip;
	private int lastSeconds;
	private int lastMilliseconds;
	private double currentTime;

	public GTB_BombTimerSound (AudioSource asource, AudioClip aclip) {
		this.asource = asource;
		this.aclip = aclip;
		lastSeconds = 0;
		lastMilliseconds = 0;
	}

	public void secondSound (double seconds) {
		if((int)seconds > lastSeconds){
			lastSeconds = (int)seconds;
			if(!asource.isPlaying){
				asource.PlayOneShot(aclip);
			}
		}
	}

	public void halfSecondSound(double milliseconds) {
		if((int)milliseconds > lastMilliseconds + 500){
			lastMilliseconds = (int)milliseconds;
			if(!asource.isPlaying){
				asource.PlayOneShot(aclip);
			}
		}
	}

	public void quarterSecondSound(double milliseconds) {
		if((int)milliseconds > lastMilliseconds + 250){
			lastMilliseconds = (int)milliseconds;
			if(!asource.isPlaying){
				asource.PlayOneShot(aclip);
			}
		}
	}

}
