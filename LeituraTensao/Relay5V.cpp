// 
// 
// 

#include "Relay5V.h"

void Relay5V::turnOn(int pin) const
{
	digitalWrite(pin, worksInLow ? LOW : HIGH);
}

void Relay5V::turnOff(int pin) const
{
	digitalWrite(pin, worksInLow ? HIGH : LOW);
}
