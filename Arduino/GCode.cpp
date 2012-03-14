/*
 * GCode.cpp
 *
 *  Created on: Mar 14, 2012
 *      Author: bob
 */

#include "Arduino.h"
#include "Settings.h"
#include <math.h>
#include "GCode.h"
#include "Utils.h"
#include "Stepper.h"

bool (*drawingUpdate)() = 0;
char plane = 0;
float argumentMap[26];
bool isParserReady = true;


void (*sleepDelegate)() = 0;
void (*awakeDelegate)() = 0;


void parseArguments(const char*& line);
void drawLine(double x, double y, double z, int f = 1000);
bool generalUpdate();
bool M30Update();


void initGCode()
{
	for(char i = 0; i < 26; i++)
		argumentMap[i] = 0;

	isParserReady = true;
}

void updateGCode()
{
	if( drawingUpdate )
	{
		if((*drawingUpdate)())
		{
			Serial.println("C2");
			sleepDelegate();
			drawingUpdate = 0;
			isParserReady = true;
		}
	}
}


bool parserReady()
{
	return isParserReady;
}

void parseGcodeLine(const char* line )
{
    isParserReady = false;
    parseArguments(line);

    //gcodes
    switch((int)(argumentMap['G' - 'A']))
	{
		case 0://move to given position as fast as possible.
		case 1://linearly interpolate to given position at given speed.
			awakeDelegate();
			drawingUpdate = &generalUpdate;
			drawLine(argumentMap['X' - 'A'], argumentMap['Y' - 'A'], -argumentMap['Z' - 'A']);
		break;
		case 04: delay(argumentMap['P' - 'A'] * 1000); break;
		case 17: plane = XYPLANE; break;
		case 18: plane = XZPLANE; break;
		case 19: plane = YZPLANE; break;
		case 28:
			awakeDelegate();
			drawingUpdate = &generalUpdate;
			drawLine(0, 0, 0);
		break;
	}

    switch((int)(argumentMap['M' - 'A']))
    {
    	case 30:
    		drawLine(axis[0].position, axis[1].position, 0);
    		drawingUpdate = &M30Update;
    	break;
    }

    if(!drawingUpdate)
    {
    	Serial.println("C1");
    	isParserReady = true;
    }
    updateGCode();
}

void parseArguments(const char*& line)
{
	//cache G and M as one unit
    int previousG = argumentMap['G' - 'A'];
    int previousM = argumentMap['M' - 'A'];
	argumentMap['M' - 'A'] = -1;
	argumentMap['G' - 'A'] = -1;

	while((*line) >= 'A' && (*line) <= 'Z' )
	{
		char parameter = (*line) - 'A';
		line++;
		argumentMap[parameter] = parseNumber(line);
	}

    if(argumentMap['M' - 'A'] == -1 && argumentMap['G' - 'A'] == -1)
    {
    	argumentMap['G' - 'A'] = previousG;
    	argumentMap['M' - 'A'] = previousM;
    }
}

void drawLine(double x, double y, double z, int f)
{
    //eta = micros() + ((STEP_DELAY / 1000.0) * PULSES_PER_MM * fmax(fabs(_x), fabs(_y))) * 1000.0;
    axis[0].expected = round(x * PULSES_PER_MM) - axis[0].position;
    axis[1].expected = round(y * PULSES_PER_MM) - axis[1].position;
    axis[2].expected = round(z * PULSES_PER_MM) - axis[2].position;

    longest = &axis[0];

    if(abs(axis[1].expected) >= abs(axis[0].expected) && abs(axis[1].expected) > abs(axis[2].expected))
    	longest = &axis[1];
    else if(abs(axis[2].expected) > abs(axis[0].expected) && abs(axis[2].expected) >= abs(axis[1].expected))
    	longest = &axis[2];

    long expected = longest->expected;
    for(Axis* a = &axis[0]; a != &axis[0]+AXISCOUNT; a++)
    {
		//(int)((((double)abs(expected)) / ((double)abs(a->expected))) * f);
        a->speed =  ( (double)abs(expected) ) / ( (double)abs(a->expected) );
        a->stepDelay = INITIAL_STEP_DELAY * a->speed;
        a->expected += a->position;
		a->direction = a->expected < a->position;
		digitalWrite(a->pins + DIR, a->direction);
    }

}


bool generalUpdate()
{
	for(int i = 0; i < AXISCOUNT; i++)
		if(axis[i].position != axis[i].expected)
			return false;
	return true;
}

bool M30Update()
{
	if(generalUpdate())
		drawLine(0, 0, 0);

	return axis[0].position == 0 && axis[1].position == 0 && axis[2].position == 0;
}



