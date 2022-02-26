using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Needed librarys to use sqlite engine. The sqlite.dll must and others must be dragged to the Plugins folder.
/// </summary>
using Mono.Data.Sqlite;
using System.Data;
using System;

using DBTfm.Types;

namespace DBTfm
{
	namespace Types
	{
		public enum HCI_TYPE
		{
			LM_AND_OCULUS = 0,
			LM_AND_NVIDIA = 1,
			NONE = 2
		}

		public enum SEX
		{
			MALE = 0,
			FEMALE = 1,
			EMPTY = 2
		}

		public enum GAME_SESION_STATE
		{
			TUTORIAL = 0,
			PLAYING = 1,
			COMPLETED = 2,
			ABORTED = 3
		}

		public enum GAME_TYPE
		{
			GAME_TUTORIAL = 0,
			GAME_RT = 1,
			GAME_GOALS = 2,
			GAME_GESTURES = 3
		}

		public enum GAME_ORDINAL
		{
			NONE = 0,
			FIRST = 1,
			SECOND = 2,
			THIRD = 3
		}

		public enum ITEM_TYPE
		{
			TURNABLE = 0,
			//girable
			HITTABLE = 1,
			//golpeable
			GRASPABLE = 2,
			//aprehensible
			LAUNCHABLE = 3
			//lanzable
		}

		public enum ITEM_POSITION
		{
			NEAR = 0,
			FAR = 1,
			LEFT = 2,
			RIGHT = 3,
			NONE = 4
		}

		public enum IDS_GAMES
		{
			ID_GMUSEUM = 1,
			ID_GFOOTBALL = 2,
			ID_GCLOCK = 3
		}

		public enum IDS_ITEMS
		{
			ID_HERMES = 1,
			ID_CRANIUM = 2,
			ID_CRATERA = 3,
			ID_OILLAMP = 4,

			ID_BALLS_SET_L = 5,
			ID_BALLS_SET_R = 6,
			ID_CAPSULES_SET_L = 7,
			ID_CAPSULES_SET_R = 8,
			ID_ICOSAHEDRONS_SET_L = 9,
			ID_ICOSAHEDRONS_SET_R = 10,
			ID_PRISMS_SET_L = 11,
			ID_PRISMS_SET_R = 12,
			ID_CUBES_SET_L = 13,
			ID_CUBES_SET_R = 14,
			ID_PYRAMIDS_SET_L = 15,
			ID_PYRAMIDS_SET_R = 16,

			ID_GRASPABLES_SET_NEAR = 17,
			ID_GRASPABLES_SET_FAR = 18
		}
	}

	public class DB_Manager : MonoBehaviour
	{
		private static DB_Manager _instance;

		public static DB_Manager Instance {
			get {
				if (_instance == null) {
					GameObject go = new GameObject ("DB_Manager_GO");
					GameObject.DontDestroyOnLoad (go);
					go.AddComponent<DB_Manager> ();
				}
				return _instance;
			}
		}

        /// <summary>
        /// Used for queue itemAttempts while "Playing" (in order to avoid the drivedisk access time), and insert into db when game "completed".
        /// </summary>
		private static List<DB_Item_Attempt> itemAttempts_insert;
        private static List<DB_Item_Attempt> itemAttempts_update;

		/// <summary>
		/// The local directory of the database. If is not correct, then sqlite will creates a new database.
		/// </summary>
		private static string dbPath = "";
		private static string dbUriFile = "";

		void Awake ()
		{
            checkDBUriFile();
            itemAttempts_insert = new List<DB_Item_Attempt>();
            itemAttempts_update = new List<DB_Item_Attempt>();
			_instance = this;	
		}

        public static string getDBURI(){
            return "URI=file:" + dbPath;
        }

		/// <summary>
		/// Checks the DB URI file. Need to be called on UI awake.
		/// </summary>
		/// <returns><c>true</c>, if DB URI file was checked, <c>false</c> otherwise.</returns>
		public bool checkDBUriFile ()
		{
//			dbPath = Application.dataPath + "/Resources/tfm_db.db";
			dbPath = "D:/tfm_db.db";
			return (File.Exists (dbPath));
		}


		/// <summary>
		/// Updates the persons list.
		/// </summary>
        /// 
		public DB_Person getOnePerson (int code)
		{
			dbUriFile = "URI=file:" + dbPath;
			DB_Person p = null;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"SELECT * FROM Person WHERE code = @code";
						cmd.Parameters.Add (new SqliteParameter ("@code", code));
						IDataReader r = cmd.ExecuteReader ();
						if (r.Read ()) {
							p =	new DB_Person (
								r.GetInt32 (0),    ///id
								r.GetInt32 (1),    ///code
								r.GetString (2),    ///name
								r.GetInt32 (3),    ///age
								(SEX)Enum.Parse (typeof(SEX), r.GetInt32 (4).ToString ())    ///sex
							);
						}
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
			return p;
		}

        /*
		public List<DB_Person> getAllPersons ()
		{
			dbUriFile = "URI=file:" + dbPath;
			List<DB_Person> ps = new List<DB_Person> ();
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = "SELECT * FROM Person WHERE 1";
						IDataReader r = cmd.ExecuteReader ();
						while (r.Read ()) {    //iterate rows
							DB_Person p =
								new DB_Person (
									r.GetInt32 (0),    ///id
									r.GetInt32 (1),    ///code
									r.GetString (2),    ///name
									r.GetInt32 (3),    ///age
									(SEX)Enum.Parse (typeof(SEX), r.GetInt32 (4).ToString ())    ///sex
								);
							ps.Add (p);
						}
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
			return ps;
		}
        */

		public void insertPerson (DB_Person p)
		{
			dbUriFile = "URI=file:" + dbPath;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						//cmd.CommandText = "INSERT INTO Person (id, code, name, age, sex) VALUES (NULL, " + p.codeStr + ", '" + p.name + "', " + p.ageStr + ", " + p.sexStr + " )";
						cmd.CommandText = @"INSERT INTO Person (id, code, name, age, sex) VALUES (@id, @c, @n, @a, @s)";
						cmd.Parameters.Add (new SqliteParameter ("@id", null));
						cmd.Parameters.Add (new SqliteParameter ("@c", p.code));
						cmd.Parameters.Add (new SqliteParameter ("@n", p.name));
						cmd.Parameters.Add (new SqliteParameter ("@a", p.age));
						cmd.Parameters.Add (new SqliteParameter ("@s", p.sex));
						int nRows = cmd.ExecuteNonQuery ();
						if (nRows != 1)
							Debug.Log ("Error: no row affected in Person insertion. Affected: " + nRows.ToString ());
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
		}

		public void updatePerson (DB_Person p)
		{
			dbUriFile = "URI=file:" + dbPath;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"UPDATE Person SET name = @n, age = @a, sex = @s WHERE code = @c";
						cmd.Parameters.Add (new SqliteParameter ("@c", p.code));
						cmd.Parameters.Add (new SqliteParameter ("@n", p.name));
						cmd.Parameters.Add (new SqliteParameter ("@a", p.age));
						cmd.Parameters.Add (new SqliteParameter ("@s", p.sex));
						int nRows = cmd.ExecuteNonQuery ();
						if (nRows != 1)
							Debug.Log ("Error: no row affected in Person update. Affected: " + nRows.ToString ());
						cmd.Dispose ();
					} 
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
		}

		/// <summary>
		/// Gets an specific gamesession.
		/// </summary>
        /// 
		public DB_GameSession getOneGSession (int id_person, int id_game, int session_number)
		{
			dbUriFile = "URI=file:" + dbPath;
			DB_GameSession gs = null;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"SELECT * FROM GameSession WHERE id_person = @id_p AND id_game = @id_g AND session_number = @sn";
						cmd.Parameters.Add (new SqliteParameter ("@id_p", id_person));
						cmd.Parameters.Add (new SqliteParameter ("@id_g", id_game));
						cmd.Parameters.Add (new SqliteParameter ("@sn", session_number));
						IDataReader r = cmd.ExecuteReader ();
						if (r.Read ()) {
							gs =	new DB_GameSession (
								r.GetInt32 (0),    ///id
								r.GetInt32 (1),    ///id_person
								r.GetInt32 (2),    ///id_game
								r.GetInt32 (3),    ///session_number
								r.GetDateTime (4),    ///date
								(HCI_TYPE)Enum.Parse (typeof(HCI_TYPE), r.GetInt32 (5).ToString ()),    ///hci_type
								(GAME_SESION_STATE)Enum.Parse (typeof(GAME_SESION_STATE), r.GetInt32 (6).ToString ()),    ///state
								r.GetDouble (7),    ///goal_reached_seconds
								r.GetInt32 (8)    ///points_obtained
							);
						}
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
			return gs;
		}

        /*
		public List<DB_GameSession> getAllGSessions (int id_person)
		{
			dbUriFile = "URI=file:" + dbPath;
			List<DB_GameSession> gss = new List<DB_GameSession> ();
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"SELECT * FROM GameSession WHERE id_person = @id_p";
						cmd.Parameters.Add (new SqliteParameter ("@id_p", id_person));
						IDataReader r = cmd.ExecuteReader ();
						while (r.Read ()) {    //iterate rows
							DB_GameSession gs =
								new DB_GameSession (
									r.GetInt32 (0),    ///id
									r.GetInt32 (1),    ///id_person
									r.GetInt32 (2),    ///id_game
									r.GetInt32 (3),    ///session_number
									r.GetDateTime (4),    ///date
									(HCI_TYPE)Enum.Parse (typeof(HCI_TYPE), r.GetInt32 (5).ToString ()),    ///hci_type
									(GAME_SESION_STATE)Enum.Parse (typeof(GAME_SESION_STATE), r.GetInt32 (6).ToString ()),    ///game_state
									r.GetDouble (7),    ///goal_reached_seconds
									r.GetInt32 (8)    ///points_obtained
								);
							gss.Add (gs);
						}
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
			return gss;
		}
        */

		public void insertGSession (DB_GameSession gs)
		{
			dbUriFile = "URI=file:" + dbPath;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"INSERT INTO GameSession (id, id_person, id_game, session_number, date, hci_type, state, goal_reached_seconds, points_obtained) VALUES (@id, @id_p, @id_g, @sn, @d, @h, @s, @grs, @p_o)";
						cmd.Parameters.Add (new SqliteParameter ("@id", null));
						cmd.Parameters.Add (new SqliteParameter ("@id_p", gs.id_person));
						cmd.Parameters.Add (new SqliteParameter ("@id_g", gs.id_game));
						cmd.Parameters.Add (new SqliteParameter ("@sn", gs.session_number));
						cmd.Parameters.Add (new SqliteParameter ("@d", gs.date));
						cmd.Parameters.Add (new SqliteParameter ("@h", gs.hci_type));
						cmd.Parameters.Add (new SqliteParameter ("@s", gs.state));
						cmd.Parameters.Add (new SqliteParameter ("@grs", gs.goal_reached_seconds));
						cmd.Parameters.Add (new SqliteParameter ("@p_o", gs.points_obtained));
						int nRows = cmd.ExecuteNonQuery ();
						if (nRows != 1)
							Debug.Log ("Error: no row affected in GameSession insertion. Affected: " + nRows.ToString ());
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
		}

		public void updateGSession (DB_GameSession gs)
		{
			dbUriFile = "URI=file:" + dbPath;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"UPDATE GameSession SET date = @d, hci_type = @h, state = @s, goal_reached_seconds = @grs, points_obtained = @p_o WHERE id_person = @id_p AND id_game = @id_g AND session_number = @s_n";
						cmd.Parameters.Add (new SqliteParameter ("@id_p", gs.id_person));
						cmd.Parameters.Add (new SqliteParameter ("@id_g", gs.id_game));
						cmd.Parameters.Add (new SqliteParameter ("@s_n", gs.session_number));
						cmd.Parameters.Add (new SqliteParameter ("@d", gs.date));
						cmd.Parameters.Add (new SqliteParameter ("@h", gs.hci_type));
						cmd.Parameters.Add (new SqliteParameter ("@s", gs.state));
						cmd.Parameters.Add (new SqliteParameter ("@grs", gs.goal_reached_seconds));
						cmd.Parameters.Add (new SqliteParameter ("@p_o", gs.points_obtained));
						int nRows = cmd.ExecuteNonQuery ();
						if (nRows != 1)
							Debug.Log ("Error: no row affected in GameSession update. Affected: " + nRows.ToString ());
						cmd.Dispose ();
					} 
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
		}

		/// <summary>
		/// Gets an specific game.
		/// </summary>
        /// 
		public DB_Game getOneGame (int ordinal)
		{
			dbUriFile = "URI=file:" + dbPath;
			DB_Game g = null;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"SELECT * FROM Game WHERE ordinal = @o";
						cmd.Parameters.Add (new SqliteParameter ("@o", ordinal));
						IDataReader r = cmd.ExecuteReader ();
						if (r.Read ()) {
							g =	new DB_Game (
								r.GetInt32 (0),    ///id
								r.GetInt32 (1),    ///ordinal
								(GAME_TYPE)Enum.Parse (typeof(GAME_TYPE), r.GetInt32 (2).ToString ()),    ///game_type
								r.GetDouble (3),    ///limit_seconds
								r.GetString (4)    ///goal_description
							);
						}
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
			return g;
		}

        /*
		public List<DB_Game> getAllGames ()
		{
			dbUriFile = "URI=file:" + dbPath;
			List<DB_Game> gs = new List<DB_Game> ();
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = "SELECT * FROM Game WHERE 1";
						IDataReader r = cmd.ExecuteReader ();
						while (r.Read ()) {    //iterate rows
							DB_Game g =
								new DB_Game (
									r.GetInt32 (0),    ///id
									r.GetInt32 (1),    ///ordinal
									(GAME_TYPE)Enum.Parse (typeof(GAME_TYPE), r.GetInt32 (2).ToString ()),    ///game_type
									r.GetDouble (3),    ///limit_seconds
									r.GetString (4)    ///goal_description
								);
							gs.Add (g);
						}
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
			return gs;
		}
        */

		public void insertGame (DB_Game g)
		{
			dbUriFile = "URI=file:" + dbPath;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"INSERT INTO Game (id, ordinal, game_type, limit_seconds, goal_description) VALUES (@id, @o, @gt, @ls, @gd)";
						cmd.Parameters.Add (new SqliteParameter ("@id", null));
						cmd.Parameters.Add (new SqliteParameter ("@o", g.ordinal));
						cmd.Parameters.Add (new SqliteParameter ("@gt", g.game_type));
						cmd.Parameters.Add (new SqliteParameter ("@ls", g.limit_seconds));
						cmd.Parameters.Add (new SqliteParameter ("@gd", g.goal_description));

						int nRows = cmd.ExecuteNonQuery ();
						if (nRows != 1)
							Debug.Log ("Error: no row affected in Game insertion. Affected: " + nRows.ToString ());
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
		}

		public void updateGame (DB_Game g)
		{
			dbUriFile = "URI=file:" + dbPath;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					///Consider each game is unique
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"UPDATE Game SET ordinal = @o, game_type = @gt, limit_seconds = @ls, goal_description = @gd WHERE id = @id";
						cmd.Parameters.Add (new SqliteParameter ("@id", g.id));
						cmd.Parameters.Add (new SqliteParameter ("@o", g.ordinal));
						cmd.Parameters.Add (new SqliteParameter ("@gt", g.game_type));
						cmd.Parameters.Add (new SqliteParameter ("@ls", g.limit_seconds));
						cmd.Parameters.Add (new SqliteParameter ("@gd", g.goal_description));
						int nRows = cmd.ExecuteNonQuery ();
						if (nRows != 1)
							Debug.Log ("Error: no row affected in Game update.  Affected: " + nRows.ToString ());
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
		}

		/// <summary>
		/// Gets an specific item from a game.
		/// </summary>
        /// 
		public DB_Item getOneItem (int id_game, int id_item)
		{
			dbUriFile = "URI=file:" + dbPath;
			DB_Item it = null;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"SELECT * FROM Item WHERE id = @id AND id_game = @id_g";
						cmd.Parameters.Add (new SqliteParameter ("@id", id_item));
						cmd.Parameters.Add (new SqliteParameter ("@id_g", id_game));
						IDataReader r = cmd.ExecuteReader ();
						if (r.Read ()) {
							it = new DB_Item (
								r.GetInt32 (0),    ///id
								r.GetInt32 (1),    ///id_game
								r.GetString (2),    ///name
								(ITEM_TYPE)Enum.Parse (typeof(ITEM_TYPE), r.GetInt32 (3).ToString ()),    ///item_type
								(ITEM_POSITION)Enum.Parse (typeof(ITEM_POSITION), r.GetInt32 (4).ToString ())    ///item_position
							);
						}
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
			return it;
		}

        /*
		public List<DB_Item> getAllItems (int id_game)
		{
			dbUriFile = "URI=file:" + dbPath;
			List<DB_Item> its = new List<DB_Item> ();
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"SELECT * FROM Item WHERE id_game = @id_g";
						cmd.Parameters.Add (new SqliteParameter ("@id_g", id_game));
						IDataReader r = cmd.ExecuteReader ();
						while (r.Read ()) {    //iterate rows
							DB_Item it =
								new DB_Item (
									r.GetInt32 (0),    ///id
									r.GetInt32 (1),    ///id_game
									r.GetString (2),    ///name
									(ITEM_TYPE)Enum.Parse (typeof(ITEM_TYPE), r.GetInt32 (3).ToString ()),    ///item_type
									(ITEM_POSITION)Enum.Parse (typeof(ITEM_POSITION), r.GetInt32 (4).ToString ())    ///item_position
								);
							its.Add (it);
						}
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
			return its;
		}
        */

		public void insertItem (DB_Item it)
		{
			dbUriFile = "URI=file:" + dbPath;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"INSERT INTO Item (id, id_game, name, item_type, item_position) VALUES (@id, @id_g, @n, @i_t, @i_p)";
						cmd.Parameters.Add (new SqliteParameter ("@id", null));
						cmd.Parameters.Add (new SqliteParameter ("@id_g", it.id_game));
						cmd.Parameters.Add (new SqliteParameter ("@n", it.name));
						cmd.Parameters.Add (new SqliteParameter ("@i_t", it.item_type));
						cmd.Parameters.Add (new SqliteParameter ("@i_p", it.item_position));

						int nRows = cmd.ExecuteNonQuery ();
						if (nRows != 1)
							Debug.Log ("Error: no row affected in Item insertion. Affected: " + nRows.ToString ());
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
		}

		public void updateItem (DB_Item it)
		{
			dbUriFile = "URI=file:" + dbPath;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					/// Consider each item is unique
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"UPDATE Item SET id_game = @id_g, name = @n, item_type = @i_t, item_position = @i_p WHERE id = @id";
						cmd.Parameters.Add (new SqliteParameter ("@id", it.id));
						cmd.Parameters.Add (new SqliteParameter ("@id_g", it.id_game));
						cmd.Parameters.Add (new SqliteParameter ("@n", it.name));
						cmd.Parameters.Add (new SqliteParameter ("@i_t", it.item_type));
						cmd.Parameters.Add (new SqliteParameter ("@i_p", it.item_position));
						int nRows = cmd.ExecuteNonQuery ();
						if (nRows != 1)
							Debug.Log ("Error: no row affected in Item update. Affected: " + nRows.ToString ());
						cmd.Dispose ();
					} 
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
		}

		/// <summary>
		/// Gets an specific item attempt.
		/// </summary>
        ///
		public DB_Item_Attempt getOneItemAttempt (int id_game_session, int id_item)
		{
			dbUriFile = "URI=file:" + dbPath;
			DB_Item_Attempt ita = null;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"SELECT * FROM ItemAttempt WHERE id_game_session = @id_gs AND id_item = @id_i";
						cmd.Parameters.Add (new SqliteParameter ("@id_gs", id_game_session));
						cmd.Parameters.Add (new SqliteParameter ("@id_i", id_item));
						IDataReader r = cmd.ExecuteReader ();
						if (r.Read ()) {
							ita = new DB_Item_Attempt (
								r.GetInt32 (0),    ///id
								r.GetInt32 (1),    ///id_game_session
								r.GetInt32 (2),    ///id_item
								r.GetDouble (3),    ///rt
								r.GetInt32 (4)    ///attempt_number
							);
						}
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
			return ita;
		}

        /*
		public List<DB_Item_Attempt> getAllItemAttempts (int id_game_session, int id_item)
		{
			dbUriFile = "URI=file:" + dbPath;
			List<DB_Item_Attempt> itas = new List<DB_Item_Attempt> ();
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"SELECT * FROM ItemAttempt WHERE id_game_session = @id_gs AND id_item = @id_i";
						cmd.Parameters.Add (new SqliteParameter ("@id_gs", id_game_session));
						cmd.Parameters.Add (new SqliteParameter ("@id_i", id_item));
						IDataReader r = cmd.ExecuteReader ();
						while (r.Read ()) {    //iterate rows
							DB_Item_Attempt ita =
								new DB_Item_Attempt (
									r.GetInt32 (0),    ///id
									r.GetInt32 (1),    ///id_game_session
									r.GetInt32 (2),    ///id_item
									r.GetDouble (3),    ///rt
									r.GetInt32 (4)    ///attempt_number
								);
							itas.Add (ita);
						}
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
			return itas;
		}
        */

        /// <summary>
        /// Queues the itemAttemp in order to be stored in db when the game is completed.
        /// </summary>
        /// <param name="ita"></param>
        public void insertItemAttempt(DB_Item_Attempt ita)
        {
            itemAttempts_insert.Add(ita);
        }

        public void insertItemAttemptImmediately (DB_Item_Attempt ita){
            dbUriFile = "URI=file:" + dbPath;
            using (IDbConnection conn = (IDbConnection)new SqliteConnection(dbUriFile))
            {
                conn.Open();
                try
                {
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO ItemAttempt (id, id_game_session, id_item, rt, attempt_number) VALUES (@id, @id_gs, @id_i, @rt, @a_n)";
                        cmd.Parameters.Add(new SqliteParameter("@id", null));
                        cmd.Parameters.Add(new SqliteParameter("@id_gs", ita.id_game_session));
                        cmd.Parameters.Add(new SqliteParameter("@id_i", ita.id_item));
                        cmd.Parameters.Add(new SqliteParameter("@rt", ita.rt));
                        cmd.Parameters.Add(new SqliteParameter("@a_n", ita.attemp_number));
                        int nRows = cmd.ExecuteNonQuery();
                        if (nRows != 1)
                            Debug.Log("Error: no row affected in ItemAttempt insertion. Affected: " + nRows.ToString());
                        cmd.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Queues the itemAttemp in order to be stored in db when the game is completed.
        /// </summary>
        /// <param name="ita"></param>
        public void updateItemAttempt(DB_Item_Attempt ita)
        {
            itemAttempts_update.Add(ita);
        }

        public void updateItemAttemptImmediately (DB_Item_Attempt ita)
		{
			dbUriFile = "URI=file:" + dbPath;
			using (IDbConnection conn = (IDbConnection)new SqliteConnection (dbUriFile)) {
				conn.Open ();
				try {
					using (IDbCommand cmd = conn.CreateCommand ()) {
						cmd.CommandText = @"UPDATE ItemAttempt SET rt = @rt, attempt_number = @a_n WHERE id_game_session = @id_gs AND id_item = @id_i";
						cmd.Parameters.Add (new SqliteParameter ("@id_gs", ita.id_game_session));
						cmd.Parameters.Add (new SqliteParameter ("@id_i", ita.id_item));
						cmd.Parameters.Add (new SqliteParameter ("@rt", ita.rt));
						cmd.Parameters.Add (new SqliteParameter ("@a_n", ita.attemp_number));
						int nRows = cmd.ExecuteNonQuery ();
						if (nRows != 1)
							Debug.Log ("Error: no row affected in ItemAttempt update. Affected: " + nRows.ToString ());
						cmd.Dispose ();
					}
				} catch (Exception e) {
					Debug.Log (e.Message);
				}
				conn.Close ();
			}
		}

        /// <summary>
        /// Stores itemAttempts in DB.
        /// </summary>
        public void dumpItemAttempts_ToDB()
        {
            //update
            for (int i = 0; i < itemAttempts_update.Count; i++)
            {
                updateItemAttemptImmediately(itemAttempts_update[i]);
            }
            itemAttempts_update.Clear();

            //insert
            for (int i = 0; i < itemAttempts_insert.Count; i++)
            {
                insertItemAttemptImmediately(itemAttempts_insert[i]);
            }
            itemAttempts_insert.Clear();
        }


        /*
         * A partir de aqui: almacenamiento inicial cuando la bd esta totalmente vacia
         */

		/// <summary>
		/// Creates or Updates the DB games and Items.
		/// </summary>
		/// <returns>The DB games.</returns>
		private List<DB_Game> createDBGames ()
		{
			List<DB_Game> games = new List<DB_Game> ();
			games.Add (
				new DB_Game ((int)IDS_GAMES.ID_GMUSEUM, (int)GAME_ORDINAL.FIRST, GAME_TYPE.GAME_RT, 0.0f,
					"Museo: Colocar los modelos del museo en su orientacion correcta usando las manos para girar la esfera contenedora.")
			);
			games.Add (
				new DB_Game ((int)IDS_GAMES.ID_GFOOTBALL, (int)GAME_ORDINAL.SECOND, GAME_TYPE.GAME_GOALS, 15.0f,
					"Futbol: Golpear los objetos y meter en portería el mayor numero de goles antes de que se termine el tiempo.")
			);
			games.Add (
				new DB_Game ((int)IDS_GAMES.ID_GCLOCK, (int)GAME_ORDINAL.THIRD, GAME_TYPE.GAME_GESTURES, 120.0f,
					"Torre: Pellizcar los objetos, arrastrarlos y formar una torre hasta la altura indicada antes de que termine el tiempo.")
			);

			return games;
		}

		private void storeOrUpdateDBGames (List<DB_Game> games)
		{
			for (int i = 0; i < games.Count; i++) {
				DB_Game g = DB_Manager.Instance.getOneGame (games [i].ordinal);
				if (g == null) {
					DB_Manager.Instance.insertGame (games [i]);
				} else {
					games [i].id = g.id;
					DB_Manager.Instance.updateGame (games [i]);
				}
			}
		}

		private List<DB_Item> createDBItems ()
		{
			List<DB_Item> items = new List<DB_Item> ();

			//MUSEUM GAME
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_HERMES, (int)GAME_ORDINAL.FIRST, "Hermes_N", ITEM_TYPE.TURNABLE, ITEM_POSITION.NEAR));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_CRANIUM, (int)GAME_ORDINAL.FIRST, "Craneo_N", ITEM_TYPE.TURNABLE, ITEM_POSITION.NEAR));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_CRATERA, (int)GAME_ORDINAL.FIRST, "Cratera_N", ITEM_TYPE.TURNABLE, ITEM_POSITION.NEAR));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_OILLAMP, (int)GAME_ORDINAL.FIRST, "Candil_N", ITEM_TYPE.TURNABLE, ITEM_POSITION.NEAR));

			//FOOTBALL GAME
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_BALLS_SET_L, (int)GAME_ORDINAL.SECOND, "Balls_Set_L", ITEM_TYPE.HITTABLE, ITEM_POSITION.LEFT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_BALLS_SET_R, (int)GAME_ORDINAL.SECOND, "Balls_Set_R", ITEM_TYPE.HITTABLE, ITEM_POSITION.RIGHT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_CAPSULES_SET_L, (int)GAME_ORDINAL.SECOND, "Capsules_Set_L", ITEM_TYPE.HITTABLE, ITEM_POSITION.LEFT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_CAPSULES_SET_R, (int)GAME_ORDINAL.SECOND, "Capsules_Set_R", ITEM_TYPE.HITTABLE, ITEM_POSITION.RIGHT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_ICOSAHEDRONS_SET_L, (int)GAME_ORDINAL.SECOND, "Icosahedrons_Set_L", ITEM_TYPE.HITTABLE, ITEM_POSITION.LEFT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_ICOSAHEDRONS_SET_R, (int)GAME_ORDINAL.SECOND, "Icosahedrons_Set_R", ITEM_TYPE.HITTABLE, ITEM_POSITION.RIGHT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_PRISMS_SET_L, (int)GAME_ORDINAL.SECOND, "Prisms_Set_L", ITEM_TYPE.HITTABLE, ITEM_POSITION.LEFT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_PRISMS_SET_R, (int)GAME_ORDINAL.SECOND, "Prisms_Set_R", ITEM_TYPE.HITTABLE, ITEM_POSITION.RIGHT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_CUBES_SET_L, (int)GAME_ORDINAL.SECOND, "Cubes_Set_L", ITEM_TYPE.HITTABLE, ITEM_POSITION.LEFT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_CUBES_SET_R, (int)GAME_ORDINAL.SECOND, "Cubes_Set_R", ITEM_TYPE.HITTABLE, ITEM_POSITION.RIGHT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_PYRAMIDS_SET_L, (int)GAME_ORDINAL.SECOND, "Pyramids_Set_L", ITEM_TYPE.HITTABLE, ITEM_POSITION.LEFT));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_PYRAMIDS_SET_R, (int)GAME_ORDINAL.SECOND, "Pyramids_Set_R", ITEM_TYPE.HITTABLE, ITEM_POSITION.RIGHT));

			//TOWER BUILDER GAME
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_GRASPABLES_SET_NEAR, (int)GAME_ORDINAL.THIRD, "Graspables_Set_N", ITEM_TYPE.GRASPABLE, ITEM_POSITION.NEAR));
			items.Add (new DB_Item ((int)IDS_ITEMS.ID_GRASPABLES_SET_FAR, (int)GAME_ORDINAL.THIRD, "Graspables_Set_F", ITEM_TYPE.GRASPABLE, ITEM_POSITION.FAR));

			return items;
		}

		private void storeOrUpdateBDItems (List<DB_Item> items)
		{
			for (int i = 0; i < items.Count; i++) {
				DB_Item it = DB_Manager.Instance.getOneItem (items [i].id_game, items [i].id);
				if (it == null) {
					DB_Manager.Instance.insertItem (items [i]);
				} else {
					items [i].id = it.id;
					DB_Manager.Instance.updateItem (items [i]);
				}
			}
		}

		public void storeOrUpdateItemsAndGames ()
		{
			storeOrUpdateDBGames (createDBGames ());
			storeOrUpdateBDItems (createDBItems ());
		}
	}
}
