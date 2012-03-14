/*
 * Graphics.h
 *
 *  Created on: Feb 11, 2012
 *      Author: bob
 */

#ifndef GRAPHICS_H_
#define GRAPHICS_H_

#include <math.h>
#include "State.h"

void drawLine(double x, double y, double z, int f = 1000)
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
    for(Axis* a = &axis[0]; a != lastAxis; a++)
    {
		//(int)((((double)abs(expected)) / ((double)abs(a->expected))) * f);
        a->speed =  ( (double)abs(expected) ) / ( (double)abs(a->expected) );
        a->stepDelay = INITIAL_STEP_DELAY * a->speed;
        a->expected += a->position;
		a->direction = a->expected < a->position;
		digitalWrite(a->pins + DIR, a->direction);
    }

    lastAcceleration = millis() - ACCELERATION_RATE - 1;
    speed = INITIAL_STEP_DELAY;
    stepsFromStandstill = 0;

	Serial.println("position:");
	Serial.println(axis[0].position);
	Serial.println(axis[1].position);
	Serial.println(axis[2].position);
	Serial.println("expected:");
	Serial.println(axis[0].expected);
	Serial.println(axis[1].expected);
	Serial.println(axis[2].expected);
	Serial.println("direction:");
	Serial.println(axis[0].direction);
	Serial.println(axis[1].direction);
	Serial.println(axis[2].direction);
	Serial.println("speed:");
	Serial.println(axis[0].stepDelay);
	Serial.println(axis[1].stepDelay);
	Serial.println(axis[2].stepDelay);
	Serial.println("---------");
}

bool M30Update()
{
	if(generalUpdate())
		drawLine(0, 0, 0);

	return axis[0].position == 0 && axis[1].position == 0 && axis[2].position == 0;
}


#endif /* GRAPHICS_H_ */
