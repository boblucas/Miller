/*
 * State.h
 *
 *  Created on: Feb 11, 2012
 *      Author: Bob
 */

#ifndef STATE_H_
#define STATE_H_

#define DIR 0
#define STEP 1
#define SLEEP 2

#define XYPLANE 0
#define XZPLANE 1
#define YZPLANE 2

#define PULSES_PER_MM 400
#define MIN_STEP_DELAY 750
#define INITIAL_STEP_DELAY 10000
//the amount of milliseconds between each change in speed
#define ACCELERATION_RATE 5
//the maximum amount of acceleration actually applied at each change
#define ACCELERATION 1.02

#define AXISCOUNT 3

struct Axis
{
	bool direction;			//the state of the direction pin
	bool stepState;			//the state of the step pin
	bool localSleep;		//the state of the sleep pin
	char pins;			//it is expected that pins+DIR is the dir pin of this axis, pins+STEP is the step etc.
	unsigned long stepDelay;		//this is the amount of microseconds between each pulse
	unsigned long lastIteration;	//this was the last time in micros since startup that we pulsed the motor
	unsigned long doneAt;		//when position and expected are equal this tells us in micros when that happend
	long position;			//our current position
	long expected;			//what position we are travelling towards
	long limit;			//The maximum range(both positive and negative) of the axis
	double speed;			//What ratio of the total speed should this axis receive to make the head go at the expected angle

} axis[AXISCOUNT];

Axis* lastAxis = &axis[0]+AXISCOUNT;
Axis* longest;

unsigned long lastAcceleration = 0;
double speed = INITIAL_STEP_DELAY;
unsigned long stepsFromStandstill = 0;

bool (*drawingUpdate)() = 0;
bool generalUpdate()
{
	for(int i = 0; i < AXISCOUNT; i++)
		if(axis[i].position != axis[i].expected)
			return false;
	return true;
}

//gcode state:
char plane = 0;

#endif /* STATE_H_ */
