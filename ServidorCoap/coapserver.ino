/*
ESP-COAP Server
*/

#include <ESP8266WiFi.h>
#include "coap_server.h"
#include "Relay5V.h"

// CoAP server endpoint url callback
void callback_light(coapPacket *packet, IPAddress ip, int port, int obs);
void callback_toggle(coapPacket *packet, IPAddress ip, int port, int obs);

coapServer coap;

//WiFi connection info
const char* ssid = "Dyego";
const char* password = "estreladamorte";

// LED STATE
bool LEDSTATE;

int RELAY_PIN = 14;
Relay5V relay = Relay5V(RELAY_PIN);


// CoAP server endpoint URL
void callback_light(coapPacket *packet, IPAddress ip, int port,int obs) {
  Serial.println("light");

  // send response
  auto size = packet->payloadlen + 1;
  char *p = new char[size];
  memcpy(p, packet->payload, packet->payloadlen);
  p[packet->payloadlen] = NULL;
  Serial.println(p);
  
  String message(p);

  if (message.equals("0"))
  {
    digitalWrite(16,LOW);
  }
  else if (message.equals("1"))
  {
    digitalWrite(16,HIGH);
  } 
	char *isOn = (digitalRead(16) > 0)? ((char *) "on") :((char *) "off");
	if(obs==1)
		coap.sendResponse(isOn);
	else
		coap.sendResponse(ip,port, isOn);
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
	  digitalWrite(16, LOW);
  }
  else if (message.equals("1"))
  {
	  digitalWrite(16, HIGH);
  }

  char *isOn = (digitalRead(16) > 0) ? ((char *) "on") : ((char *) "off");
  if (obs == 1)
	  coap.sendResponse(isOn);
  else
	  coap.sendResponse(ip, port, isOn);
}


void setup() {
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

  // LED State
  pinMode(16, OUTPUT);
  digitalWrite(16, HIGH);
  LEDSTATE = true;

  pinMode(5, OUTPUT);
  digitalWrite(5, HIGH);


  // add server url endpoints.
  // can add multiple endpoint urls.

  coap.server(callback_light, "light");
  coap.server(callback_light, "toggle");
  
  // start coap server/client
  coap.start();
  // coap.start(5683);
}

void loop() {
  coap.loop();
  delay(1000);
}
