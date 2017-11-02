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
  EEPROM.write(0, 0);
  EEPROM.write(1, 0);
  EEPROM.write(2, 0);
  EEPROM.write(3, 0);
}

void EEPROM_Manager::writeCredentials(String ssid, String password) {

//  Credentials2 credentials = {
//    ssid,
//    password
//  };

//  EEPROM.put(0, credentials);
//  EEPROM.commit();
  Serial.println("Credentials saved");


  int i = 0;

  EEPROM.write(i++, 0); //indicates that there is a 
  EEPROM.write(i++, 1); //indicates that there is a 
  EEPROM.write(i++, 1); //indicates that there is a 
  EEPROM.write(i++, 0); //indicates that there is a 

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
  
  
//  int i = 0;  
//  
//  Serial.println("Writing ssid to EEPROM"); 
//  auto ssidSize = ssid.length();
//  EEPROM.write(i++, ssidSize);
//  for (; i < ssidSize; i++) {
//    EEPROM.write(i, (uint8_t)ssid[i]);
//    yield();
//  }
//
//  Serial.println("Writing password to EEPROM"); 
//  auto passwordSize = password.length();
//  EEPROM.write(i++, passwordSize);
//  for (int j = 0; j < passwordSize; j++, i++) {
//    EEPROM.write(i, (uint8_t)password[j]);
//    yield();
//  }  
//  
  EEPROM.commit();
}

Credentials EEPROM_Manager::readCredentials() {  
  Serial.println("Reading credentials:");
//  auto ssidSize = EEPROM.read(0);
//  yield();
//  
//  String ssid = "";
//  for (int i = 1; i < ssidSize + 1; )
//  {
//    ssid += char(EEPROM.read(i));
//    yield();
//  }
//  Serial.print("SSID: ");
//  Serial.println(ssid);
//  
//  String password = "";
//  auto passwordSize = EEPROM.read(ssidSize + 1);
//  for (int i = passwordSize + 2; i < ssidSize + passwordSize + 2; i++)
//  {
//    password += char(EEPROM.read(i));
//    yield();
//  }
//  Serial.print("PW: ");
//  Serial.println(password);
//
//  EEPROM.end();


//  Credentials2 credentials; //Variable to store custom object read from EEPROM.
//  EEPROM.get(0, credentials);
//  Serial.print(credentials.ssid);
//  Serial.print(" | ");
//  Serial.print(credentials.password);

  int i = 3; //because of the sequence

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
  auto has = EEPROM.read(0) == 0 && 
    EEPROM.read(1) == 1 &&
    EEPROM.read(2) == 1 &&
    EEPROM.read(3) == 0;
  Serial.print("Has credentials in EEPROM? ");
  Serial.println(has);
  return has;
    
}
