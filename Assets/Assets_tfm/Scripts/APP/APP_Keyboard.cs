using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class APP_Keyboard : MonoBehaviour
{
	private string[] sceneNames;

	void Start ()
	{
		sceneNames = new string[7];
		sceneNames [0] = "tfm_ui";
		sceneNames [1] = "tfm_game_museum_tutorial";
		sceneNames [2] = "tfm_game_museum";
		sceneNames [3] = "tfm_game_football_tutorial";
		sceneNames [4] = "tfm_game_football";
		sceneNames [5] = "tfm_game_tower_tutorial";
		sceneNames [6] = "tfm_game_tower";
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.J)) {
			for (int i = sceneNames.Length - 1; i > 0; i--) {
				if (SceneManager.GetActiveScene ().name == sceneNames [i]) {
//					destroySceneHRTimers ();
					SceneManager.LoadScene (sceneNames [i - 1]);
					break;
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.K)) {
			for (int i = 0; i < sceneNames.Length - 1; i++) {
				if (SceneManager.GetActiveScene ().name == sceneNames [i]) {
//					destroySceneHRTimers ();
					SceneManager.LoadScene (sceneNames [i + 1]);
					break;
				}
			}
		}
	}
}
