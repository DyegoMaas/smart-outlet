#include "SensoresCorrente.h"
#include "Utils.h"

namespace SensoresCorrente{
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
  	auto voltage = rawValue / 1024.0 * 5000; // mV
  	auto amps = (voltage - ACSoffset) / mVperAmp;
  	auto power = Utils::computePower(amps, voltage);
  	return DCReading(amps, voltage, power);
  }

  const int goodReadingsRequired = 3;
  int ACReadCount = 0;
  
  
  double calibrationValue = 0.0;
  double tempCalibrationValue = 0.0;
  int middleValue = 510;

  int goodReadings = 0;
  
  ACReading ACS712::readAC(int analogInPin) const
  {
    double sensorValue = 0.0;
    int numeroLeituras = 5000;  
    int lastMiddleValue = 0;

    if(goodReadings == goodReadingsRequired) 
      middleValue = calibrationValue / (numeroLeituras * goodReadings);
      
    for (int i = 0; i < numeroLeituras; i++) {
      int rawValue = analogRead(analogInPin);
      //Serial.println(rawValue);
      tempCalibrationValue += rawValue;

      int auxValue = rawValue - middleValue;

      // somam os quadrados das leituras.
      sensorValue += pow(auxValue, 2);
      delay(1);
    }



    Serial.print("middle: ");
    Serial.println(middleValue);
    
    Serial.print("somou: ");
    Serial.println(sensorValue);
    // finaliza o calculo da média quadratica e ajusta o valor lido para volts
    //double voltsporUnidade = 0.002737;
    double voltsporUnidade = 0.0048875855;// 5/1024;
    sensorValue = (sqrt(sensorValue / numeroLeituras)) * voltsporUnidade;
    Serial.print("calculou: ");
    Serial.println(sensorValue);
    double sensibility = mVperAmp;
    double amps = (sensorValue/sensibility);
    Serial.print("amps: ");
    Serial.println(amps);
   
    //tratamento para possivel ruido
    //O ACS712 para 30 Amperes é projetado para fazer leitura
    // de valores alto acima de 0.25 Amperes até 30.
    // por isso é normal ocorrer ruidos de até 0.20A
    //por isso deve ser tratado
    if(amps <= 0.095){
      if(goodReadings < goodReadingsRequired){
        calibrationValue += tempCalibrationValue;
        goodReadings++;
        Serial.print(goodReadings);
        Serial.println(" good readings so far");
      }       
      
      amps = 0;
    }

    if(goodReadings < goodReadingsRequired) {
      Serial.println("not calibrated yet");
      return ACReading(0, 220, 0);  
    }

    return ACReading(amps, 220, amps*220);
  }
}
