/******************************************************/
//       THIS IS A GENERATED FILE - DO NOT EDIT       //
/******************************************************/

#include "Particle.h"
#line 1 "/Users/tbar0012/Projects/ObsTallyLamp/TallyLamp/src/TallyLamp.ino"
/*
 * Project TallyLamp
 * Description:
 * Author:
 * Date:
 */

#include "neopixel.h"

void setup();
int setSource(String tally);
int getTally();
int setTally(String stal);
void toggle_flash();
void loop();
#line 10 "/Users/tbar0012/Projects/ObsTallyLamp/TallyLamp/src/TallyLamp.ino"
Adafruit_NeoPixel strip = Adafruit_NeoPixel(1, D6, WS2812B); 

UDP Udp;

struct Settings
{
  int Tally=0;
};

Timer timer(300, toggle_flash);

enum TallyMode {OFF, PREVIEW, LIVE};

Settings CurrentSettings;
TallyMode CurrentMode;
bool IsStreaming = false;

LEDStatus blinkRed(RGB_COLOR_RED, LED_PATTERN_BLINK, LED_SPEED_SLOW, LED_PRIORITY_IMPORTANT);
LEDStatus solidRed(RGB_COLOR_RED, LED_PATTERN_SOLID, LED_SPEED_NORMAL, LED_PRIORITY_IMPORTANT);
LEDStatus changeSettings(RGB_COLOR_BLUE,LED_PATTERN_BLINK,LED_SPEED_FAST,LED_PRIORITY_IMPORTANT);
LEDStatus black(0x00000000,LED_PATTERN_SOLID,LED_SPEED_FAST,LED_PRIORITY_IMPORTANT);

// setup() runs once, when the device is first turned on.
void setup() {
  // Put initialization like pinMode and begin functions here.

  EEPROM.get(0,CurrentSettings);

  strip.begin();
  timer.start();

  // digitalWrite(A3,4095);

  Particle.function("SetTallySource", setSource);
  Particle.function("SetTally",setTally);
  Particle.variable("TallySource",getTally);

  int remotePort = 8854;
  IPAddress multicastAddress(224,0,0,0);
  Udp.begin(remotePort);
  Udp.joinMulticast(multicastAddress);

  Serial.begin(9600);
  // Serial.println(WiFi.localIP());
}

int setSource(String tally){
  CurrentSettings.Tally = max(1,min(tally.toInt(),8));
  EEPROM.put(0,CurrentSettings);
  changeSettings.setActive(true);
  delay(1000);
  changeSettings.setActive(false);
  black.setActive(true);
  // Serial.println("Set Source to " + tally);
  return 0;
}

int getTally()
{
  return CurrentSettings.Tally;
}

int setTally(String stal)
{
  int tal = stal.toInt();
  if (tal < 0 || tal > 2)
    return -1;
  
  switch (tal)
  {
    case 0:
      CurrentMode = OFF;
    break;
    case 1:
      CurrentMode = PREVIEW;
    break;
    case 2:
      CurrentMode = LIVE;
    break;
  }
  return 0;
}

bool toggle = false;

void toggle_flash()
{
  // if (CurrentMode == PREVIEW)
  // {
    if (toggle)
    {
      strip.setColor(0,0,255,0);
      strip.show();
    }
    else
    {
      strip.setColor(0,0,0,0);
      strip.show();
    }
    toggle = !toggle;
  // }
}

// loop() runs over and over again, as quickly as it can execute.
void loop() {
  // The core of your code will likely live here.
  if (Udp.parsePacket() > 1) {

    byte mask = 0b00000001 << (CurrentSettings.Tally-1);

    // Read first char of data received
    char Preview = Udp.read();
    bool IsPreview = Preview & mask;

    char Live = Udp.read();
    bool IsLive = Live & mask;

    IsStreaming = 0;

    IsStreaming = Udp.read();

    if (!IsLive && !IsPreview)
      CurrentMode = OFF;
    
    if (IsPreview)
      CurrentMode = PREVIEW;

    if (IsLive)
      CurrentMode = PREVIEW;

    if (IsLive && IsStreaming)
      CurrentMode = LIVE;

    Serial.println(IsLive);
    Serial.println(IsPreview);
    Serial.println(IsStreaming);
    Serial.println(CurrentMode);

    // Ignore other chars
    while(Udp.available())
      Udp.read();
  }

  switch (CurrentMode)
  {
    case OFF:
      black.setActive(true);
      blinkRed.setActive(false);
      solidRed.setActive(false);
      strip.setColor(0,0,0,0);
      strip.show();
      timer.stop();
    break;

    case PREVIEW:
      black.setActive(false);
      blinkRed.setActive(true);
      solidRed.setActive(false);
      if (!timer.isActive())
      {
        toggle = true;
        toggle_flash();
        timer.start();
      }
      // toggle = true;
      // strip.setColor(0,255,0,0);
      // strip.show();
    break;

    case LIVE:
      black.setActive(false);
      blinkRed.setActive(false);
      solidRed.setActive(true);
      strip.setColor(0,0,255,0);
      strip.show();
      timer.stop();
    break;
  }
}
