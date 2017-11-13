#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include "Relay5V.h"
#include "SensoresCorrente.h"
#include "TickerScheduler.h"
using namespace SensoresCorrente;

const char* SSID = "Dyego"; 
const char* PASSWORD = "estreladamorte"; 

const char* BROKER_MQTT = "iot.eclipse.org"; 
int BROKER_PORT = 1883; 

void initPins();
void initSerial();
void initWiFi();
void initMQTT();

WiFiClient espClient;
PubSubClient MQTT(espClient);
TickerScheduler scheduler(10);

int WIFI_LED = D7;
int RELAY_PIN = D5;
Relay5V relay = Relay5V(true);

int AC_SENSOR_PIN = A0;
ACS712 currentSensor = ACS712(_30A);

long lastMessageTime = 0;

void reportConsumption() {
  auto timeSinceLastConsumptionReport = millis() - lastMessageTime;  
  if (timeSinceLastConsumptionReport >= 1000) {
    auto reading = currentSensor.readAC(AC_SENSOR_PIN);

    char power[10];
    dtostrf(reading.power, 7, 2, power);
    //sprintf(power, "%7.2f", reading.power);
    
    Serial.print("lidox ");
    Serial.print(reading.power);
    
    Serial.print(" | publicando ");
    Serial.println(power);
    
    MQTT.publish("/smart-plug/consumption", power);
    lastMessageTime = millis();
  }
}

void setup() {
	initPins();
	initSerial();
	initWiFi();
	initMQTT();
}

void loop() {
	if (!MQTT.connected()) {
		reconnectMQTT();
	}
	recconectWiFi();

  reportConsumption();  
	MQTT.loop();
  scheduler.update();
}

void initPins() {
  pinMode(WIFI_LED, OUTPUT);
  digitalWrite(WIFI_LED, LOW);
  
  pinMode(RELAY_PIN, OUTPUT);
  relay.turnOff(RELAY_PIN);

  pinMode(AC_SENSOR_PIN, INPUT);
}

void initSerial() {
	Serial.begin(115200);
}

void initWiFi() {
	delay(10);
	Serial.println("Conectando-se em: " + String(SSID));

	WiFi.begin(SSID, PASSWORD);
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");
	}
	Serial.println();
	Serial.print("Conectado na Rede " + String(SSID) + " | IP => ");
	Serial.println(WiFi.localIP());
 
  digitalWrite(WIFI_LED, HIGH);
}

void initMQTT() {
	Serial.println("Initializing MQTT");
	MQTT.setServer(BROKER_MQTT, BROKER_PORT);
	MQTT.setCallback(mqtt_callback);

	lastMessageTime = millis();
}

void turnOn() {
	 relay.turnOn(RELAY_PIN);
	 sendConfirmationOfRelayStateChange();
}

void turnOff() {
	 relay.turnOff(RELAY_PIN);
	 sendConfirmationOfRelayStateChange();
}

void sendConfirmationOfRelayStateChange() {
  auto isOn = relay.isOn(RELAY_PIN) ? "on": "off";
	MQTT.publish("/smart-plug/new-state", (char *)isOn);
}

void mqtt_callback(char* topic, byte* payload, unsigned int length) {
	String message;
	for (int i = 0; i < length; i++) {
		char c = (char)payload[i];
		message += c;
	}
 
	auto topicString = String(topic);
	Serial.println("Topic => " + topicString + " | Value => " + message);
  
	if (topicString == "/smart-plug/state") {
		if (message == "turn-on") {
			turnOn();			
		}
		else if (message == "turn-off") {
			turnOff();
		}  
	} else if (topicString == "/smart-plug/schedule-on") {
    
    auto intervaloMilisegundos = message.toInt();
    Serial.print("ligando em ");
    Serial.print(intervaloMilisegundos);
    Serial.println("ms");

    scheduler.remove(1);
    scheduler.remove(2);
    
	  scheduler.add(1, intervaloMilisegundos, [&](void*) { 
      Serial.println("ON");
      turnOn();
      scheduler.remove(1);
    }, nullptr, false);
    
  } else if (topicString == "/smart-plug/schedule-off") {

    auto intervaloMilisegundos = message.toInt();
    Serial.print("desligando em ");
    Serial.print(intervaloMilisegundos);
    Serial.println("ms");

    scheduler.remove(1);
    scheduler.remove(2);
    
    auto funfou = scheduler.add(2, intervaloMilisegundos, [&](void*) { 
      Serial.println("OFF");
      turnOff();
      scheduler.remove(2);
    }, nullptr, false);
    Serial.print("funfou = ");
    Serial.println(funfou);    
  }
	
	Serial.flush();
}

void reconnectMQTT() {
	while (!MQTT.connected()) {
		Serial.println("Tentando se conectar ao Broker MQTT: " + String(BROKER_MQTT));
		if (MQTT.connect("ESP8266-ESP12-E")) {
			Serial.println("Conectado ao broker MQTT!");
      
			MQTT.subscribe("/smart-plug/state");
      MQTT.subscribe("/smart-plug/schedule-on");
      MQTT.subscribe("/smart-plug/schedule-off");
		}
		else {
			Serial.println("Falha ao Reconectar");
			Serial.println("Tentando se reconectar em 2 segundos");
			delay(2000);
		}
	}
}

void recconectWiFi() {
	while (WiFi.status() != WL_CONNECTED) {
		delay(100);
		Serial.print(".");
	}
}
