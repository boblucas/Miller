/*
 * Stepper.h
 *
 *  Created on: Feb 15, 2012
 *      Author: koen
 */

#ifndef STEPPER_H_
#define STEPPER_H_

#include "State.h"
#include "Graphics.h"



void sleep()
{
	for(Axis* a = &axis[0]; a != lastAxis; a++)
	{
		a->localSleep = true;
		a->doneAt = millis();
	}
}

void awake()
{
	for(Axis* a = &axis[0]; a != lastAxis; a++)
	{
		a->localSleep = false;
		a->doneAt = 0;
		digitalWrite(a->pins + SLEEP, false);
	}
}


void updateStepper()
{
	if(lastAcceleration + ACCELERATION_RATE <= millis())
	{
		lastAcceleration += ACCELERATION_RATE;
		//int timeLeft = MIN_STEP_DELAY * abs(longest->expected - longest->position);

		if(((INITIAL_STEP_DELAY / 3) * abs(longest->expected - longest->position)) >= (stepsFromStandstill * ACCELERATION_RATE) * 1000)
		{
			if(longest->stepDelay > MIN_STEP_DELAY)
			{
				speed = speed / ACCELERATION;
				stepsFromStandstill++;
			}
		}
		else if(stepsFromStandstill > 0)
		{
			speed = speed * (((ACCELERATION - 1) * 1.33) + 1.0);
			stepsFromStandstill--;
			if(stepsFromStandstill == 0)
				speed = INITIAL_STEP_DELAY;
		}

		axis[0].stepDelay = axis[0].speed * speed;
		axis[1].stepDelay = axis[1].speed * speed;
		axis[2].stepDelay = axis[2].speed * speed;
	}

	for(int i = 0; i < AXISCOUNT; i++)
	{
		Axis& a = axis[i];

		if(a.localSleep && a.doneAt + 10 < millis() && a.doneAt != 0)
		{
			digitalWrite(a.pins + SLEEP, true);
			a.doneAt = 0;
		}

		if(a.localSleep) continue;

		if(a.lastIteration + a.stepDelay <= micros() && (a.position != a.expected))
		{
			a.lastIteration = micros();
			a.stepState = !a.stepState;

			if(a.direction)
				a.position--;
			else
				a.position++;

			if(a.position <= -a.limit || a.position >= a.limit )
				a.expected = a.position;

			digitalWrite(a.pins + STEP, a.stepState);
		}


		if(a.position == a.expected)
		{
			a.localSleep = true;
			a.doneAt = millis();
		}
	}
}

#endif /* STEPPER_H_ */
