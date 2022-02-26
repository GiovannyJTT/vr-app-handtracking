using UnityEngine;
using System.Collections;
using DBTfm;
using DBTfm.Types;

namespace APPTfm
{
	public class LM_Info
	{
		private Leap.Controller c;
		private Leap.Device d;

		public LM_Info ()
		{
			c = new Leap.Controller ();
			d = c.Devices [0];
		}

		private bool getIsConnected ()
		{
			return c != null && d != null && d.IsValid && c.IsConnected;
		}

		private bool getIsStreaming ()
		{
			return getIsConnected () && d.IsStreaming;
		}

		private float getHorizontalView ()
		{
			return d.HorizontalViewAngle * 180.0f / 3.141592f;
		}

		private float getVerticalView ()
		{
			return d.VerticalViewAngle * 180.0f / 3.141592f;
		}

		private bool getIsHM ()
		{
			return c.IsPolicySet (Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
		}

		public string getLMInfo ()
		{
			string info_lm = "LM: ";
			info_lm += (getIsConnected ()) ? "Conectado," : "No conectado,";
			info_lm += "\n";
			info_lm += (getIsStreaming ()) ? "" : "No transmitiendo";
			if (getIsStreaming ()) {
				info_lm += "HView " + getHorizontalView ().ToString () + "º,";
				info_lm += "\n";
				info_lm += "VView " + getVerticalView ().ToString () + "º";
//				info_lm += "\n";
//				info_lm += (getIsHM ()) ? "En la cabeza" : "Sobre el escritorio";
			}
			return info_lm;
		}
	}



	public class DisplaysInfo
	{
		public static int nDisplays;

		public DisplaysInfo ()
		{
			nDisplays = Display.displays.Length;
		}

		public string getDisplaysInfo ()
		{
			string info = "Monitores: ";
			info += nDisplays.ToString () + "\n";
			info += "1-" + Display.displays[0].renderingWidth + "x" + Display.displays[0].renderingHeight;
			info += (nDisplays > 1)? "\n2-" + Display.displays[1].renderingWidth + "x" + Display.displays[1].renderingHeight : "";
			return info;
		}
	}



	public class GameSesionInfo
	{
		public GameSesionInfo ()
		{
		}

		private HCI_TYPE getCurrentHCI ()
		{
			return APP_Manager.Instance.getHCIType ();
		}

		public string getInteractionInfo (DB_GameSession gs)
		{
			string info = "Interacción Actual: \n";
			if(gs != null){
				info += gs.hci_type.ToString ();			 
			} else {
				info += getCurrentHCI ().ToString ();
			}
			return info;
		}
	}



	public class DB_Info{
		public string getDBInfo(){
			string info = "BD: ";
			if(DB_Manager.Instance.checkDBUriFile()){
				info += "Ok";
			} else {
				info += "Fallo";
			}
			return info;
		}
	}



	public class Oculus_Info {
		public string getOculusInfo(){
			string info = "Oculus: \n";
			if(OVRManager.instance != null){
				info += OVRManager.display.isPresent? "Display Activo, ": "Display No activo, ";
				info += OVRManager.tracker.isPresent? "Tracker Activo": "Tracker No activo";
			} else {
				info += "No conectado";
			}
			return info;
		}
	}

	public class APP_Info
	{
		private LM_Info lminfo;
		private DisplaysInfo dinfo;
		private GameSesionInfo gsinfo;
		private DB_Info dbinfo;
//		private Oculus_Info oInfo;

		public APP_Info ()
		{
			lminfo = new LM_Info ();
			dinfo = new DisplaysInfo ();
			gsinfo = new GameSesionInfo ();
			dbinfo = new DB_Info();
//			oInfo = new Oculus_Info();
		}

		public string getDevicesInfo (DB_GameSession gs)
		{
			string info_dev = "";
			info_dev += lminfo.getLMInfo ();
			info_dev += "\n\n" + dinfo.getDisplaysInfo ();
			info_dev += "\n\n" + gsinfo.getInteractionInfo (gs);
//			info_dev += "\n\n" + oInfo.getOculusInfo ();
			info_dev += "\n\n" + dbinfo.getDBInfo();
			return info_dev;
		}
	}

}