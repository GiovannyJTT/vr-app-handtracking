using UnityEngine;
using System.Collections;
using APPTfm;
using DBTfm;
using DBTfm.Types;

public class APP_InstructionsController : MonoBehaviour {
	public GameObject instOculus;
	public GameObject instNvidia;

	void Start () {
		DB_GameSession gs = APP_Manager.Instance.getCurrentGameSession();
		if(gs != null)
			toggleInstructions(gs.hci_type);
		else
			toggleInstructions(HCI_TYPE.LM_AND_NVIDIA);
	}

	public void toggleInstructions(HCI_TYPE hci){
		if(hci == HCI_TYPE.LM_AND_NVIDIA){
			instOculus.SetActive(false);
			instNvidia.SetActive(true);
		} else{
			instOculus.SetActive(true);
			instNvidia.SetActive(false);
		}
	}
}
