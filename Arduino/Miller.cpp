
#include "Miller.h"

#include "GCode.h"
#include "Stepper.h"
#include "SerialReader.h"

void setup()
{
	initStepper();

	initGCode();
	awakeDelegate = &awake;
	sleepDelegate = &sleep;

	initSerialReader();
	parseGCodeDelegate = &parseGcodeLine;
	parserReadyDelegate = &parserReady;
	serialSleepDelegate = &sleep;
	serialWakeDelegate = &awake;

}

void loop()
{
    updateStepper();
    updateGCode();
    updateSerialReader();
}


