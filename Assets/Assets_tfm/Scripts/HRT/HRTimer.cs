//----traduccion de HTTimer.h y HTTimer.cpp----

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


//----bibliotecas Unity3d----
using UnityEngine;
using System.Collections;
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

#if RTTIMER_H
public class HRTimer : RTTimer {
	//----constantes----
	//const en C# es const y static
	public const float RTT_NO_SAMPLING_FREQUENCYf = -1.0f;
	public const int RTT_NO_SAMPLING_FREQUENCY = -1;

#if OS_MSWINDOWS
	public const Int64 HRT_MAX_TIME = RTT_MAXTIME;    //8 bytes

	//64 bits constants
	public const Int64 HRT_TIME_INVALID = -1LL;
	public const Int64 HRT_NOTIME = 0LL;
	public const Int64 HRT_INMEDIATELY = 0LL;
#elif OS_LINUX
#elif OS_OSX
#elif OS_ANDROID
#endif
	//------------------

	public enum HRT_STATES{
		HRT_INACTIVE,   ///The HRT is not counting now. State reached just after EndCounting(); is invoked
		HRT_ACTIVE,   ///The HRT is counting now. State reached just after InitCounting(); is invoked
		HRT_MAX_STATES   ///Amount of different states a HRT can reach
	}
	
	string Name;
	public HRT_STATES State;

	///Performance attributes

	public HRT_Time InitialCounter,   ///<Initial performance counter value at the very beginning of the measured process. Units: System Clock Ticks
	FinalCounter,   ///<Final performance counter value at the very end of the measured process. Units: System Clock Ticks

	///Statistical derived values
	Accumulated,   ///<Total performance counter values accumulated during every measured process. Units: System Clock Ticks
	LastPeriod,   ///<Performance counter values accumulated during the last measured process. Units: System Clock Ticks
	MaxPartial,   ///<Maximun partial period detected during every measured process. Units: System Clock Ticks
	MinPartial,   ///<Minimum partial period detected during every measured process. Units: System Clock Ticks
	AlarmPeriod;   ///<Total Amount of time to wait from now on until the alarm sounds

	public UInt32 Periods;   ///<Amount of times a period has been accumulated

	//----ticks management----
	public void ResetSamplingFrequency(HRT_Time SF){
		if (HRT_STATES.HRT_ACTIVE == State){
			EndCounting();   //Careful. State is now HRT_INACTIVE
			Accumulate();   //Accumulation supervening. Not explicitly ordered by the user, so... 
			Periods--;   //... no accounting for another period has to be performed. Do not increment the amount of times the accumulation is performed.
			InitCounting();   //A change to the previous state is performed and a new accounting with the new value
			//Reset accumulated
			Accumulated = HRT_NOTIME;
		}
		SetSF(SF);
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void EndCounting (){
		FinalCounter = GetRealTime();
		LastPeriod = FinalCounter - InitialCounter;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HRT_Time TicksUntilNow () {
		EndCounting();
		return LastPeriod;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HRT_Time TotalTicksUntilNow (){
		HRT_Time T;
		TicksUntilNow();
		T = Accumulated + LastPeriod;
		return T;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Accumulate (){
		Accumulated += LastPeriod;
		Periods++;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AccumulateUntilNow (){
		Accumulated += TicksUntilNow();
		Periods++;
		InitCounting();
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ResetAccumulate(){
		Accumulated = 0;
		Periods = 0;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void InitCounting(){
		//UnityEngine.Debug.Log("----InitCounting");
		SW.Start(); ///<<<<<<<
		InitialCounter = GetRealTime();
		FinalCounter = InitialCounter;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public void Start(){
	public void Start(){
		InitCounting();
		State = HRT_STATES.HRT_ACTIVE;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ResetName (){
		Name = "";
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Reset(){
		InitialCounter	= HRT_NOTIME;
		FinalCounter	= HRT_NOTIME;
		Accumulated		= HRT_NOTIME;
		LastPeriod		= HRT_NOTIME;
		MaxPartial		= HRT_NOTIME;
		DisableAlarm();
		
		Periods			= 0;
		MinPartial		= RTT_MAXTIME;
		Start();
	}

	public void UpdateMinMax(){
		if(LastPeriod > MaxPartial)
			MaxPartial = LastPeriod;
		else if(LastPeriod < MinPartial)
			MinPartial = LastPeriod;
	}
	
	//Accessing to private attributes for read only mode
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HRT_Time GetAccumulated () {
		return Accumulated;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HRT_Time GetLastPeriod () {
		return LastPeriod;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HRT_Time GetMaxPartial () {
		return MaxPartial;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HRT_Time GetMinPartial () {
		return MinPartial;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UInt32 GetPeriods () {
		return Periods;
	}
	
	//Next computations are multiplied by 1000 ms in a second
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double GetAccumulatedms () {
		return Ticks2ms(Accumulated);
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double GetAccumulatedSecs () {
		return Ticks2s(Accumulated);
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double GetLastPeriodms () {
		return Ticks2ms(LastPeriod);
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double GetLastPeriodSecs(){
		return Ticks2s(LastPeriod);
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double GetMaxPartialms () {
		return Ticks2ms(MaxPartial);
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double GetMinPartialms () {
		return Ticks2ms(MinPartial);
	}
	
	//Alarms
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetAlarm(HRT_Time A) {
		AlarmPeriod = A;
		InitCounting();
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetAlarm (double A) {
		SetAlarm(ms2Ticks(A));
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HRT_Time GetAlarmPeriod () {
		return AlarmPeriod;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Double GetAlarmPeriodms () {
		return Ticks2ms(AlarmPeriod);
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsSounding () {
		EndCounting();
		return AlarmPeriod < LastPeriod;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void StartSounding () {
		FinalCounter = GetRealTime();
		InitialCounter = FinalCounter - AlarmPeriod;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ResetAlarm () {
		InitCounting();
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void DisableAlarm () {
		AlarmPeriod = HRT_MAX_TIME;
	}
	
	//Miscelanea
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetName (string N) {
		Name = N;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetName () {
		return Name;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Boolean IsActive () {
		return HRT_STATES.HRT_ACTIVE == State;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Boolean isInActive () {
		return HRT_STATES.HRT_INACTIVE == State;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetState (HRT_STATES S) {
		State = S;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HRT_STATES GetState () {
		return State;
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Activate () {
		if (isInActive()) Reset();
	}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void DeActivate () {
		State = HRT_STATES.HRT_INACTIVE;
	}

	public void DisplayHRTimerProperties(){
		if (Stopwatch.IsHighResolution){
			UnityEngine.Debug.Log("Operations timed using the system's high-resolution performance counter.");
		}
		else {
			UnityEngine.Debug.Log("Operations timed using the DateTime class.");
		}
		
		HRT_Time frequency = Stopwatch.Frequency;
		UnityEngine.Debug.Log("  Timer frequency in ticks per second = " +
		                  frequency.ToString());
		HRT_Time nanosecPerTick = (1000L*1000L*1000L) / frequency;
		UnityEngine.Debug.Log("  Timer is accurate within nanoseconds = " +
		                  nanosecPerTick.ToString());
	}
}
#endif