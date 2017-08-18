#include "Relay5V.h"
#include "Utils.h"
#include "SensoresCorrente.h"
using namespace SensoresCorrente;

const int ACSensorIn = A0;
const int Relay1Pin = 7;
ACS712 currentSensor = ACS712(_30A);
Relay5V relay = Relay5V(true);

void setupPins()
{
	pinMode(ACSensorIn, INPUT);
	pinMode(Relay1Pin, OUTPUT);
}

void setup()
{
	setupPins();
	Serial.begin(9600);
}

void readAC()
{
	auto leitura = currentSensor.readAC(ACSensorIn);
	Serial.print("potencia: ");
	Serial.print(leitura.power);

	Serial.print("	corrente: ");
	Serial.print(leitura.amps);

	Serial.print("	tensao: ");
	Serial.print(leitura.voltage);
}

void loop()
{
	relay.toggle(Relay1Pin);
	readAC();
	Serial.println();

	relay.toggle(Relay1Pin);
	delay(3000);
}