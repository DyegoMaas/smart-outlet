#include "SensoresCorrente.h"
using namespace SensoresCorrente;

	//class ACS712;
	/*struct DCReading;
	struct ACReading;*/

ACS712Class::ACS712Class(ModuleType moduleType)
{
	switch (moduleType)
	{
		case _30A: mVperAmp = 66; break;
		case _20A: mVperAmp = 100; break;
		case _5A:
		default: mVperAmp = 185; break;
	}
}

DCReading ACS712Class::readDC(int analogInPin) const
{
	auto rawValue = analogRead(analogInPin);
	auto voltage = rawValue / 1024.0 * 5000; // Gets you mV
	auto amps = (voltage - ACSoffset) / mVperAmp;
		
	return DCReading(amps, voltage);
}

float getVPP(int analogInPin)
{
	auto maxValue = 0;          // store max value here
	auto minValue = 1024;          // store min value here
	
	uint32_t start_time = millis();
	while ((millis() - start_time) < 1000) //sample for 1 Sec
	{
		auto readValue = analogRead(analogInPin);
	
		// see if you have a new maxValue
		if (readValue > maxValue)
		{
			/*record the maximum sensor value*/
			maxValue = readValue;
		}
		if (readValue < minValue)
		{
			/*record the maximum sensor value*/
			minValue = readValue;
		}
	}
	
	// Subtract min from max
	float result = ((maxValue - minValue) * 5.0) / 1024.0;	
	return result;
}

ACReading ACS712Class::readAC(int analogInPin) const
{
	auto current = getVPP(analogInPin);
	auto VRMS = (current / 2.0) * 0.707;
	auto ampsRms = (VRMS * 1000) / mVperAmp;
		
	return ACReading(ampsRms, VRMS);
}

	
