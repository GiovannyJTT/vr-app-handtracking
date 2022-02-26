using UnityEngine;
using System.Collections;
using APPVoices.Types;

namespace APPVoices
{
	namespace Types
	{
		public enum APP_VOICES_MUSEUM
		{
			MUSEUMEXAMPLE = 0,
			ITSYOURTURN = 1,
			HERMES = 2,
			CRANIUM = 3,
			CRATERA = 4,
			OILLAMP = 5,
			PRESSTHEBUTTONTOCONTINUE = 6,
			LOOKATTHEFIGURES = 7
		}

		public enum APP_VOICES_FOOTBALL
		{
			FOOTBALLEXAMPLE = 0,
			ITSYOURNTURN = 1,
			PRESSTHEBUTTON = 2
		}

		public enum APP_VOICES_TOWER
		{
			TOWEREXAMPLE = 0,
			ITSYOURNTURN = 1,
			CENTERIT = 2,
			EVALUATINGTHERESISTANCE = 3,
			RESISTEDYES = 4,
			RESISTEDNO = 5,
			PRESSTHEBUTTON = 6
		}
	}

	public class APP_Voices : MonoBehaviour
	{
		private static APP_Voices _instance;

		public static APP_Voices Instance {
			get {
				if (_instance == null) {
					GameObject go = new GameObject ("APP_Voices_GO");
					GameObject.DontDestroyOnLoad (go);
					go.AddComponent<APP_Voices> ();
				}
				return _instance;
			}
		}

		private AudioSource asource;
		public AudioClip[] museumClips;
		public AudioClip[] footballClips;
		public AudioClip[] towerClips;

		[HideInInspector]
		public bool first_LOOKATTHEFIGURES;
		[HideInInspector]
		public bool first_CENTERIT;

		void Awake ()
		{
			_instance = this;
			asource = this.GetComponent<AudioSource> ();
		}

		void Start ()
		{
			first_LOOKATTHEFIGURES = false;
			first_CENTERIT = false;
		}

		public void playMuseumVoice (int i)
		{
			if (i == (int)APP_VOICES_MUSEUM.PRESSTHEBUTTONTOCONTINUE) {
				asource.Stop ();
			}

			if (!asource.isPlaying) {
				if (i != (int)APP_VOICES_MUSEUM.LOOKATTHEFIGURES) {					
					asource.PlayOneShot (museumClips [i]);
				} else {
					if (first_LOOKATTHEFIGURES) {
						first_LOOKATTHEFIGURES = false;
						asource.PlayOneShot (museumClips [i]);
					}
				}
			}
		}

		public void playFootballVoice (int i)
		{
			if (i == (int)APP_VOICES_FOOTBALL.PRESSTHEBUTTON) {
				asource.Stop ();
			}

			if (!asource.isPlaying) {
				asource.PlayOneShot (footballClips [i]);
			}
		}

		public void playTowerVoice (int i)
		{
			if (i == (int)APP_VOICES_TOWER.PRESSTHEBUTTON) {
				asource.Stop ();
			}

			if (!asource.isPlaying) {
				if (i != (int)APP_VOICES_TOWER.CENTERIT) {
					asource.PlayOneShot (towerClips [i]);
				} else {
					if (first_CENTERIT) {
						first_CENTERIT = false;
						asource.PlayOneShot (towerClips [i]);
					}
				}
			}
		}

		public void stopSource ()
		{
			asource.Stop ();
		}
	}

}
