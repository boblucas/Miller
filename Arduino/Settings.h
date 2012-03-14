/*
 * Settings.h
 *
 *  Created on: Mar 14, 2012
 *      Author: bob
 */

#ifndef SETTINGS_H_
#define SETTINGS_H_

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

#endif /* SETTINGS_H_ */
