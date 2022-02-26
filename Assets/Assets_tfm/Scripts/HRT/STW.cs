/*
	Ejemplo de uso básico de la clase StopWatch para realizar
	mediciones temporales de alta resolucion en Unity3d
*/

using System.Runtime.InteropServices;   //DllImport
using System.Diagnostics;   //StopWatch
using System;

using UnityEngine;
using System.Collections;

public class STW : MonoBehaviour {
	long Now, Previous, ElapsedMs;
	Stopwatch stw;

	void Start () {
		DisplaySTWProperties ();

		stw = new Stopwatch ();
		stw.Start ();

		Previous = Now = (long)(1000L * Time.realtimeSinceStartup);
	}
	
	// Update is called once per frame
	void Update () {
		Now = (long)(1000L * Time.realtimeSinceStartup);
		stw.Stop ();

		ElapsedMs = Now - Previous;
		UnityEngine.Debug.Log (stw.ElapsedMilliseconds.ToString() +  "    " + ElapsedMs.ToString() );
		Previous = Now;

		stw.Reset ();
		stw.Start ();
	}

	void DisplaySTWProperties(){
		if (Stopwatch.IsHighResolution){
			UnityEngine.Debug.Log("Se utiliza system's high-resolution performance counter.");
		}
		else {
			UnityEngine.Debug.Log("Se utiliza DateTime class.");
		}
		
		long frequency = Stopwatch.Frequency;
		UnityEngine.Debug.Log(" Frecuencia (ticks/segundo) = " +
		                      frequency.ToString());
		long nanosecPerTick = (1000L*1000L*1000L) / frequency;
		UnityEngine.Debug.Log(" Nanosegundos por tick = " +
		                      nanosecPerTick.ToString());
	}
}
