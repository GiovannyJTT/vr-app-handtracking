//----traduccion de HRTimerManager.h y HRTimerManager.cpp----

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

#if !HRTIMERM_H
#define HRTIMERM_H
#endif

//----bibliotecas Unity3d----
using UnityEngine;
using System.Collections;
//------------------------

//----bibliotecas HRT----
using System;                                //Int64
using System.Diagnostics;                    //Stopwatch
using System.Collections.Generic;            //List
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


#if HRTIMERM_H
public class HRTManager : HRTimer {
	public List<HRTimer> Timers = new List<HRTimer>();

	//----constantes----
	//const en C# es const y static
	public const int HRTM_NO_SAMPLING_FREQUENCY = -1;
	public const int HRTM_NO_TIMER_CREATED = -1;
	//------------------

	public enum HRTM_ERRORS {
		HRTM_NO_ERROR,
		HRTM_NO_TIMERS_CREATED,
		HRTM_MAX_ERRORS
	}

	public List<string> HRTM_ErrorMsg = new List<string>(){
		"No error", //Corresponds to HRTM_NO_ERROR error
		"No timer has been possible to be created"	//Corresponds to HRTM_NO_TIMERS_CREATED error
	};

	public string Name;   ///<String that identifies the timer. The name of the timer

	/// Hardware clock sampling frequency. This is the common sampling frequency that all timers
	/// managed by this manager will share among them
	public RTT_Time SamplingFrequency;

	//Methods
	//constructor
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HRTManager(){
		DestroyTimers();
		SamplingFrequency = HRTM_NO_SAMPLING_FREQUENCY;
	}

	//destructor
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	~HRTManager(){
		DestroyTimers();
	}

	/*
	public void listResize (List<HRTimer> list, int size){
		if (list != null) {
			if (size > list.Count)
				for (int i = 0; i <= size - list.Count; i++)
					list.Add (new HRTimer());
			else if (size < list.Count)
				for (int i = 0; i <= list.Count - size; i++)
					list.RemoveAt (list.Count - 1);
		}
	}
	*/

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CreateTimers(int T) {
		//listResize(Timers, T);

		for (int i = 0; i < T; i++){
			Timers.Add(new HRTimer());
			Timers[i].SetSF(Stopwatch.Frequency);
		}
		ResetSamplingFrequency();
	}

	public void SetTimersName (List<string> TimerNames){
		for (int i = 0; i < Timers.Count; i++)
			Timers[i].SetName(TimerNames[i]);
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ErrorMsg(HRTM_ERRORS E){
		return HRTM_ErrorMsg[(int)E];
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void DestroyTimers(){
		if(Timers != null) Timers.Clear();
		Name = "";
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool SamplingFrequencyAvailable(){
		return HRTM_NO_SAMPLING_FREQUENCY != SamplingFrequency;
	}

	public void ResetSamplingFrequency(){
		//SetSamplingFrequency(HRTM_NO_SAMPLING_FREQUENCY);
		SamplingFrequency = Stopwatch.Frequency;
		int i, size;
		size = Timers.Count;
		if (size > 0){
			for (i = 0; i < size; i++)
				Timers[i].ResetSamplingFrequency(SamplingFrequency);
		}
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ResetAllTimers(){
		for(int i = 0; i < Timers.Count; i++)
			Timers[i].Reset();
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetName(string N){
		Name = N;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetName(){
		return Name;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetSamplingFrequency(RTT_Time SF){
		SamplingFrequency = SF;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void UpdateWindow(RTT_Time SF){
		int i, size = Timers.Count;
		for(i = 0; i < size; i++)
			Timers[i].SetSF(SF);
	}
}
#endif
