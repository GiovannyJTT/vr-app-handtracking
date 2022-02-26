using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DBTfm;
using DBTfm.Types;
using System;

namespace APPTfm
{


	/// <summary>
	/// App manager. Static ensures that only one instance (singleton) is running. All the scenes that consults properties of the APP_Manager will get the same instance.
	/// </summary>
	public class APP_Manager : MonoBehaviour
	{
		/// <summary>
		/// The instance of the APP_Manager using a singleton to mantain the information while jumping between scenes
		/// </summary>
		private static APP_Manager _instance;

		public static APP_Manager Instance {
			get {
				if (_instance == null) {
					GameObject go = new GameObject ("APP_Manager_GO");
					GameObject.DontDestroyOnLoad (go);
					go.AddComponent<APP_Manager> ();
				}
				return _instance;
			}
		}

		private HCI_TYPE hci_type;

		private Leap.Controller c;

		private static DB_Person currentPerson;
		private static DB_GameSession currentGSession;
		private static DB_Game currentGame;
		private static DB_Item currentItem;
		private static DB_Item_Attempt currentItemAttempt;

		void Awake ()
		{
			_instance = this;

			c = new Leap.Controller ();
			c.SetPolicyFlags (Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
		
			setHCIType (HCI_TYPE.NONE);
		}

		void Start ()
		{
			if (DB_Manager.Instance == null)    ///Comment this line for INSERT new Games and Items in DB.
				DB_Manager.Instance.storeOrUpdateItemsAndGames ();
		}

		public HCI_TYPE getHCIType ()
		{
			return Instance.hci_type;
		}

		public void setHCIType (HCI_TYPE hci)
		{
			Instance.hci_type = hci;
		}

		/// <summary>
		/// Gets the current person.
		/// </summary>
		/// <returns>The current person.</returns>
		public DB_Person getCurrentPerson ()
		{
			return currentPerson;
		}

		public void setCurrentPerson (DB_Person p)
		{
			currentPerson = p;
		}

		public DB_GameSession getCurrentGameSession ()
		{
			return currentGSession;
		}

		public void setCurrentGameSession (DB_GameSession gs)
		{
			currentGSession = gs;
		}

		public void setCurrentGame (DB_Game g)
		{
			currentGame = g;
		}

		public DB_Game getCurrentGame ()
		{
			return currentGame;
		}

		public void setCurrentItem (DB_Item it)
		{
			currentItem = it;
		}

		public DB_Item getCurrentItem ()
		{
			return currentItem;
		}

		public void setCurrentItemAttempt (DB_Item_Attempt ita)
		{
			currentItemAttempt = ita;
		}

		public DB_Item_Attempt getCurrentItemAttempt ()
		{
			return currentItemAttempt;
		}

		/// <summary>
		/// Updates the current game in app. This type of functions are used in both cases: debug session, or real session.
		/// </summary>
		/// <param name="ordinal">Ordinal.</param>
		public void updateCurrentGameInApp (int ordinal)
		{
			if (DB_Manager.Instance != null) {
				DB_Game g = DB_Manager.Instance.getOneGame (ordinal);
				if (g != null) {
					setCurrentGame (g);
				} else {
					Debug.Log ("There is no " + ordinal.ToString () + " game ordinal");
				}
			} else {
				Debug.Log ("There is no DB_Manager.Instance");
			}
		}

		public void updateCurrentItemInApp (int id_item)
		{
			if (DB_Manager.Instance != null) {
				DB_Item it = DB_Manager.Instance.getOneItem (getCurrentGame ().id, id_item);
				if (it != null) {
					setCurrentItem (it);
				} else {
					Debug.Log ("Error: there is no current item.");
				}
			} else {
				Debug.Log ("There is no DB_Manager.Instance");
			}
		}

		public void updateCurrentGSessionInAPP_and_InDB (GAME_SESION_STATE g_session_state, int g_ordinal)
		{
			DB_GameSession gs = getCurrentGameSession ();
			if (gs != null) {
				gs.setCurrentDate ();
				gs.setGSState (g_session_state);
				setCurrentGameSession (gs);
				DB_Manager.Instance.updateGSession (gs);
			} else {
				Debug.Log ("Using Debug GameSession");
				setCurrentGameSession (
					new DB_GameSession (
						-1, -1, g_ordinal, -1, DateTime.Now, HCI_TYPE.LM_AND_NVIDIA, g_session_state, 0.0d, 0
					));
			}
		}

		public void gameCompletedInDB (double seconds, int points_obtained)
		{
			if (DB_Manager.Instance != null) {
				DB_GameSession gs = getCurrentGameSession ();
				if (gs != null) {
					if (gs.session_number != -1) {
						gs.setReachedOK (seconds, points_obtained);
						DB_Manager.Instance.updateGSession (gs);
					} else {
						Debug.Log ("Game Completed. Using Debug GameSession. No update the 'game state'.");
					}
				} else {
					Debug.Log ("There is no getCurrentGameSession");
				}
			} else {
				Debug.Log ("There is no DB_Manager.Instance");
			}
		}

		public void insertOrUpdateCurrentItemAttemptInDB (int id_item, int attempt_number, double seconds)
		{
			if (DB_Manager.Instance != null) {
				DB_GameSession gs = APP_Manager.Instance.getCurrentGameSession ();

				if (gs.session_number != -1) {
					DB_Item_Attempt ita =
						DB_Manager.Instance.getOneItemAttempt (
							APP_Manager.Instance.getCurrentGameSession ().id,
							id_item
						);

					if (ita == null) {
						//insert
//						Debug.Log("Item Insert");
						DB_Manager.Instance.insertItemAttempt (
							new DB_Item_Attempt (
								-1, APP_Manager.Instance.getCurrentGameSession ().id, id_item, seconds, attempt_number
							));
					} else {
						//update
//						Debug.Log("Item Update");
						ita.setRT (seconds);
						ita.setAttemptNumber (attempt_number);
						DB_Manager.Instance.updateItemAttempt (ita);
					}
				} else {
					Debug.Log ("Using Debug items. No update in DB");
					//nothing to do
				}
			} else {
				Debug.Log ("There is no DB_Manager.Instance");
			}
		}


		/// <summary>
		/// Prepares the next G session. The "setCurrentGame"  it is made at the "Start" of each game.
		/// </summary>
		public void prepareNextGSession ()
		{
			DB_GameSession gs = getCurrentGameSession ();

			if (gs.session_number != -1) {
				DB_GameSession gs_next = DB_Manager.Instance.getOneGSession (gs.id_person, gs.id_game + 1, gs.session_number + 1);
				if (gs_next == null) {
					gs_next = new DB_GameSession (
						-1,
						gs.id_person,
						gs.id_game + 1,
						gs.session_number + 1,
						DateTime.Now,
						gs.hci_type,
						GAME_SESION_STATE.TUTORIAL,
						0.0d,
						0
					);
					DB_Manager.Instance.insertGSession (gs_next);
				}
				setCurrentGameSession (DB_Manager.Instance.getOneGSession (gs_next.id_person, gs_next.id_game, gs_next.session_number));
			} else {
				Debug.Log("prepareNextGSession in Debug mode. No update in db.");
			}
		}
	}
}
