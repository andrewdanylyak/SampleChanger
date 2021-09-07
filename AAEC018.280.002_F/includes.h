/*
 * includes.h
 *
 * Created: 13.08.2021 13:11:04
 *  Author: Andrii
 */ 


#ifndef INCLUDES_H_
#define INCLUDES_H_

#define F_CPU 800000UL

#include <avr/io.h>
#include <stdbool.h>
#include <avr/interrupt.h>
#include <avr/cpufunc.h>
#include <util/delay.h>
#include <string.h>

#include "lift.h"
#include "timer.h"
#include "disk.h"
#include "cell.h"
#include "gpio.h"
#include "i2c.h"

#endif /* INCLUDES_H_ */