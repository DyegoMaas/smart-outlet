#include "SensoresCorrente.h"
#include "Utils.h"
using namespace SensoresCorrente;

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
	auto power = Utils::computePower(amps, voltage);
	return DCReading(amps, voltage, power);
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
	auto voltageAmplitude = getVPP(analogInPin);
	auto VRMS = Utils::computeVrms(voltageAmplitude);
	auto vrmsVolts = VRMS * 1000;
	auto ampsRms = vrmsVolts / mVperAmp;
	auto power = Utils::computePower(ampsRms, vrmsVolts);

	return ACReading(ampsRms, vrmsVolts, power);
}