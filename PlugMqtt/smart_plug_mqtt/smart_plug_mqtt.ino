#include <ESP8266WiFi.h> //ESP8266 Core WiFi Library (you most likely already have this in your sketch)
#include <DNSServer.h>            //Local DNS Server used for redirecting all requests to the configuration portal
#include <ESP8266WebServer.h>     //Local WebServer used to serve the configuration portal
#include <PubSubClient.h>
#include <EEPROM.h>

#include "WiFiManager.h"    //https://github.com/tzapu/WiFiManager WiFi Configuration Magic
#include "Relay5V.h"
#include "EEPROM_Manager.h"
#include "SensoresCorrente.h"
using namespace SensoresCorrente;

const char* SSID = "temp"; 
const char* PASSWORD = "temp"; 

const char* BROKER_MQTT = "iot.eclipse.org"; 
int BROKER_PORT = 1883; 

void initPins();
void initSerial();
void initWiFi();
void initMQTT();
void initEEPROM();
bool mustAskWifiCredentials();
void loadWifiCredentials();

WiFiClient espClient;
PubSubClient MQTT(espClient);
EEPROM_Manager eepromManager;

int RESET_PIN = D2;
int TRIGGER_PIN = D8;
int WIFI_LED_PIN = D7;
int RELAY_PIN = D5;
Relay5V relay = Relay5V(true);

int AC_SENSOR_PIN = A0;
ACS712 currentSensor = ACS712(_30A);

long lastMessageTime = 0;
bool reseting = false;

void reportConsumption() {
  auto timeSinceLastConsumptionReport = millis() - lastMessageTime;  
  if (timeSinceLastConsumptionReport >= 1000) {
    auto reading = currentSensor.readAC(AC_SENSOR_PIN);

    char power[10];
    dtostrf(reading.power, 7, 2, power);
    
    Serial.print("lido ");
    Serial.print(reading.power);
    
    Serial.print(" | publicando ");
    Serial.println(power);
    
    MQTT.publish("/smart-plug/consumption", power);
    lastMessageTime = millis();
  }
}

void resetar() {
  Serial.println("Reiniciando em 2s..");
  delay(2000);
  digitalWrite(D2, LOW);  
}

void setup() {
  digitalWrite(RESET_PIN, HIGH);  
  initPins();
  initSerial();
  Serial.println("SETUP");

  WiFi.disconnect();
  initEEPROM();
  if (mustAskWifiCredentials())
    return;
  else {
    loadWifiCredentials();  
  }
   
	initWiFi();
	initMQTT();
}

void saveCredentials(WifiCredentials credentials) {
  Serial.println("credenciais: ");
  Serial.print(credentials.getSSID());
  Serial.print(" / ");
  Serial.println(credentials.getPassword());

  auto ssid = credentials.getSSID();
  auto password = credentials.getPassword();
  eepromManager.writeCredentials(ssid, password);
  auto readCredentials = eepromManager.readCredentials();
  Serial.println("What was read?");
  Serial.println(readCredentials.getSSID());
  Serial.println(readCredentials.getPassword());

  resetar();
}

void askWifiCredentials() {
  WiFi.disconnect();
  Serial.println("resetando...");
  digitalWrite(WIFI_LED_PIN, LOW);
  delay(250);
  digitalWrite(WIFI_LED_PIN, HIGH);
  delay(250);
  digitalWrite(WIFI_LED_PIN, LOW);
  delay(250);
  digitalWrite(WIFI_LED_PIN, HIGH);
  delay(250);
  digitalWrite(WIFI_LED_PIN, LOW);
  
  //WiFiManager
  //Local intialization. Once its business is done, there is no need to keep it around
  WiFiManager wifiManager;

  wifiManager.setSaveConfigCallback(saveCredentials);
  
  //reset settings - for testing
  //wifiManager.resetSettings();
  
  //sets timeout until configuration portal gets turned off
  //useful to make it all retry or go to sleep
  //in seconds
  wifiManager.setTimeout(60);
  
  //it starts an access point with the specified name
  //here  "AutoConnectAP"
  //and goes into a blocking loop awaiting configuration
  
  //WITHOUT THIS THE AP DOES NOT SEEM TO WORK PROPERLY WITH SDK 1.5 , update to at least 1.5.1
  //WiFi.mode(WIFI_STA);

  Serial.println("Subindo portal de configuração!");
  if (!wifiManager.startConfigPortal("OnDemandAP")) {
    Serial.println("failed to connect and hit timeout");
    delay(3000);
    //reset and try again, or maybe put it to deep sleep
    //ESP.reset();
    //ESP.restart();
    resetar();
    return;
//    delay(5000);
  }
  
  //if you get here you have connected to the WiFi
  Serial.println("Open Plug IP!");
}

bool mustReset() {
  digitalRead(TRIGGER_PIN) == HIGH;
}

bool mustAskWifiCredentials() {
  return mustReset() || !eepromManager.hasCredentials();
}

void loadWifiCredentials() {
  auto credentials = eepromManager.readCredentials();  
  SSID = credentials.getSSID().c_str();
  PASSWORD = credentials.getPassword().c_str();
  Serial.println("New credentials:");
  Serial.println(SSID);
  Serial.println(PASSWORD);
}

void loop() {
  if (mustAskWifiCredentials()) {   
    reseting = true; 
    eepromManager.resetCredentials();
    askWifiCredentials();  
  }
  
  if (reseting) {
    Serial.println("skipping");
    delay(100);
    return;
  }
	if (!MQTT.connected()) {
		reconnectMQTT();
	}
	recconectWiFi();

  reportConsumption();  
	MQTT.loop();
}

void initPins() {
  pinMode(WIFI_LED_PIN, OUTPUT);
  digitalWrite(WIFI_LED_PIN, LOW);
  
  pinMode(RELAY_PIN, OUTPUT);
  relay.turnOff(RELAY_PIN);

  pinMode(AC_SENSOR_PIN, INPUT);

  pinMode(TRIGGER_PIN, INPUT);
}

void initSerial() {
	Serial.begin(115200);
}

void initEEPROM() {
  EEPROM.begin(512);
}

void initWiFi() {
	delay(10);
	Serial.println("Conectando-se em: " + String(SSID));

	WiFi.begin(SSID, PASSWORD);
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.print(".");

    if (mustReset())
      return;
	}
	Serial.println();
	Serial.print("Conectado na Rede " + String(SSID) + " | IP => ");
	Serial.println(WiFi.localIP());
 
  digitalWrite(WIFI_LED_PIN, HIGH);
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
