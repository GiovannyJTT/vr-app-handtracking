using UnityEngine;
using System.Collections;

public class GTB_OpacityAnimation : MonoBehaviour
{
	private enum GTB_OPACITY_STATE
	{
		INCREASING = 0,
		DEACREASING = 1
	}

	private GTB_OPACITY_STATE opacity_state;

	private MeshRenderer mr;
	private float opacityUpper;
	private float opacityLower;


	void Start ()
	{
		mr = this.GetComponent<MeshRenderer> ();
		opacityUpper = 0.75f;
		opacityLower = 0.0f;

		opacity_state = GTB_OPACITY_STATE.INCREASING;
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (opacity_state) {
		case GTB_OPACITY_STATE.INCREASING:			
			if (mr.material.color.a < opacityUpper) {
				Color c = new Color (mr.material.color.r, mr.material.color.g, mr.material.color.b, mr.material.color.a);
				c.a += 0.75f * Time.deltaTime;
				mr.material.color = c;

				mr.material.SetColor("_EmissionColor", Color.green * c.a * 0.5f);
			} else {
				opacity_state = GTB_OPACITY_STATE.DEACREASING;
			}
			break;
		case GTB_OPACITY_STATE.DEACREASING:
			if (mr.material.color.a > opacityLower) {
				Color c = new Color (mr.material.color.r, mr.material.color.g, mr.material.color.b, mr.material.color.a);
				c.a -= 0.75f * Time.deltaTime;
				mr.material.color = c;

				mr.material.SetColor("_EmissionColor", Color.green * c.a * 0.5f);
			} else {
				opacity_state = GTB_OPACITY_STATE.INCREASING;
			}
			break;
		}
	}
}
