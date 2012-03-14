/*
 * GCode.h
 *
 *  Created on: Mar 14, 2012
 *      Author: bob
 */

#ifndef GCODE_H_
#define GCODE_H_

/*
 * assumtions made in this parser:
 * a plus sign in front of number to denote it is positive is never used.
 * Line numbers are not used.
 * It's always cutting in mm.
 * Feed rate is currently constant
 */

void initGCode();
void updateGCode();

bool parserReady();
void parseGcodeLine(const char* line );

extern void (*sleepDelegate)();
extern void (*awakeDelegate)();

#endif /* GCODE_H_ */
