/*
 * SerialReader.h
 *
 *  Created on: Mar 14, 2012
 *      Author: bob
 */

#ifndef SERIALREADER_H_
#define SERIALREADER_H_



void initSerialReader();
void updateSerialReader();

extern bool (*parserReadyDelegate) ();
extern void (*parseGCodeDelegate) (const char* line);

#endif /* SERIALREADER_H_ */
