#include "SensoresCorrente.h"
#include "Utils.h"
using namespace SensoresCorrente;

ACS712Class::ACS712Class(ModuleType moduleType)
{
	//TODO verificar se é isso mesmo
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
//
//int getMaxValue(int analogInPin)
//{
//	int sensorValue;    //value read from the sensor
//	int sensorMax = 0;
//	uint32_t start_time = millis();
//	while ((millis() - start_time) < 1000) //sample for 1000ms
//	{
//		sensorValue = analogRead(analogInPin);
//		if (sensorValue > sensorMax)
//		{
//			/*record the maximum sensor value*/
//
//			sensorMax = sensorValue;
//		}
//	}
//	return sensorMax;
//}
//
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
	//auto sensor_max = getMaxValue(analogInPin);
	//Serial.print("sensor_max = ");
	//Serial.println(sensor_max);

	////the VCC on the Arduino interface of the sensor is 5v

	//auto amplitude_current = (float)(sensor_max - 512) / 1024 * 5 / 185 * 1000000; // for 5A mode,you need to modify this with 20 A and 30A mode;
	//auto effective_value = amplitude_current / 1.414;

	////for minimum current=1/1024*5/185*1000000/1.414=18.7(mA)
	////Only sinusoidal alternating current

	//Serial.println("The amplitude of the current is(in mA)");
	//Serial.println(amplitude_current, 1);

	////Only one number after the decimal point

	//Serial.println("The effective value of the current is(in mA)");
	//Serial.println(effective_value, 1);

	//return ACReading(effective_value * 1000, VRMS, power);





	/*auto amplitude = getVPP(analogInPin);
	Serial.print("amplitude: ");
	Serial.print(amplitude);
	Serial.print("	");

	auto VRMS = (amplitude / 2.0) * 0.707;
	auto ampsRms = (VRMS * 1000) / mVperAmp;

	auto power = ampsRms * (VRMS * 1000);
	return ACReading(ampsRms, VRMS, power);*/








	auto voltageAmplitude = getVPP(analogInPin);
	auto VRMS = Utils::computeVrms(voltageAmplitude);
	auto vrmsVolts = VRMS * 1000;
	auto ampsRms = vrmsVolts / mVperAmp;
	auto power = Utils::computePower(ampsRms, vrmsVolts);

	return ACReading(ampsRms, vrmsVolts, power);
}