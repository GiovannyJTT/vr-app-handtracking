﻿/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

/** 
 * Detects pinches and grabs the closest rigidbody if it's within a given range.
 * 
 * Attach this script to the physics hand object assinged to the HandController in a scene.
 */
public class MyMagneticPinch : MonoBehaviour {
	/// <summary>
	/// TFM
	/// </summary>
	private AudioSource asource;
	public AudioClip aclip1;
	public AudioClip aclip2;
	private LineRenderer lr;

  public const float TRIGGER_DISTANCE_RATIO = 0.5f;

  /** The stiffness of the spring force used to move the object toward the hand. */
  private float forceSpringConstant = 150.0f;    //TFM
  /** The maximum range at which an object can be picked up.*/
  private float magnetDistance = 0.25f;    //TFM, 25 cm

  protected bool pinching_;
  protected Collider grabbed_;

  void Start() {
  	///TFM
		lr = this.GetComponent<LineRenderer>();
		lr.SetWidth(0.01f, 0.001f);
		lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		lr.SetPosition(1, new Vector3(0f, 0f, 0f));
		asource = this.GetComponent<AudioSource>();

    pinching_ = false;
    grabbed_ = null;
  }

  private bool isOtherFinger(Collider col){
		if(col.transform.parent != null && col.transform.parent.parent != null){
			return (
				col.transform.parent.name == "MyMagneticPinchHand_Left(Clone)" || ///palm
				col.transform.parent.parent.name == "MyMagneticPinchHand_Left(Clone)" || //thumb
				col.transform.parent.parent.name == "MyMagneticPinchHand_Left(Clone)" || //index
				col.transform.parent.parent.name == "MyMagneticPinchHand_Left(Clone)" || //middle
				col.transform.parent.parent.name == "MyMagneticPinchHand_Left(Clone)" || //ring
				col.transform.parent.parent.name == "MyMagneticPinchHand_Left(Clone)" || //pinky

				col.transform.parent.name == "MyMagneticPinchHand_Right(Clone)" || ///palm
				col.transform.parent.parent.name == "MyMagneticPinchHand_Right(Clone)" || //thumb
				col.transform.parent.parent.name == "MyMagneticPinchHand_Right(Clone)" || //index
				col.transform.parent.parent.name == "MyMagneticPinchHand_Right(Clone)" || //middle
				col.transform.parent.parent.name == "MyMagneticPinchHand_Right(Clone)" || //ring
				col.transform.parent.parent.name == "MyMagneticPinchHand_Right(Clone)"    //pinky
		  	);
		} else {
			return false;
		}
  }

  private bool isGraspableStacked(GameObject obj){
		return obj.GetComponent<GTB_Graspable>() != null &&
			obj.GetComponent<GTB_Graspable>().graspable_state == GTB_Graspable.GTB_GRASPABLEITEM_STATE.STACKED;
  }

  /** Finds an object to grab and grabs it. */
  void OnPinch(Vector3 pinch_position) {
    pinching_ = true;

    // Check if we pinched a movable object and grab the closest one that's not part of the hand.
    Collider[] close_things = Physics.OverlapSphere(pinch_position, magnetDistance);
    Vector3 distance = new Vector3(magnetDistance, 0.0f, 0.0f);

    for (int j = 0; j < close_things.Length; ++j) {
      Vector3 new_distance = pinch_position - close_things[j].transform.position;
      if (close_things[j].GetComponent<Rigidbody>() != null && new_distance.magnitude < distance.magnitude &&
      		///TFM
          !close_things[j].transform.IsChildOf(transform) && !isOtherFinger(close_things[j]) &&
					!isGraspableStacked(close_things[j].gameObject)
					) {
        grabbed_ = close_things[j];
        distance = new_distance;
      }
    }

    ///TFM
    if(!asource.isPlaying){
			asource.volume = 1.2f;
    	asource.PlayOneShot(aclip1);
    }
		if(grabbed_ != null && grabbed_.GetComponent<Rigidbody>() != null && grabbed_.GetComponent<Rigidbody>().isKinematic){
			grabbed_.GetComponent<Rigidbody>().isKinematic = false;
//			Debug.Log(grabbed_.name);
		}
  }

  /** Clears the pinch state. */
  void OnRelease() {
    grabbed_ = null;
    pinching_ = false;

    ///TFM
		lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		lr.SetPosition(1, new Vector3(0f, 0f, 0f));
		asource.Stop();
  }

  /**
   * Checks whether the hand is pinching and updates the position of the pinched object.
   */
  void Update() {
    bool trigger_pinch = false;
    HandModel hand_model = GetComponent<HandModel>();
    Hand leap_hand = hand_model.GetLeapHand();

    if (leap_hand == null)
      return;

    // Scale trigger distance by thumb proximal bone length.
    Vector leap_thumb_tip = leap_hand.Fingers[0].TipPosition;
    float proximal_length = leap_hand.Fingers[0].Bone(Bone.BoneType.TYPE_PROXIMAL).Length;
    float trigger_distance = proximal_length * TRIGGER_DISTANCE_RATIO;

    // Check thumb tip distance to joints on all other fingers.
    // If it's close enough, start pinching.
    for (int i = 1; i < HandModel.NUM_FINGERS && !trigger_pinch; ++i) {
      Finger finger = leap_hand.Fingers[i];

      for (int j = 0; j < FingerModel.NUM_BONES && !trigger_pinch; ++j) {
        Vector leap_joint_position = finger.Bone((Bone.BoneType)j).NextJoint;
        if (leap_joint_position.DistanceTo(leap_thumb_tip) < trigger_distance)
          trigger_pinch = true;
      }
    }

    Vector3 pinch_position = hand_model.fingers[0].GetTipPosition();

    // Only change state if it's different.
    if (trigger_pinch && !pinching_)
      OnPinch(pinch_position);
    else if (!trigger_pinch && pinching_)
      OnRelease();

    // Accelerate what we are grabbing toward the pinch.
    if (grabbed_ != null) {
      Vector3 distance = pinch_position - grabbed_.transform.position;
      grabbed_.GetComponent<Rigidbody>().AddForce(forceSpringConstant * distance);

      ///TFM
			lr.SetPosition(0, pinch_position);
			lr.SetPosition(1, grabbed_.transform.position);
			if(!asource.isPlaying){
				asource.volume = 0.8f;
    		asource.PlayOneShot(aclip2);
    	}
    }
  }
}
