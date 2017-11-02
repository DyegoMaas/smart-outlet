#include <EEPROM.h>

#if defined(ARDUINO) && ARDUINO >= 100
    #include "Arduino.h"
#else
    #include "WProgram.h"
#endif

class Credentials {
public: 
  Credentials(String ssid, String password);
  String getSSID();
  String getPassword();
private:
  String _ssid;
  String _password;
};

struct Credentials2 {
  String ssid;
  String password;
};

class EEPROM_Manager {
  public:
    void begin();
    void resetCredentials();
    void writeCredentials(String ssid, String password);
    bool hasCredentials();
    Credentials readCredentials();

  private:
    const int _size = 96;
};
