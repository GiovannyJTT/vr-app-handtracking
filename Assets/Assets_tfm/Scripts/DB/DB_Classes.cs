using System;
using DBTfm.Types;

namespace DBTfm
{
	public class DB_Person
	{
		public int id = -1;
		public int code = -1;
		public string name = "";
		public int age = -1;
		public SEX sex;

		public string idStr = "";
		public string codeStr = "";
		public string ageStr = "";
		public string sexStr = "";

		public DB_Person ()
		{
		}

		public DB_Person (int id, int c, string n, int a, SEX s)
		{
			this.id = id;
			this.code = c;
			this.name = n;
			this.age = a;
			this.sex = s;

			this.idStr = this.id.ToString ();
			this.codeStr = this.code.ToString ("D3");
			this.ageStr = this.age.ToString ();
			this.sexStr = ((int)this.sex).ToString ();
		}
	}

	public class DB_GameSession
	{
		public int id = -1;
		public int id_person = -1;
		public int id_game = -1;
		public int session_number = -1;
		public DateTime date = DateTime.MinValue;
		public HCI_TYPE hci_type;
		public GAME_SESION_STATE state;
		public double goal_reached_seconds = -1f;
		public int points_obtained = 0;

		public string idStr = "";
		public string id_personStr = "";
		public string id_gameStr = "";
		public string session_numberStr = "";
		public string dateStr = "";
		public string hci_typeStr = "";
		public string stateStr = "";
		public string goal_reached_secondsStr = "";
		public string points_obtainedStr = "";

		public static string dateFormat = "yyyy-MM-dd HH:mm:ss";

		public DB_GameSession ()
		{
		}

		public DB_GameSession (int id, int id_p, int id_g, int sn, DateTime d, HCI_TYPE hci, GAME_SESION_STATE gss, double grs, int ps_o)
		{
			this.id = id;
			this.id_person = id_p;
			this.id_game = id_g;
			this.session_number = sn;
			this.date = d;
			this.hci_type = hci;
			this.state = gss;
			this.goal_reached_seconds = grs;
			this.points_obtained = ps_o;

			this.idStr = this.id.ToString ();
			this.id_personStr = this.id_person.ToString ();
			this.id_gameStr = this.id_game.ToString ();
			this.session_numberStr = this.session_number.ToString ();
			this.dateStr = this.date.ToString (dateFormat);
			this.hci_typeStr = ((int)this.hci_type).ToString ();
			this.stateStr = ((int)this.state).ToString ();
			this.goal_reached_secondsStr = this.goal_reached_seconds.ToString("##.###");
			this.points_obtainedStr = this.points_obtained.ToString();
		}

		public void setCurrentDate ()
		{
			this.date = DateTime.Now;
			this.dateStr = this.date.ToString (dateFormat);
		}

		public void setReachedOK(double reachedSecs, int points_obtained){
			this.goal_reached_seconds = reachedSecs;
			this.goal_reached_secondsStr = this.goal_reached_seconds.ToString("##.###");
			this.state = GAME_SESION_STATE.COMPLETED;
			this.points_obtained = points_obtained;
			this.points_obtainedStr = this.points_obtained.ToString();
			this.stateStr = ((int)this.state).ToString ();
		}

		public void setGSState(GAME_SESION_STATE gss){
			this.state = gss;
			this.stateStr = ((int)this.state).ToString ();
		}
	}

	public class DB_Game
	{
		public int id = -1;
		public int ordinal = -1;
		public GAME_TYPE game_type;
		public double limit_seconds = -1f;
		public string goal_description = "";

		public string idStr = "";
		public string ordinalStr = "";
		public string game_typeStr = "";
		public string limit_secondsStr = "";

		public DB_Game ()
		{
		}

		public DB_Game (int id, int o, GAME_TYPE gt, double ls, string gd)
		{
			this.id = id;
			this.ordinal = o;
			this.game_type = gt;
			this.limit_seconds = ls;
			this.goal_description = gd;

			this.idStr = this.id.ToString ();
			this.ordinalStr = this.ordinal.ToString();
			this.game_typeStr = ((int)this.game_type).ToString ();
			this.limit_secondsStr = this.limit_seconds.ToString ("##.###");
		}
	}

	public class DB_Item
	{
		public int id = -1;
		public int id_game = -1;
		public string name = "";
		public ITEM_TYPE item_type;
		public ITEM_POSITION item_position;

		public string idStr = "";
		public string id_gameStr = "";
		public string item_typeStr = "";
		public string item_positionStr = "";
		public string item_sucessStr = "";

		public DB_Item ()
		{
		}

		public DB_Item (int id, int id_g, string n, ITEM_TYPE i_t, ITEM_POSITION i_p)
		{
			this.id = id;
			this.id_game = id_g;
			this.name = n;
			this.item_type = i_t;
			this.item_position = i_p;

			this.idStr = this.id.ToString ();
			this.id_gameStr = this.id_game.ToString ();
			this.item_typeStr = ((int)this.item_type).ToString ();
			this.item_positionStr = ((int)this.item_position).ToString ();
		}
	}

	public class DB_Item_Attempt
	{
		public int id = -1;
		public int id_game_session = -1;
		public int id_item = -1;
		public double rt = -1f;
		//reaction time
		public int attemp_number = -1;

		public string idStr = "";
		public string id_game_sessionStr = "";
		public string id_itemStr = "";
		public string rtStr = "";
		//reaction time
		public string attemp_numberStr = "";

		public DB_Item_Attempt ()
		{
		}

		public DB_Item_Attempt (int id, int id_gs, int id_i, double rt, int a_n)
		{
			this.id = id;
			this.id_game_session = id_gs;
			this.id_item = id_i;
			this.rt = rt;
			this.attemp_number = a_n;

			this.idStr = this.id.ToString ();
			this.id_game_sessionStr = this.id_game_session.ToString ();
			this.id_itemStr = this.id_item.ToString ();
			this.rtStr = this.rt.ToString ("##.###");
			this.attemp_numberStr = this.attemp_number.ToString ();
		}

		public void setAttemptNumber(int an){
			this.attemp_number = an;
			this.attemp_numberStr = this.attemp_number.ToString();
		}

		public void setRT(double rt){
			this.rt = rt;
			this.rtStr = this.rt.ToString("##.###");
		}
	}

}
