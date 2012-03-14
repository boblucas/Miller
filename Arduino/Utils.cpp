/*
 * Utils.cpp
 *
 *  Created on: Mar 14, 2012
 *      Author: bob
 */

#include <math.h>

double parseNumber(const char*& s)
{
    int negative = 1;
    double result = 0.0;

    for(;;s++)
    {
       if(*s == '.')
       {
           const char* dotPosition = ++s;
           return (result + parseNumber(s) / pow(10, s - dotPosition)) * negative;
       }
       else if(*s >= '0' && *s <= '9')
           result = result * 10 + ((*s) - 48);
       else if(*s == '-')
            negative = -1;
       else
           return result * negative;
    }
    return -1;
}


