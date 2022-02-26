//----conversion de tipos de C++ a C#----
/*
INT64_MAX            Int64.Maxvalue   8 Bytes
long long            Int64            8 Bytes
unsigned char        Byte             1 Byte
unsigned short int   UInt16           2 Bytes
unsigned int         uint, UInt32     4 bytes
double               Double           8 Bytes
*/
//---------------------------------------


#if !OS_OPERATINGSYSTEM
#define OS_OPERATINGSYSTEM
#define OS_MSWINDOWS
#define OS_64BITS
#endif

#if !RTTIMER_H
#define RTTIMER_H
#define RTT_MM_COUNTERS         //QPC-Stopwatch
//#define RTT_TIME_STAMP_ASM    //HRTdll
#endif

#if !HRTIMER_H
#define HRTIMER_H
#endif


//----depuracion pruebas----
#if !PRINTCONSOLE
//#define PRINTCONSOLE
#endif

#if !PRINTTEXT
#define PRINTTEXT
#endif


//----bibliotecas Unity3d----
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//------------------------

//----bibliotecas HRT----
using System;                                //Int64
using System.Diagnostics;                    //Stopwatch
//using System.Runtime.InteropServices;        //DllImport
//using System.Runtime.CompilerServices;       //AggressivInlining (.Net 4.5)
//-----------------------


//----constantes y tipos-----
#if OS_MSWINDOWS
using RTT_Time = System.Int64;
using HRT_Time = System.Int64;
#elif OS_LINUX
#elif OS_OSX
#elif OS_ANDROID
#endif


public class HRTTest : MonoBehaviour {
	public const int TOTAL_TIMERS = 2;
	public const int COUNTING = 1000000;
	public const int COUNTING2 = 10000000;

	public Text texto;
	string salida = "";

	//'Awake' se ejecuta en el arranque, 'Start' se ejecuta cuando finalizan todos los 'Awake'
	void Awake (){
		int n;
		Int64 accumulator = 0;
		HRT_Time a, Sf;
		Double c;

		HRTManager TM = new HRTManager ();
		TM.ResetSamplingFrequency();
		TM.CreateTimers (TOTAL_TIMERS);

		//----Prueba 1: medir con HRTimer 0 cuantos ms le cuesta al procesador iterar COUNTING veces----
		TM.Timers [0].InitCounting ();

		//un millon de iteraciones
		for (n = 0; n < COUNTING; n++) {
			accumulator += accumulator;
		}

		TM.Timers[0].EndCounting();
		TM.Timers[0].Accumulate();

		//----Prueba 2: medir con HRTimer 1 cuantos ms le cuesta al procesador iterar COUNTING veces----
		TM.Timers[1].InitCounting();

		a = TM.Timers[0].GetAccumulated();
		Sf = TM.Timers[0].GetSamplingFreq();
		c = TM.Timers[0].GetAccumulatedms();

#if PRINTCONSOLE
		UnityEngine.Debug.Log("Total counting = " + a.ToString());
		UnityEngine.Debug.Log("Sampling Freq. = " + Sf.ToString());
		UnityEngine.Debug.Log("ms = " + c.ToString() + " \n");
		UnityEngine.Debug.Log("nsXIter = " + (c*1000L*1000L/COUNTING).ToString() + " \n");
#elif PRINTTEXT
		salida += "Total counting = " + a.ToString() +
			" Sampling Freq. = " + Sf.ToString() +
			" ms = " + c.ToString() + " \n" + 
			" nsXIter = " + (c*1000L*1000L/COUNTING).ToString() + " \n\n";
#endif

		TM.Timers[1].EndCounting();
		TM.Timers[1].Accumulate();

		a = TM.Timers[1].GetAccumulated();
		Sf = TM.Timers[1].GetSamplingFreq();
		c = TM.Timers[1].GetAccumulatedms();

#if PRINTCONSOLE
		UnityEngine.Debug.Log("Total counting = " + a.ToString());
		UnityEngine.Debug.Log("Sampling Freq. = " + Sf.ToString());
		UnityEngine.Debug.Log("ms = " + c.ToString() + " \n");
		UnityEngine.Debug.Log("nsXIter = " + (c*1000L*1000L/COUNTING).ToString() + " \n");
#elif PRINTTEXT
		salida += "Total counting = " + a.ToString() +
			" Sampling Freq. = " + Sf.ToString() +
			" ms = " + c.ToString() + " \n" + 
			" nsXIter = " + (c*1000L*1000L/COUNTING).ToString() + " \n\n";
#endif

		//----Prueba 3: medir con HRTimer 1 cuantos segundos le cuesta al procesdor iterar COUNTING2 veces----
		TM.Timers[1].InitCounting();
		accumulator = 0;
		//diez millones de iteraciones
		for (n = 0; n < COUNTING2; n++){
			TM.Timers[1].EndCounting();
			if (TM.Timers[1].GetLastPeriodms() > 1000.0d){
		#if PRINTCONSOLE
				UnityEngine.Debug.Log(
				"Counting = " + (accumulator++).ToString() + " " +
				" n = " + n.ToString() +
				" TS = " + TM.Timers[1].GetRealTime().ToString() + 
				" LastPeriodms = " + TM.Timers[1].GetLastPeriodms() + "\n");
		#elif PRINTTEXT
				salida += "Counting = " + (accumulator++).ToString() + " " +
					" n = " + n.ToString() +
					" TS = " + TM.Timers[1].GetRealTime().ToString() + 
					" LastPeriodms = " + TM.Timers[1].GetLastPeriodms() + "\n";
		#endif
				TM.Timers[1].InitCounting();
			}
		}

		//----prueba 4: contar el numero de accesos en un intervalo de 1 segundo, Parada controlada con HRTimer 1----
		accumulator = 0;
		a = TM.Timers[1].GetRealTime();
		c = a + Sf;

		double tini = 1000L * Time.realtimeSinceStartup;

		TM.Timers[1].InitCounting();

		do {
			TM.Timers[1].EndCounting();
			accumulator++;
		} while (TM.Timers[1].GetLastPeriod() < Sf);

		double tfin = 1000L * Time.realtimeSinceStartup;
		double t = tfin - tini;

#if PRINTCONSOLE
		UnityEngine.Debug.Log(
			"Initial counting = " + a.ToString() +
			" Final Counting = " + TM.Timers[1].GetRealTime().ToString() +
			" Total accesses per second = " + accumulator.ToString() +
			" Last Period = " + TM.Timers[1].GetLastPeriodms() + " ms" +
			"\n");
		UnityEngine.Debug.Log("Last Period Unity3D = " + t.ToString() + " ms");
#elif PRINTTEXT
		salida += "Initial counting = " + a.ToString() +
			" Final Counting = " + TM.Timers[1].GetRealTime().ToString() +
			" Total accesses per second = " + accumulator.ToString() +
			" Last Period = " + TM.Timers[1].GetLastPeriodms() + " ms" +
			"\n" +
			"Last Period Unity3D = " + t.ToString() + " ms"  + "\n\n";
#endif

		//----prueba 5: contar el numero de accesos en un intervalo de 1 segundo, Parada controlada con Time.realTimeSinceStartUp de Unity----
		accumulator = 0;

		tini = Time.realtimeSinceStartup;   //s
		tfin = tini + 1L;   //s
		t = tfin - tini;

		a = TM.Timers[1].GetRealTime();
		TM.Timers[1].InitCounting();

		do {
			accumulator++;
		} while (Time.realtimeSinceStartup < tfin);   //s

		TM.Timers[1].EndCounting();

#if PRINTCONSOLE
		UnityEngine.Debug.Log(
			"Initial counting = " + a.ToString() +
			" Final Counting = " + TM.Timers[1].GetRealTime().ToString() +
			" Total accesses per second = " + accumulator.ToString() +
			" Last Period = " + TM.Timers[1].GetLastPeriodms() + " ms" +
			"\n");
		UnityEngine.Debug.Log("Last Period Unity3D = " + (t * 1000L).ToString() + " ms" + "\n\n");
#elif PRINTTEXT
		salida += "Initial counting = " + a.ToString() +
			" Final Counting = " + TM.Timers[1].GetRealTime().ToString() +
			" Total accesses per second = " + accumulator.ToString() +
			" Last Period = " + TM.Timers[1].GetLastPeriodms() + " ms" +
			"\n" + 
			"Last Period Unity3D = " + (t * 1000L).ToString() + " ms";
#endif

	}

	void Start(){
		texto.text = salida;
	}
}