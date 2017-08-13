const int sensorIn = A0;

// use 100 for 20A Module and 185 for 5A Module
const auto mVperAmp = 66; //saída proporcional
const auto ACSoffset = 2500;

void setup()
{
	pinMode(13, OUTPUT);
	digitalWrite(13, HIGH);

	pinMode(sensorIn, INPUT);
	
	Serial.begin(9600);
}

double lerCorrentDireta()
{
	auto rawValue = analogRead(sensorIn);
	auto tensao = rawValue / 1024.0 * 5000; // Gets you mV
	auto amps = (tensao - ACSoffset) / mVperAmp;
	return amps;
}

float getVPP()
{
	auto maxValue = 0;          // store max value here
	auto minValue = 1024;          // store min value here

	uint32_t start_time = millis();
	while ((millis() - start_time) < 1000) //sample for 1 Sec
	{
		auto readValue = analogRead(sensorIn);
	/*	Serial.print("readValue: ");
		Serial.println(readValue);*/
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
	float result = ((maxValue - minValue) * 5.0) / 1024.0;
	return result;
}

float lerCorrentAlternada()
{
	auto voltage = getVPP();
	Serial.print("voltage: "); 
	Serial.println(voltage);

	auto VRMS = (voltage / 2.0) * 0.707;

	Serial.print("VRMS: ");
	Serial.println(VRMS);

	auto ampsRms = (VRMS * 1000) / mVperAmp;
	return ampsRms;
}

void loop()
{
	auto leitura = lerCorrentAlternada();
	Serial.print("Amps RMS: ");
	Serial.println(leitura);
}
