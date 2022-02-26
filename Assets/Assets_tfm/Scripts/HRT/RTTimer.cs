//----traduccion de RTTimer.h y RTTimer.cpp----

/*
Los define en C# deben aparecer antes de cualquier instruccion
que no sea directiva de preproceador, y no se pueden utilizar
como las macros de C++ para asignar valores a , en su lugar
usamos 'enum' o 'const'. La deficion de variables enum o const
solo se puede realizar dentro de una clase.
*/

/*
Las funciones inline en C# estan sepueden emular sobre .NET 4.5
mediante incrustado agresivo empleando:
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
Mono (sobre el que funciona Unity3d) no tiene soporte para esto,
Por lo que el tiempo hay que tener encuenta que existe un tiempo
de pasar por la pila de funciones.
*/

/*
La clase StopWatch de System.Diagnostics es una clase del sistema
de Windows para realizar mediciones temporales de alta resolucion.
Para dichas mediciones emplea por debajo las funciones
QueryPerformanceCounter y QueryPerformanceFrequency, que acceden
directamente al Performance Counter del South-Bridge y permiten
otener la cuenta actual de ticks y la frecuencia en ticks/s.
Propiedades:
	Elapsed					Obtiene el tiempo total transcurrido medido por la instancia actual.
	ElapsedMilliseconds		Obtiene el tiempo total transcurrido medido por la instancia actual, en milisegundos.
	ElapsedTicks 			Obtiene el tiempo total transcurrido medido por la instancia actual, en tics de temporizador.
	IsRunning				Obtiene un valor que indica si el temporizador Stopwatch está en funcionamiento.
Metodos principales:
	Reset()			Detiene la medición del intervalo de tiempo y restablece el tiempo transcurrido en cero.
	Restart()		Detiene la medición del intervalo de tiempo, restablece el tiempo transcurrido en cero y comienza a medir el tiempo transcurrido.
	Start()			Inicia o reanuda la medición del tiempo transcurrido para un intervalo.
	StartNew()		Inicializa una nueva instancia de Stopwatch, establece la propiedad de tiempo transcurrido en cero e inicia la medición de tiempo transcurrido.
	Stop()			Detiene la medición del tiempo transcurrido para un intervalo.
Campos:
	Frequency			Obtiene la frecuencia del temporizador en forma de número de tics por segundo. Este campo es de solo lectura.
	IsHighResolution	Indica si el temporizador se basa en un contador de rendimiento de alta resolución. Este campo es de solo lectura.
*/

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
#elif OS_LINUX
#elif OS_OSX
#elif OS_ANDROID
#endif

/*
//----definicion de nuevos tipos, C++:

typedef unsigned char		RTT_Shorts	[8];
typedef unsigned short int	RTT_Ints	[4];
typedef unsigned long		RTT_Longs	[2];

//----definicion de nuevos tipos, C#:

[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
public struct RTT_Shorts {
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]
	public Byte s;
};

[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
public struct RTT_Ints {
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst=4)]
	public UInt16 i;
};

[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
public struct RTT_Longs {
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst=2)]
	public UInt32 l;
};
*/
//---------------------------

#if RTTIMER_H
public class RTTimer {
	//----constantes----
	//const en C# es const y static
	public const float RTT_NO_SAMPLING_FREQUENCYf = -1.0f;
	public const int RTT_NO_SAMPLING_FREQUENCY = -1;

#if OS_MSWINDOWS
	public const Int64 RTT_MAXTIME = Int64.MaxValue;    //8 bytes
#elif OS_LINUX
#elif OS_OSX
#elif OS_ANDROID
#endif
	//------------------

	//----atributos----
	//StopWatch es una estructura que mide el tiempo transcurrido
	//Propiedades: Frequency, isHighResolution, IsRunning
	public Stopwatch SW = new Stopwatch();

	public RTT_Time SamplingFrequency, Time;    //Sampling frequency in ms
	public Double SFms, msXTick;                //msXTick = 1/SFms
	//-----------------

	//----metodos----
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetSF(RTT_Time SF){
		SamplingFrequency = SF;
		SFms = SF * 0.001d;
		if (RTT_NO_SAMPLING_FREQUENCY == SF)
			msXTick = RTT_NO_SAMPLING_FREQUENCYf;
		else
			msXTick = 1.0d / SFms;
	}

	/* 
	//Funciones redundantes eliminadas
	//public void ResetSF(){}
	//public void ReSetSFnb (){}
	*/

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RTT_Time GetSamplingFreq(){
		return SamplingFrequency;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Double Ticks2ms(RTT_Time T){
		return T*msXTick;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Double Ticks2s(RTT_Time T){
		return T*msXTick*0.001f;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RTT_Time ms2Ticks(Double ms){
		RTT_Time aux;
		aux = (RTT_Time) (SFms*ms);
		return aux;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Double GetSamplingFreqms(){
		return SFms;
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RTT_Time GetRealTime(){
#if RTT_MM_COUNTERS
	#if OS_MSWINDOWS
		//En C++: QueryPerformanceCounter((LARGE_INTEGER *)&Time);
		//En C#: la clase StopWatch emplea por debajo el QPC
		//  Se pueden consultar las propiedades de tiempo transcurrido cuando
		//  esta corriendo o cuando esta detenido
		Time = SW.ElapsedTicks;

		return Time;
	#elif OS_LINUX
	#elif OS_OSX
	#elif OS_ANDROID
	#endif

#elif RTT_TIME_STAMP_ASM
		//HRTdll
		//[DllImport("HRTdll.dll")]
		//public static extern IntPtr QueryPerformanceFrecuency(Int64 SamplingFrecuency);
#endif
	}
}
#endif