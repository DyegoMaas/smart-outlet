//// ACS712.h
//
//#ifndef _ACS712_h
//#define _ACS712_h
//
//#if defined(ARDUINO) && ARDUINO >= 100
//	#include "Arduino.h"
//#else
//	#include "WProgram.h"
//#endif
//
//
//#endif
//
//#include "SensoresCorrente.h"
//
////
////struct DCReading
////{
////	float amps;
////	float voltage;
////	DCReading(float current, float voltage) : amps(current), voltage(voltage) {}
////};
//
//class ACS712
//{
//	private:
//		const float mVperAmp = 66; // use 100 for 20A Module and 185 for 5A Module
//		const float ACSoffset = 2500;
//	
//	public:
//	SensoresCorrente::ACReading readDC(int analogInPin);
//	SensoresCorrente::ACReading readAC(int analogInPin);
//};
//
