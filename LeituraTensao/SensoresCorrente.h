#ifndef _SENSORESCORRENTE_h
#define _SENSORESCORRENTE_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "Arduino.h"
#else
	#include "WProgram.h"
#endif

namespace SensoresCorrente 
{
	enum ModuleType
	{
		_5A = 5,
		_20A = 20,
		_30A = 30
	};

	struct ACReading
	{
		float amps;
		float voltage;
		ACReading(float current, float voltage) 
			: amps(current), voltage(voltage) {}
	};

	struct DCReading
	{
		float amps;
		float voltage;
		DCReading(float current, float voltage) 
			: amps(current), voltage(voltage) {}
	};

	class ACS712Class
	{
		public:
			explicit ACS712Class(ModuleType moduleType);
			DCReading readDC(int analogInPin) const;
			ACReading readAC(int analogInPin) const;

		private:
			float mVperAmp = 66; // use 100 for 20A Module and 185 for 5A Module
			const float ACSoffset = 2500;
	};
}

#endif