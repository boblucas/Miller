/*
 * GCodeParser.h
 *
 *  Created on: Feb 11, 2012
 *      Author: koen
 */

#ifndef GCODEPARSER_H_
#define GCODEPARSER_H_

#include "Graphics.h"
#include "Stepper.h"
/*
 * assumtions made in this parser:
 * a plus sign in front of number to denote it is positive is never used.
 * Line numbers are not used.
 * It's always cutting in mm.
 * Feed rate is currently constant
 */

boolean parserReady = true;

float argumentMap[26];


void gcodeLoop();

double parseNumber(const char*& s)
{
    int negative = 1;
    double result = 0.0;

    for(;;s++)
    {
       if(*s == '.')
       {
           const char* dotPosition = ++s;
           return (result + parseNumber(s) / pow(10, s - dotPosition)) * negative;
       }
       else if(*s >= '0' && *s <= '9')
           result = result * 10 + ((*s) - 48);
       else if(*s == '-')
            negative = -1;
       else
           return result * negative;
    }
    return -1;
}

char* doubleToString(double number, int precision)
{
	int n = (int)number;
	int p = (number - n) * precision;

	char* sNumber = new char[ceil(log(n)) + 1 + ceil(log(p))];
	sprintf(sNumber, "%i.%i", n, p);
	return sNumber;
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

void parseGcodeLine(const char* line )
{
    parserReady = false;
    parseArguments(line);

    //gcodes
    switch((int)(argumentMap['G' - 'A']))
	{
		case 0://move to given position as fast as possible.
		case 1://linearly interpolate to given position at given speed.
			awake();
			drawingUpdate = &generalUpdate;
			drawLine(argumentMap['X' - 'A'], argumentMap['Y' - 'A'], -argumentMap['Z' - 'A']);
		break;
		case 04: delay(argumentMap['P' - 'A'] * 1000); break;
		case 17: plane = XYPLANE; break;
		case 18: plane = XZPLANE; break;
		case 19: plane = YZPLANE; break;
		case 28:
			awake();
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
    	parserReady = true;
    }
    gcodeLoop();
}

void parseAtSign(const char* line)
{
	char request = parseNumber(line);
	switch(request)
	{
	case 0:
		Serial.println("x");
		break;
	}
}

void gcodeLoop()
{
	if( drawingUpdate )
	{
		if((*drawingUpdate)())
		{
			Serial.println("C2");
			sleep();
			drawingUpdate = 0;
			parserReady = true;
		}
	}
}


#endif /* GCODEPARSER_H_ */
