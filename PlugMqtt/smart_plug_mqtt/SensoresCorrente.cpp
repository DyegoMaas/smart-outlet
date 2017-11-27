#include "SensoresCorrente.h"
#include "Utils.h"

namespace SensoresCorrente{
  ACS712::ACS712(ModuleType moduleType)
  {
  	switch (moduleType)
  	{
  		case _30A: mVperAmp = 66; break;
  		case _20A: mVperAmp = 100; break;
  		case _5A:
  		default: mVperAmp = 185; break;
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
  
  //ACSvoltage = 2800 //é o que está vindo
  //ACSoffset = 1400
  //mVperAmp = 66
  ACReading ACS712::readAC(int analogInPin) const
  {  
    auto sensorValue = 0.0;
    int numeroLeituras = 1000;  
    for (int i = 0; i < numeroLeituras; i++) {
      int rawValue = analogRead(analogInPin);
      // int auxValue = rawValue - 510; //seria assim num arduino
      int auxValue = rawValue - 453; //seria assim num arduino
      // auxValue = map(auxValue, 0, 1024, 4, 910);
    //   auxValue = map(auxValue, 4, 910, 0, 1024); //ajustando para a escala do arduino
      
      // somam os quadrados das leituras.
      sensorValue += pow(auxValue, 2);
      delay(1);
    }
   
    // finaliza o calculo da média quadratica e ajusta o valor lido para volts
    auto voltsporUnidade = 0.002737;
    // auto voltsporUnidade = 0.0048828125;// 5%1024;
    sensorValue = (sqrt(sensorValue / numeroLeituras)) * voltsporUnidade;
	auto sensibility = map(mVperAmp, 0, 1024, 4, 910);
    auto amps = (sensorValue/sensibility);
   
    //tratamento para possivel ruido
    //O ACS712 para 30 Amperes é projetado para fazer leitura
    // de valores alto acima de 0.25 Amperes até 30.
    // por isso é normal ocorrer ruidos de até 0.20A
    //por isso deve ser tratado
    if(amps <= 0.095){
      amps = 0;
    }


    // //auto scaledValue = changeScale(0, 1024, 4, 910, rawValue);
    // //auto scaledValue2 = changeScale(4, 910, -30, 30, scaledValue);
    // auto voltage = (scaledValue / 910) * ACSVoltage;
  
    // auto vrms = (voltage - ACSoffset) * 0.707; 
    // double amps = (vrms / mVperAmp);
    // if (amps < 0)
    // 	amps *= -1;
    // if (amps < 0.05)
    //   amps = 0;
  
  	// Serial.print("Ingenuous Amp calculus = " ); // shows pre-scaled value 
  	// Serial.println(scaledValue2); 
  	
  
    Serial.print("Raw Value = " ); // shows pre-scaled value 
    Serial.print(analogRead(analogInPin)); 
    // Serial.print("\t mV (("); // shows the voltage measured 
    // Serial.print(voltage, 3); // the '3' after voltage allows you to display 3 digits after decimal point
    // Serial.print(")\t - ACOffset ("); // shows the voltage measured 
    // Serial.print(ACSoffset, 3); // the '3' after voltage allows you to display 3 digits after decimal point
    // Serial.print(")) / mVperAmp ("); // shows the voltage measured 
    // Serial.print(mVperAmp, 3); // the '3' after voltage allows you to display 3 digits after decimal point
    
    // Serial.print(") = "); // shows the voltage measured 
    Serial.print("\t"); // shows the voltage measured 
    Serial.print(amps, 3); // the '3' after voltage allows you to display 3 digits after decimal point
    Serial.println("A"); // shows the voltage measured 
   
  
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
  
  // auto ampsRms = 0;
  //auto voltage = 0;
  // auto power = 0;
    return ACReading(amps, 220, amps*220);
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
}
