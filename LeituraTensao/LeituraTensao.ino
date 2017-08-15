#include "Utils.h"
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

void loop()
{
	auto leitura = currentSensor.readAC(sensorIn);
	Serial.print("potencia: ");
	Serial.println(leitura.power);
}
