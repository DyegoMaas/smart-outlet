#if defined(ARDUINO) && ARDUINO >= 100
  #include "Arduino.h"
#else
  #include "WProgram.h"
#endif

#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <DNSServer.h>

class WifiCredentials {
public: 
  WifiCredentials(String ssid, String password);
  String getSSID();
  String getPassword();
private:
  String _ssid;
  String _password;
};

class WiFiManagerUltraSimple {
public:
  WiFiManagerUltraSimple();
  void connect();
    
  //called when settings have been changed and connection was successful
  void setSaveConfigCallback( void (*func)(WifiCredentials) );

private:  
  static void handleWifiSave();
  static void (*savecallback)(WifiCredentials); 
};
