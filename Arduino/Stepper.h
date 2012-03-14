/*
 * Stepper.h
 *
 *  Created on: Feb 15, 2012
 *      Author: bob
 */

#ifndef STEPPER_H_
#define STEPPER_H_

#include "Settings.h"



void initStepper();
void updateStepper();

void sleep();
void awake();

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

};

extern Axis axis[AXISCOUNT];
extern Axis* longest;


#endif /* STEPPER_H_ */
