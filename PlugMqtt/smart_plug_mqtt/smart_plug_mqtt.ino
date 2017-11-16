#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include "Relay5V.h"
#include "SensoresCorrente.h"
#include "TickerScheduler.h"
#include "EEPROM_Manager.h"
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
EEPROM_Manager eepromManager;

int WIFI_LED = D7;
int RELAY_PIN = D5;
Relay5V relay = Relay5V(true);

int AC_SENSOR_PIN = A0;
ACS712 currentSensor = ACS712(_30A);

bool hasCredentials = false;
String id = "";
long lastMessageTime = 0;

void reportConsumption() {  
  auto timeSinceLastConsumptionReport = millis() - lastMessageTime;  
  if (timeSinceLastConsumptionReport >= 5000) {
    auto reading = currentSensor.readAC(AC_SENSOR_PIN);

    char power[10];
    dtostrf(reading.power, 7, 2, power);
    
    Serial.print("lido ");
    Serial.print(reading.power);
    
    Serial.print(" | publicando ");
    Serial.println(power);

    String payload = id;
    payload += '|';
    payload += power;
    
    MQTT.publish("/smart-plug/consumption", (char *)payload.c_str());
    lastMessageTime = millis();
  }
}

void initCredentials() {
  Serial.println("INITILIASING CREDENTIALS: ");
  
  hasCredentials = eepromManager.hasData();
  id = eepromManager.loadId();
  Serial.print("loaded credentials:");
  Serial.println(id);  
}

void setup() {
	initPins();
	initSerial();
 
  eepromManager.begin();
  initCredentials();
  
	initWiFi();
	initMQTT();
}

void loop() {
	recconectWiFi();
	if (!MQTT.connected()) {
		reconnectMQTT();
	}	

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
  Serial.print("TURNING ON ");
  Serial.println(id);
  relay.turnOn(RELAY_PIN);
  sendConfirmationOfRelayStateChange();
}

void turnOff() {
  Serial.print("TURNING OFF ");
  Serial.println(id);
  relay.turnOff(RELAY_PIN);
  sendConfirmationOfRelayStateChange();
}

void sendConfirmationOfRelayStateChange() {
  auto isOn = relay.isOn(RELAY_PIN) ? "on": "off";
  String payload = id;
  payload += '|';
  payload += isOn;

	MQTT.publish("/smart-plug/new-state", (char *)payload.c_str());
}

void mqtt_callback(char* topic, byte* payload, unsigned int length) {
	String message;
	for (int i = 0; i < length; i++) {
		char c = (char)payload[i];
		message += c;
	}
 
	auto topicString = String(topic);
	Serial.println("Topic => " + topicString + " | Value => " + message);

  if (topicString == "/smart-plug/clean-identity") {
    Serial.println("identity cleared");
    eepromManager.clear(); 
    initCredentials();   
    return;
  } else if (topicString == "/smart-plug/activate") {
    if (!hasCredentials) {
      Serial.print("assuming new identity! ");
      auto newId = message;
      Serial.println(newId);      
      eepromManager.saveId(newId);   
      initCredentials();  
      return; 
    }
    else { 
      Serial.print("already have identity! ");
      Serial.println(id);
      return;
    }
  } 

  if (!hasCredentials) {
    Serial.print("No credentials for topic ");
    Serial.println(topic);
    return;
  }

  auto delimiterIndex = message.indexOf('|');
  if (delimiterIndex == -1) {
    Serial.print("No id on topic ");
    Serial.println(topic);
    return;
  }

  auto targetId = message.substring(0, delimiterIndex);
  auto data = message.substring(delimiterIndex + 1);

  Serial.print("Message is intented for ");
  Serial.print(targetId);
  if (id != targetId) {
    Serial.print(", not for me: ");
    Serial.println(id);
    return;
  }
  else {
    Serial.println("it's for me!");
  }
  Serial.print("Message content is ");
  Serial.println(data);
  
  if (topicString == "/smart-plug/state") {
		if (data == "turn-on") {
			turnOn();			
		}
		else if (data == "turn-off") {
			turnOff();
		}  
    else {
      Serial.println("nothing to do related to state...");  
    }
	} else if (topicString == "/smart-plug/schedule-on") {
    
    auto intervaloMilisegundos = data.toInt();
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

    auto intervaloMilisegundos = data.toInt();
    Serial.print("desligando em ");
    Serial.print(intervaloMilisegundos);
    Serial.println("ms");

    scheduler.remove(1);
    scheduler.remove(2);
    
    scheduler.add(2, intervaloMilisegundos, [&](void*) { 
      Serial.println("OFF");
      turnOff();
      scheduler.remove(2);
    }, nullptr, false); 
  }
  else {
    Serial.println("not implemented topic");
  }
	
	Serial.flush();
}

void reconnectMQTT() {
	while (!MQTT.connected()) {
		Serial.println("Tentando se conectar ao Broker MQTT: " + String(BROKER_MQTT));
		if (MQTT.connect("ESP8266-ESP12-E")) {
			Serial.println("Conectado ao broker MQTT!");

      Serial.println("subscribed to /smart-plug/clean-identity");
      MQTT.subscribe("/smart-plug/clean-identity");

      Serial.println("subscribed to /smart-plug/activate");
      MQTT.subscribe("/smart-plug/activate");
			
			Serial.println("subscribed to /smart-plug/state");
			MQTT.subscribe("/smart-plug/state");

      Serial.println("subscribed to /smart-plug/schedule-on");
      MQTT.subscribe("/smart-plug/schedule-on");

      Serial.println("subscribed to /smart-plug/schedule-off");
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
