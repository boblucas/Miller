/*
 * Stepper.c
 *
 *  Created on: Mar 14, 2012
 *      Author: bob
 */

#include "WProgram.h"
#include "Stepper.h"
#include "Settings.h"

unsigned long lastAcceleration = 1 << 15;
double speed = INITIAL_STEP_DELAY;
unsigned long stepsFromStandstill = 0;

Axis axis[AXISCOUNT];
Axis* longest;

void sleep()
{
	for(Axis* a = &axis[0]; a != &axis[0]+AXISCOUNT; a++)
	{
		a->localSleep = true;
		a->doneAt = millis();
	}
}

void awake()
{
	for(Axis* a = &axis[0]; a != &axis[0]+AXISCOUNT; a++)
	{
		a->localSleep = false;
		a->doneAt = 0;
		digitalWrite(a->pins + SLEEP, false);
	}

    lastAcceleration = millis() - ACCELERATION_RATE - 1;
    speed = INITIAL_STEP_DELAY;
    stepsFromStandstill = 0;
}

void initStepper()
{
	for(int i = 2; i < 11; i++)
	{
		pinMode(i, OUTPUT);
		digitalWrite(i, LOW);
	}

	for(int i = 0; i < AXISCOUNT; i++)
	{
		axis[i].direction = 0;
		axis[i].expected = 0;
		axis[i].lastIteration = 0;
		axis[i].stepState = INITIAL_STEP_DELAY;
		axis[i].position = 0;
	}

	axis[0].pins = 5;
	axis[1].pins = 2;
	axis[2].pins=  8;

	axis[0].limit = 60000;
	axis[1].limit = 60000;
	axis[2].limit=  40000;

	for(Axis* a = &axis[0]; a != &axis[0]+AXISCOUNT; a++)
	{
		a->localSleep = true;
		a->doneAt = 0;
		digitalWrite(a->pins + SLEEP, true);
	}
}

void updateStepper()
{
	bool allAsleep = false;
	for(int i = 0; i < AXISCOUNT && !allAsleep; i++)
		allAsleep = axis[i].localSleep;

	if(allAsleep)
		return;

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
	}

	for(int i = 0; i < AXISCOUNT; i++)
	{
		Axis& a = axis[i];
		a.stepDelay = axis[i].speed * speed;

		if(a.localSleep && a.doneAt + 10 < millis() && a.doneAt != 0)
		{
			digitalWrite(a.pins + SLEEP, true);
			a.doneAt = 0;
		}

		if(a.localSleep)
			continue;

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

