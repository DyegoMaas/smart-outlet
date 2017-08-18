// Relay5V.h

#ifndef _RELAY5V_h
#define _RELAY5V_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "Arduino.h"
#else
	#include "WProgram.h"
#endif

class Relay5V
{
public:
	explicit Relay5V(bool worksInLow) : worksInLow(worksInLow) {}
	void turnOn(int pin) const;
	void turnOff(int pin) const;
private:
	bool worksInLow = true; //relays are activated on LOW
};

#endif