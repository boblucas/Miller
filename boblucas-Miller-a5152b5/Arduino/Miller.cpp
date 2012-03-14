#include "Miller.h"
#include "HardwareSerial.h"
#include "State.h"
#include "GCodeParser.h"
#include "Stepper.h"

extern HardwareSerial Serial;

//static memory usage
//arduino globals probably use 100 or 200 bytes
//All remaining memory might be used for variables on the stack.
//Miller.cpp: 	68  // gcode buffer
//Graphics.h: 	18 	// arc drawing state
//GcodeParser: 	109 // argument map
//State:		55	// State of the axis
//Stepper:		1
//--------------------------------------
//				262 bytes
//Additionally the arc function has 62 bytes of state

char buffer[256];
char* lineBuffer = &buffer[0];
bool currentLineParsed = false;

void setup()
{
	Serial.begin(9600);
	Serial.println('W');

	int i;
	for(i = 0; i < 32; i++)
	  lineBuffer[i] = 0;

	for(i = 0; i < 26; i++)
		argumentMap[i] = 0;

	for(i = 2; i < 11; i++)
	{
		pinMode(i, OUTPUT);
		digitalWrite(i, LOW);
	}

	for(i = 0; i < AXISCOUNT; i++)
	{
		axis[i].direction = 0;
		axis[i].expected = 0;
		axis[i].lastIteration = 0;
		axis[i].stepState = INITIAL_STEP_DELAY;
		axis[i].position = 0;
	}

	axis[0].pins = 5;
	axis[1].pins = 2;
	axis[2].pins=  8;

	axis[0].limit = 60000;
	axis[1].limit = 60000;
	axis[2].limit=  40000;

	lastAcceleration = 1 << 15;

	for(Axis* a = &axis[0]; a != lastAxis; a++)
	{
		a->localSleep = true;
		a->doneAt = 0;
		digitalWrite(a->pins + SLEEP, true);
	}
}

void loop()
{
    updateStepper();
    gcodeLoop();

    while(Serial.available() > 0 && parserReady)
    {
        (*lineBuffer) = Serial.read();
        if(*lineBuffer == '\n')
        {
           lineBuffer = &buffer[0];
           parseGcodeLine(lineBuffer);
        }
        else if(*lineBuffer != 32)
        	lineBuffer++;
    }
}

