#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include "Relay5V.h"

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

int WIFI_LED = D7;
int RELAY_PIN = D5;
Relay5V relay = Relay5V(true);

long lastMessageTime = 0;

void reportConsumption() {
  auto timeSinceLastConsumptionReport = millis() - lastMessageTime;  
  if (timeSinceLastConsumptionReport >= 1000) {
    Serial.print("publicando 25.7");
    MQTT.publish("/smart-plug/consumption", (char *)"25.7");
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
}

void initPins() {
  pinMode(WIFI_LED, OUTPUT);
  digitalWrite(WIFI_LED, LOW);
  
  pinMode(RELAY_PIN, OUTPUT);
  relay.turnOff(RELAY_PIN);
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
}

void turnOff() {
   relay.turnOff(RELAY_PIN);
}

void sendConfirmationOfRelayActivation() {
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
	Serial.println("Topic => " + topicString + " | Value => " + String(message));
  
	if (topicString == "/smart-plug/state") {
		if (message == "turn-on") {
			turnOn();
			sendConfirmationOfRelayActivation();
		}
		else if (message == "turn-off") {
			turnOff();
			sendConfirmationOfRelayActivation();
		}  
	}
	
	Serial.flush();
}

void reconnectMQTT() {
	while (!MQTT.connected()) {
		Serial.println("Tentando se conectar ao Broker MQTT: " + String(BROKER_MQTT));
		if (MQTT.connect("ESP8266-ESP12-E")) {
			Serial.println("Conectado");
      
			MQTT.subscribe("/smart-plug/state");
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
