using UnityEngine;
using System.Collections;
using LMWidgets;
using Leap;

public class MyButtonNext : ButtonToggleBase
{

	/// <summary>
	/// TFM - The audio clip to reproduce when the state is on.
	/// </summary>
	public AudioClip aclipOn;

	/// <summary>
	/// TFM - The audio clip to reproduce when the state is off.
	/// </summary>
	public AudioClip aclipOff;

	/// <summary>
	/// TFM - The audio source attached to the button to reproduce the sounds.
	/// </summary>
	private AudioSource asource;

	private HRTManager hrtm;
	public float secondsReset;


	public ButtonDemoGraphics onGraphics;
	public ButtonDemoGraphics offGraphics;
	public ButtonDemoGraphics midGraphics;
	public ButtonDemoGraphics botGraphics;
  
	public Color MidGraphicsOnColor = new Color (0.0f, 0.5f, 0.5f, 1.0f);
	public Color BotGraphicsOnColor = new Color (0.0f, 1.0f, 1.0f, 1.0f);
	public Color MidGraphicsOffColor = new Color (0.0f, 0.5f, 0.5f, 0.1f);
	public Color BotGraphicsOffColor = new Color (0.0f, 0.25f, 0.25f, 1.0f);


	public override void ButtonTurnsOn ()
	{
		TurnsOnGraphics ();
	}

	public override void ButtonTurnsOff ()
	{
		TurnsOffGraphics ();
	}

	private void TurnsOnGraphics ()
	{
		///TFM
		asource.PlayOneShot(aclipOn);
		hrtm.Timers[0].InitCounting();


		onGraphics.SetActive (true);
		offGraphics.SetActive (false);
		midGraphics.SetColor (MidGraphicsOnColor);
		botGraphics.SetColor (BotGraphicsOnColor);
	}

	private void TurnsOffGraphics ()
	{
		///TFM
		asource.PlayOneShot(aclipOff);


		onGraphics.SetActive (false);
		offGraphics.SetActive (true);
		midGraphics.SetColor (MidGraphicsOffColor);
		botGraphics.SetColor (BotGraphicsOffColor);
	}

	private void UpdateGraphics ()
	{
		Vector3 position = transform.localPosition;
		position.z = Mathf.Min (position.z, m_localTriggerDistance);
		onGraphics.transform.localPosition = position;
		offGraphics.transform.localPosition = position;
		Vector3 bot_position = position;
		bot_position.z = Mathf.Max (bot_position.z, m_localTriggerDistance - m_localCushionThickness);
		botGraphics.transform.localPosition = bot_position;
		Vector3 mid_position = position;
		mid_position.z = (position.z + bot_position.z) / 2.0f;
		midGraphics.transform.localPosition = mid_position;
	}

	protected override void Start ()
	{
		///TFM
		asource = this.GetComponent<AudioSource>();
		hrtm = new HRTManager ();
		hrtm.ResetSamplingFrequency ();
		hrtm.CreateTimers (1);
		secondsReset = 1.5f;


		base.Start ();
	}

	protected override void FixedUpdate ()
	{
		///TFM
		if(this.ToggleState){
			hrtm.Timers[0].EndCounting();
			if(hrtm.Timers[0].GetLastPeriodSecs() > secondsReset){    //1.5 second for spring coefficient of 7.5
				this.ToggleState = false;
				ButtonTurnsOff();
			}
		}
		base.FixedUpdate ();
		UpdateGraphics ();
	}
}
