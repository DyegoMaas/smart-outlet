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


float getAmplitudeOverOneSecond(int analogInPin)
{
  auto maxValue = 0;         
  auto minValue = 4096;
  
  uint32_t start_time = millis();
  while ((millis() - start_time) < 1000) //sample for 1 Sec
  {
    auto readValue = analogRead(analogInPin);
  
    if (readValue > maxValue)
      maxValue = readValue;
    
    if (readValue < minValue)
      minValue = readValue;
  }

  // Subtract min from max
  const auto inputVoltage = 3.4; // Volts
  auto voltsPerUnit = inputVoltage / 4096.0;
  float result = (maxValue - minValue) * voltsPerUnit;
  return result;
}

ACReading ACS712::readAC(int analogInPin) const
{
  int RawValue = analogRead(analogInPin);
  double Voltage = (RawValue / 1024.0) * 3400; // Gets you mV
  double Amps = ((Voltage - ACSoffset) / mVperAmp);

  Serial.print("Raw Value = " ); // shows pre-scaled value 
  Serial.print(RawValue); 
  Serial.print("\t mV = "); // shows the voltage measured 
  Serial.print(Voltage,3); // the '3' after voltage allows you to display 3 digits after decimal point
  Serial.print("\t Amps = "); // shows the voltage measured 
  Serial.println(Amps,3); // the '3' after voltage allows you to display 3 digits after decimal point
 

//  auto voltageAmplitude = getAmplitudeOverOneSecond(analogInPin);
//  auto vMax = voltageAmplitude / 2.0;
//  Serial.print("vmax: ");
//  Serial.print(vMax);
//  
//  auto vrms = vMax * 0.707;
//  Serial.print(" | vrms: ");
//  Serial.print(vrms);
//  auto ampsRms = vrms / mVperAmp;
//  Serial.print(" | ampsrms: ");
//  Serial.print(ampsRms);
//  if (ampsRms <= 0.095) {
//    ampsRms = 0;
//  }
//  
//  const auto voltage = 220;
//  auto power = ampsRms * voltage;

auto ampsRms = 0;
auto voltage = 0;
auto power = 0;
  return ACReading(ampsRms, voltage, power);
}

//obsolete
ACReading ACS712::readAC_Old(int analogInPin) const
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
