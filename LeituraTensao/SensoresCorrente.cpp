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
		
		/*case _30A: mVperAmp = 0.066; break;
		case _20A: mVperAmp = 0.1; break;
		case _5A:
		default: mVperAmp = 0.185; break;*/
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



void printThis(char string[], float valor)
{
	Serial.print(string);
	Serial.print(": ");
	Serial.println(valor);
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

	//printThis("max", maxValue);
	//printThis("min", minValue);
	//printThis("diff", maxValue - minValue);
	//printThis("diff * V (arduino)", (maxValue - minValue)*5.0);
	
	// Subtract min from max
	const auto arduinoVoltage = 5.0;
	auto voltsPerUnit = arduinoVoltage / 1024.0;
	float result = (maxValue - minValue) * voltsPerUnit;
	return result;
}

float getVPP2(int pin)
{
	auto sum = 0.0;
	for (int i = 10000; i>0; i--) {
		// le o sensor na pino analogico A0 e ajusta o valor lido ja que a saída do sensor é (1023)vcc/2 para corrente =0
		float sensorValue_aux = (analogRead(pin) - 510);
		// somam os quadrados das leituras.
		sum += pow(sensorValue_aux, 2);
		delay(1);
	}

	return sum;
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




	auto voltageAmplitude = getVPP2(analogInPin);
	const auto voltsporUnidade = 5 / 1024.0;
	auto VRMS = (sqrt(voltageAmplitude / 10000)) * voltsporUnidade;
	float sensibilidade = 0.066;
	auto ampsRms = (VRMS / sensibilidade/*mVperAmp*/);
	if (ampsRms <= 0.095) {
		ampsRms = 0;
	}
	const auto voltage = 220;
	auto power = ampsRms * voltage;
	//return ACReading(ampsRms, voltage, power);







	auto voltageAmplitude2 = getVPP(analogInPin);
	//auto vMax = voltageAmplitude2 / 2.0;
	auto vMax = voltageAmplitude2 / 2.0;
	auto adjust = 2.0;
	auto VRMS2 = vMax * 0.707 / adjust;

	//auto VRMS = Utils::computeVrms(voltageAmplitude);


	//auto vrmsVolts = VRMS * 1000;
	auto ampsRms2 = VRMS2 / sensibilidade;//mVperAmp;
	if (ampsRms2 <= 0.095) {
		ampsRms2 = 0;
	}
	auto power2 = ampsRms2 * 220;
	//auto power = Utils::computePower(ampsRms, VRMS);

	printThis("voltage amplitude I", voltageAmplitude);
	printThis("voltage amplitude II", voltageAmplitude2);
	printThis("-------------", 0);
	printThis("VRMS I", VRMS);
	printThis("VRMS II", VRMS2);
	printThis("-------------", 0);
	printThis("VRMS I", VRMS);
	printThis("VRMS II", VRMS2);
	printThis("-------------", 0);
	printThis("AMP I", ampsRms);
	printThis("AMP II", ampsRms2);
	printThis("-------------", 0);
	printThis("POWER I", power);
	printThis("POWER II", power2);
	printThis("-------------", 0);

	delay(3000);


	return ACReading(ampsRms, VRMS, power);
}