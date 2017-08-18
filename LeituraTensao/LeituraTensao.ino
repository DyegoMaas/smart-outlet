#include "Relay5V.h"
#include "Utils.h"
#include "SensoresCorrente.h"
using namespace SensoresCorrente;

ACS712Class currentSensor = ACS712Class(_30A);
Relay5V relay = Relay5V(false); //TODO inverter a ligação física
const int ACSensorIn = A0;
const int Relay2Pin = 7;
auto ligado = true;

void setupPins()
{
	pinMode(ACSensorIn, INPUT);
	pinMode(Relay2Pin, OUTPUT);
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
	relay.turnOn(Relay2Pin);
	readAC();
	Serial.println();

	relay.turnOff(Relay2Pin);
	delay(3000);
	//ligado = !ligado;
}

////ACS712 Arduino Code
//float vcc = 0;
//
//void setup() {
//	Serial.begin(9600);
//}
//
//
//long readVcc() {
//	// Read 1.1V reference against AVcc
//	// set the reference to Vcc and the measurement to the internal 1.1V reference
//#if defined(__AVR_ATmega32U4__) || defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__)
//	ADMUX = _BV(REFS0) | _BV(MUX4) | _BV(MUX3) | _BV(MUX2) | _BV(MUX1);
//#elif defined (__AVR_ATtiny24__) || defined(__AVR_ATtiny44__) || defined(__AVR_ATtiny84__)
//	ADMUX = _BV(MUX5) | _BV(MUX0);
//#elif defined (__AVR_ATtiny25__) || defined(__AVR_ATtiny45__) || defined(__AVR_ATtiny85__)
//	ADMUX = _BV(MUX3) | _BV(MUX2);
//#else
//	ADMUX = _BV(REFS0) | _BV(MUX3) | _BV(MUX2) | _BV(MUX1);
//#endif
//
//	delay(2); // Wait for Vref to settle
//	ADCSRA |= _BV(ADSC); // Start conversion
//	while (bit_is_set(ADCSRA, ADSC)); // measuring
//
//	uint8_t low = ADCL; // must read ADCL first – it then locks ADCH
//	uint8_t high = ADCH; // unlocks both
//
//	long result = (high << 8) | low;
//
//	result = 1125300L / result; // Calculate Vcc (in mV); 1125300 = 1.1*1023*1000
//	return result; // Vcc in millivolts
//}
//
//
//void loop() {
//	vcc = readVcc() / 1000.0;
//	Serial.print("Vcc: ");
//	Serial.print(vcc);
//
//	long average = 0;
//	for (int i = 0; i < 100; i++) {
//		average = average + analogRead(A0);
//		delay(1);
//	}
//	average = average / 100;
//
//	float sensorValue = average * (5.0 / 1023.0);
//	Serial.print(" sense : ");
//	Serial.print(sensorValue, 3);
//
//	float acoffset = vcc / 2.0;
//	Serial.print(" offst : ");
//	Serial.print(acoffset, 3);
//
//	float sensitivity = 0.185 * (vcc / 5.0);
//	Serial.print(" sensi : ");
//	Serial.print(sensitivity, 4);
//
//	float amps = (sensorValue - acoffset) / sensitivity;
//	Serial.print(" Amperes : ");
//	Serial.print(amps);
//	Serial.println("…");
//	delay(1000);
//}












//#define CURRENT_SENSOR A0  // Define Analog input pin that sensor is attached
//
//float amplitude_current;      // Float amplitude current
//float effective_value;       // Float effective current
//
//void pins_init()
//{
//	pinMode(CURRENT_SENSOR, INPUT);
//}
//
//void setup()
//{
//	Serial.begin(9600);
//	pins_init();
//}
//
//
///*Function: Sample for 1000ms and get the maximum value from the S pin*/
//
//int getMaxValue()
//{
//	int sensorValue;    //value read from the sensor
//	int sensorMax = 0;
//	uint32_t start_time = millis();
//	while ((millis() - start_time) < 1000) //sample for 1000ms
//	{
//		sensorValue = analogRead(CURRENT_SENSOR);
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
//void loop()
//{
//	int sensor_max;
//	sensor_max = getMaxValue();
//	Serial.print("sensor_max = ");
//	Serial.println(sensor_max);
//
//	//the VCC on the Arduino interface of the sensor is 5v
//
//	amplitude_current = (float)(sensor_max - 512) / 1024 * 5 / 185 * 1000000; // for 5A mode,you need to modify this with 20 A and 30A mode;
//	effective_value = amplitude_current / 1.414;
//
//	//for minimum current=1/1024*5/185*1000000/1.414=18.7(mA)
//	//Only sinusoidal alternating current
//
//	Serial.println("The amplitude of the current is(in mA)");
//	Serial.println(amplitude_current, 1);
//
//	//Only one number after the decimal point
//
//	Serial.println("The effective value of the current is(in mA)");
//	Serial.println(effective_value, 1);
//}








