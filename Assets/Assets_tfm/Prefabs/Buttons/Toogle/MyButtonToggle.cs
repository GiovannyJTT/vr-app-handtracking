using UnityEngine;
using System.Collections;
using LMWidgets;
using Leap;

public class MyButtonToggle : ButtonToggleBase
{
	/// <summary>
	/// TFM - Indica si el button3D esta en modo On.
	/// </summary>
	private bool isOn = false;
	public Gesture.GestureType gestuType = Gesture.GestureType.TYPE_INVALID;
	private Controller c;




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
		isOn = true;
		switch (gestuType) {
		case Gesture.GestureType.TYPE_CIRCLE:
			c.EnableGesture (Gesture.GestureType.TYPE_CIRCLE, isOn);
			c.Config.SetFloat ("Gesture.Circle.MinRadius", 20.0f);    //mm
			c.Config.SetFloat ("Gesture.Circle.MinArc", 1.75f * Mathf.PI);    //radians
			break;
		case Gesture.GestureType.TYPE_SWIPE:
			c.EnableGesture (Gesture.GestureType.TYPE_SWIPE, isOn);
			c.Config.SetFloat ("Gesture.Swipe.MinLength", 25.0f);    //mm
			c.Config.SetFloat ("Gesture.Swipe.MinVelocity", 8.0f);    //mm/s
			break;
		case Gesture.GestureType.TYPE_SCREEN_TAP:
			c.EnableGesture (Gesture.GestureType.TYPE_SCREEN_TAP, isOn);
			c.Config.SetFloat ("Gesture.KeyTap.MinDownVelocity", 30.0f);    //mm/s
			c.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", 0.2f);    //s
			c.Config.SetFloat ("Gesture.KeyTap.MinDistance", 1.0f);    //mm
			break;
		case Gesture.GestureType.TYPE_KEY_TAP:
			c.EnableGesture (Gesture.GestureType.TYPE_KEY_TAP, isOn);
			c.Config.SetFloat ("Gesture.ScreenTap.MinForwardVelocity", 25.0f);    //mm/s
			c.Config.SetFloat ("Gesture.ScreenTap.HistorySeconds", 0.5f);    //s
			c.Config.SetFloat ("Gesture.ScreenTap.MinDistance", 1.0f);    //mm 
			break;
		}
		c.Config.Save ();



		onGraphics.SetActive (true);
		offGraphics.SetActive (false);
		midGraphics.SetColor (MidGraphicsOnColor);
		botGraphics.SetColor (BotGraphicsOnColor);
	}

	private void TurnsOffGraphics ()
	{
		///TFM
		isOn = false;
		switch (gestuType) {
		case Gesture.GestureType.TYPE_CIRCLE:
			c.EnableGesture (Gesture.GestureType.TYPE_CIRCLE, isOn);
			break;
		case Gesture.GestureType.TYPE_SWIPE:
			c.EnableGesture (Gesture.GestureType.TYPE_SWIPE, isOn);
			break;
		case Gesture.GestureType.TYPE_SCREEN_TAP:
			c.EnableGesture (Gesture.GestureType.TYPE_SCREEN_TAP, isOn);
			break;
		case Gesture.GestureType.TYPE_KEY_TAP:
			c.EnableGesture (Gesture.GestureType.TYPE_KEY_TAP, isOn);
			break;
		case Gesture.GestureType.TYPE_INVALID:
			break;
		}
		c.Config.Save ();



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
		///TFM - GestureActivation
		c = new Controller ();
		c.EnableGesture (Gesture.GestureType.TYPE_CIRCLE, false);
		c.EnableGesture (Gesture.GestureType.TYPE_SWIPE, false);
		c.EnableGesture (Gesture.GestureType.TYPE_SCREEN_TAP, false);
		c.EnableGesture (Gesture.GestureType.TYPE_KEY_TAP, false);
		c.Config.Save ();



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
