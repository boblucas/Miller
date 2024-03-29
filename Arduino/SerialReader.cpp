/*
 * SerialReader.cpp
 *
 *  Created on: Mar 14, 2012
 *      Author: bob
 */
#include "SerialReader.h"
#include "WProgram.h"
#include "Utils.h"
#include "Settings.h"
#include "HardwareSerial.h"
#include "Stepper.h"

bool (*parserReadyDelegate) ();
void (*parseGCodeDelegate) (const char* line);
void (*serialSleepDelegate) ();
void (*serialWakeDelegate) ();

extern HardwareSerial Serial;

char buffer[128];
char* lineBuffer = &buffer[0];
bool readingRequest = false;

void parseRequest(const char* line)
{
	readingRequest = false;
	int code = parseNumber(line);
	switch(code)
	{
	case POSITION: //return position
		Serial.write('X');
		Serial.write( ((double)axis[0].position) / PULSES_PER_MM);
		Serial.write('Y');
		Serial.write( ((double)axis[1].position) / PULSES_PER_MM);
		Serial.write('Z');
		Serial.println( ((double)axis[2].position) / PULSES_PER_MM);
		break;
	case DESTINATION: //return destination
		Serial.write('X');
		Serial.write( ((double)axis[0].expected) / PULSES_PER_MM);
		Serial.write('Y');
		Serial.write( ((double)axis[1].expected) / PULSES_PER_MM);
		Serial.write('Z');
		Serial.println( ((double)axis[2].expected) / PULSES_PER_MM);
		break;
	case PAUSE:
		serialSleepDelegate();
		break;
	case CONTINUE:
		serialWakeDelegate();
		break;
	}
}

void initSerialReader()
{
	Serial.begin(9600);
	Serial.println('W');
}
void updateSerialReader()
{
    while(Serial.available() > 0 && parserReadyDelegate())
    {
        (*lineBuffer) = Serial.read();

        if(*lineBuffer == '\n')
        {
            lineBuffer = &buffer[0];
        	if(readingRequest)
        		parseRequest(lineBuffer);
        	else
        		parseGCodeDelegate(lineBuffer);
        }
        else if(*lineBuffer == '@')
        	readingRequest = true;
        else if(*lineBuffer != 32)
        	lineBuffer++;
    }
}

