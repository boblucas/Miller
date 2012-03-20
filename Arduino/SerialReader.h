/*
 * SerialReader.h
 *
 *  Created on: Mar 14, 2012
 *      Author: bob
 */

#ifndef SERIALREADER_H_
#define SERIALREADER_H_


enum SerialInterrupts
{
	POSITION,
	DESTINATION,
	PAUSE,
	CONTINUE
};

void initSerialReader();
void updateSerialReader();

extern bool (*parserReadyDelegate) ();
extern void (*parseGCodeDelegate) (const char* line);

extern void (*serialSleepDelegate) ();
extern void (*serialWakeDelegate) ();

#endif /* SERIALREADER_H_ */
