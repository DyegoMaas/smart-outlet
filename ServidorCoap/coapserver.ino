/*
ESP-COAP Server
*/

#include <ESP8266WiFi.h>
#include "coap_server.h"
#include "Relay5V.h"

// CoAP server endpoint url callback
void callback_light(coapPacket *packet, IPAddress ip, int port, int obs);
void callback_toggle(coapPacket *packet, IPAddress ip, int port, int obs);
void callback_time(coapPacket *packet, IPAddress ip, int port, int obs);

coapServer coap;

//WiFi connection info
const char* ssid = "Dyego";
const char* password = "estreladamorte";

// LED STATE
int LED_PIN = 16;
bool LEDSTATE;

int GREEN_WIFI_LED_PIN = 15;
int RED_COAP_LED_PIN = 13;

int RELAY_PIN = 14;
Relay5V relay = Relay5V(true);


// CoAP server endpoint URL
void callback_light(coapPacket *packet, IPAddress ip, int port,int obs) {
	char time[50];
	sprintf(time, "Llight in time %lu", millis());
	Serial.print(time);

  // send response
  auto size = packet->payloadlen + 1;
  char *p = new char[size];
  memcpy(p, packet->payload, packet->payloadlen);
  p[packet->payloadlen] = NULL;
  Serial.println(p);
  
  String message(p);
  if (message.equals("0"))
  {
    digitalWrite(LED_PIN,LOW);
  }
  else if (message.equals("1"))
  {
    digitalWrite(LED_PIN,HIGH);
  } 
	char *isOn = (digitalRead(LED_PIN) > 0)? ((char *) "on") :((char *) "off");
	if(obs==1)
		coap.sendResponse(isOn);
	else
		coap.sendResponse(ip,port, isOn);

	Serial.print("light end");
}

void callback_toggle(coapPacket *packet, IPAddress ip, int port,int obs) {
  Serial.println("Toggle");

  // send response
  auto size = packet->payloadlen + 1;
  char *p = new char[size];
  memcpy(p, packet->payload, packet->payloadlen);
  p[packet->payloadlen] = NULL;
  Serial.println(p);

  String message(p);
  if (message.equals("0"))
  {
	  relay.turnOff(RELAY_PIN);
  }
  else if (message.equals("1"))
  {
	  relay.turnOn(RELAY_PIN);
  }

  char *isOn = (digitalRead(RELAY_PIN) > 0) ? ((char *) "on") : ((char *) "off");
  if (obs == 1)
	  coap.sendResponse(isOn);
  else
	  coap.sendResponse(ip, port, isOn);

  Serial.print("toggle end");
}

void callback_time(coapPacket *packet, IPAddress ip, int port, int obs) {
	Serial.println("Time");

	// send response
	auto size = packet->payloadlen + 1;
	char *p = new char[size];
	memcpy(p, packet->payload, packet->payloadlen);
	p[packet->payloadlen] = NULL;
	Serial.println(p);
	
	char time[50];
	sprintf(time, "%lu", millis());
	Serial.print("Time: ");
	Serial.println(time);
	if (obs == 1)
		coap.sendResponse(time);
	else
		coap.sendResponse(ip, port, time);

	Serial.print("time end");
}

void setup() {
	pinMode(LED_PIN, OUTPUT);
	pinMode(GREEN_WIFI_LED_PIN, OUTPUT);
	pinMode(RED_COAP_LED_PIN, OUTPUT);
	
	digitalWrite(LED_PIN, HIGH);
	digitalWrite(GREEN_WIFI_LED_PIN, HIGH),
	digitalWrite(RED_COAP_LED_PIN, HIGH),

  yield();
  //serial begin
  Serial.begin(115200);

  WiFi.begin(ssid, password);
  Serial.println(" ");

  // Connect to WiFi network
  Serial.println();
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    //delay(500);
    yield();
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected");
  // Print the IP address
  Serial.println(WiFi.localIP());

  // READY INDICATION  
  digitalWrite(GREEN_WIFI_LED_PIN, HIGH),

  digitalWrite(LED_PIN, LOW);
  delay(250);
  digitalWrite(LED_PIN, HIGH);
  delay(250);
  digitalWrite(LED_PIN, LOW);
  delay(250);
  digitalWrite(LED_PIN, HIGH);
  delay(250);
  digitalWrite(LED_PIN, LOW);
  LEDSTATE = true;

  // Relay state
  pinMode(RELAY_PIN, OUTPUT);
  relay.turnOff(RELAY_PIN);


  // add server url endpoints.
  // can add multiple endpoint urls.

  //coap.server(callback_light, "light");
  coap.server(callback_toggle, "toggle");
  coap.server(callback_time, "time");
  
  // start coap server/client
  coap.start();
  //digitalWrite(RED_COAP_LED_PIN, LOW);
  Serial.println("CoAP server online");
  // coap.start(5683);
}

int counter = 0;
void loop() {
	Serial.print("coap loop started");
	Serial.println(counter);
	coap.loop();
	Serial.print("coap loop finished");
	Serial.println(counter++);
	delay(1000);
}
