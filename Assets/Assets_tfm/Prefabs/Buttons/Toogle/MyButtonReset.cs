using UnityEngine;
using System.Collections;
using LMWidgets;
using Leap;

public class MyButtonReset : ButtonToggleBase
{
	/// <summary>
	/// TFM - Indicates if the 3d button is in ON mode or not.
	/// </summary>
	private bool isOn = false;

	/// <summary>
	/// The object to reset.
	/// </summary>
	public GameObject objectToReset;

	/// <summary>
	/// The position original of the object to reset.
	/// </summary>
	private Vector3 posOrig;

	/// <summary>
	/// The rotation original of the object to reset.
	/// </summary>
	private Quaternion rotOrig;

	/// <summary>
	/// The audio clip to reproduce when the state is on.
	/// </summary>
	public AudioClip aclipOn;

	/// <summary>
	/// The audio clip to reproduce when the state is off.
	/// </summary>
	public AudioClip aclipOff;

	/// <summary>
	/// The audio source attached to the button to reproduce the sounds.
	/// </summary>
	private AudioSource asource;

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
		objectToReset.GetComponent<Rigidbody>().isKinematic = true;
		objectToReset.transform.position = this.posOrig;
		objectToReset.transform.rotation = this.rotOrig;
		asource.PlayOneShot(aclipOn);


		onGraphics.SetActive (true);
		offGraphics.SetActive (false);
		midGraphics.SetColor (MidGraphicsOnColor);
		botGraphics.SetColor (BotGraphicsOnColor);
	}

	private void TurnsOffGraphics ()
	{
		///TFM
		objectToReset.GetComponent<Rigidbody>().isKinematic = false;
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
		this.posOrig = objectToReset.transform.position;
		this.rotOrig = objectToReset.transform.rotation;
		asource = this.GetComponent<AudioSource>();



		base.Start ();
	}

	protected override void FixedUpdate ()
	{
		base.FixedUpdate ();
		UpdateGraphics ();
	}

	/// <summary>
	/// TFM - Gets the is on.
	/// </summary>
	public bool getIsOn(){
		return this.isOn;
	}
}
