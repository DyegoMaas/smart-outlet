#include "EEPROM_Manager.h"

Credentials::Credentials(String ssid, String password) {
  _ssid = ssid;
  _password = password;  
}

String Credentials::getSSID() {
  return _ssid;
}

String Credentials::getPassword() {
  return _password;
}

void EEPROM_Manager::begin() {
  EEPROM.begin(512);
}

void EEPROM_Manager::resetCredentials() {
  EEPROM.write(0, (uint8_t)'f');  
}

void EEPROM_Manager::writeCredentials(String ssid, String password) {  
  int i = 0;
  EEPROM.write(i++, (uint8_t)'t');  

  Serial.println("Writing ssid to EEPROM"); 
  auto ssidSize = ssid.length();  
  for (int j = 0; j < ssidSize; i++, j++) {
    EEPROM.write(i, (uint8_t)ssid[j]);
    Serial.print(j); Serial.print("="); Serial.print(ssid[j]); Serial.print("|"); Serial.print((uint8_t)ssid[j]); Serial.print(" ");
    yield();
  }
  EEPROM.write(i++, '\0');

  Serial.println("Writing password to EEPROM"); 
  auto passwordSize = password.length();  
  for (int j = 0; j < passwordSize; i++, j++) {
    EEPROM.write(i, (uint8_t)password[j]);
    Serial.print(j); Serial.print("="); Serial.print(password[j]); Serial.print("|"); Serial.print((uint8_t)password[j]); Serial.print(" ");
    yield();
  }  
  EEPROM.write(i++, '\0');
 
  EEPROM.commit();
}

Credentials EEPROM_Manager::readCredentials() {
  int i = 1; //because of the indicator

  Serial.println("Reading ssid:");
  String ssid = "";
  while(true) {
    auto value = EEPROM.read(i++);
    char c = char(value);
    Serial.print(value); Serial.print("@"); Serial.print(i); Serial.print("=>"); Serial.print(c); Serial.print(" ");
    if(c == '\0')
      break;

    ssid += c;
  }

  Serial.println();
  Serial.println("Reading password:");
  String password = "";
  while(true) {
    auto value = EEPROM.read(i++);
    char c = char(value);
    Serial.print(value); Serial.print("@"); Serial.print(i-1); Serial.print("=>"); Serial.print(c); Serial.print(" ");
    Serial.print(c); Serial.print(" ");
    if(c == '\0')
      break;

    password += c;
  }
  Serial.print(ssid);
  Serial.print(" | ");
  Serial.print(password);
  return Credentials(ssid, password);
}

bool EEPROM_Manager::hasCredentials() {
  auto has = char(EEPROM.read(0)) == 't';
  return has;    
}
