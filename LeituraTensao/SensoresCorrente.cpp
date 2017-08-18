#include "SensoresCorrente.h"
#include "Utils.h"
using namespace SensoresCorrente;

ACS712::ACS712(ModuleType moduleType)
{
	switch (moduleType)
	{
		case _30A: mVperAmp = 0.066; break;
		case _20A: mVperAmp = 0.1; break;
		case _5A:
		default: mVperAmp = 0.185; break;
	}
}

DCReading ACS712::readDC(int analogInPin) const
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
	const auto arduinoVoltage = 5.0;
	auto voltsPerUnit = arduinoVoltage / 1024.0;
	float result = (maxValue - minValue) * voltsPerUnit;
	return result;
}

ACReading ACS712::readAC(int analogInPin) const
{
	auto voltageAmplitude = getVPP(analogInPin);
	auto vMax = voltageAmplitude / 2.0;
	const auto adjust = 2.0;
	auto vrms = vMax * 0.707 / adjust;
	auto ampsRms = vrms / mVperAmp;
	if (ampsRms <= 0.095) {
		ampsRms = 0;
	}
	
	const auto voltage = 220;
	auto power = ampsRms * voltage;

	return ACReading(ampsRms, voltage, power);
}