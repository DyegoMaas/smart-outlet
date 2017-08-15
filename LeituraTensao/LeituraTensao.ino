#include "SensoresCorrente.h"
using namespace SensoresCorrente;

ACS712Class currentSensor = ACS712Class(_30A);
const int sensorIn = A0;
const int greenLed = 13;

void setup()
{
	pinMode(greenLed, OUTPUT);
	digitalWrite(greenLed, HIGH);

	pinMode(sensorIn, INPUT);

	Serial.begin(9600);
}

float computePower(float amps, float voltage)
{
	return amps * (voltage * 1000);
}

void loop()
{
	auto leitura = currentSensor.readAC(sensorIn);
	auto potencia = computePower(leitura.amps, leitura.voltage);
	Serial.print("potencia: ");
	Serial.println(potencia);
}
