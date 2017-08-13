//
//#include "SensoresCorrente.h"
//
//SensoresCorrente::ACReading ::ACS712::readDC(int analogInPin)
//{
//	auto rawValue = analogRead(analogInPin);
//	auto tensao = rawValue / 1024.0 * 5000; // Gets you mV
//	auto amps = (tensao - ACSoffset) / mVperAmp;
//
//	return amps;
//}
//
//float getVPP(int analogInPin)
//{
//	auto maxValue = 0;          // store max value here
//	auto minValue = 1024;          // store min value here
//
//	uint32_t start_time = millis();
//	while ((millis() - start_time) < 1000) //sample for 1 Sec
//	{
//		auto readValue = analogRead(analogInPin);
//
//		// see if you have a new maxValue
//		if (readValue > maxValue)
//		{
//			/*record the maximum sensor value*/
//			maxValue = readValue;
//		}
//		if (readValue < minValue)
//		{
//			/*record the maximum sensor value*/
//			minValue = readValue;
//		}
//	}
//
//	// Subtract min from max
//	float result = ((maxValue - minValue) * 5.0) / 1024.0;	
//	return result;
//}
//
////ACReading ::ACS712::readAC(int analogInPin)
////{
////	auto tensao = getVPP(analogInPin);
////	/*Serial.print("voltage: ");
////	Serial.println(tensao);*/
////	
////	auto VRMS = (tensao / 2.0) * 0.707;
////	
////	/*Serial.print("VRMS: ");
////	Serial.println(VRMS);*/
////	
////	auto ampsRms = (VRMS * 1000) / mVperAmp;
////	
////	/*auto watts = ampsRms * (tensao * 1000);
////	Serial.print("watts: ");
////	Serial.println(watts);*/
////	
////	auto x = new ACReading();
////	return ampsRms;
////}
//
//
//SensoresCorrente::ACReading::ACS712::readAC(int analogInPin)
//{
//	auto voltage = getVPP(analogInPin);
//	/*Serial.print("voltage: ");
//	Serial.println(tensao);*/
//		
//	auto VRMS = (voltage / 2.0) * 0.707;
//		
//	/*Serial.print("VRMS: ");
//	Serial.println(VRMS);*/
//		
//	auto ampsRms = (VRMS * 1000) / mVperAmp;
//		
//	/*auto watts = ampsRms * (tensao * 1000);
//	Serial.print("watts: ");
//	Serial.println(watts);*/
//		
//	auto reading = new SensoresCorrente::ACReading(ampsRms, voltage);
//	return reading;
//}
