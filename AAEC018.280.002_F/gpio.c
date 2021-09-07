/*
 * gpio.c
 *
 * Created: 16.08.2021 15:54:19
 *  Author: Andrii
 */ 

 #include "includes.h"

 void init_gpio(){
	/*init lift gpios*/
	DDRC &= ~(1 << DD3);									// end switch pin input
	DDRC |= (1 << DD2);										// lift enable pin output
	PORTC &= ~(1 << PORT2);								// lift enable pin off

	DDRB |= (1 << DD2);										//syc led pwm pin
	DDRB |= (1 << DD1);										// servo control pin output
	DDRB |= (1 << DD0);										//direction signal pin
	PORTB |= (1 << PB0);									//direction pin, any direction
	PORTB &= ~(1 << PB1);									//servo control pin off
	PORTB &= ~(1 << PB2);									//sync led pin off
	
	DDRD |= (1 << DD7);										//brake signal pin
	DDRD |= (1 << DD6);										//disk motor pin
	DDRD |= (1 << DD5);										//cell motor pin
	DDRD |= (1 << DD3);										//data led pwm pin

	PORTD |= (1 << PD7);									//brake pin on -> brake func off
	PORTD &= ~(1 << PD6);									//disk motor pin off		
	PORTD &= ~(1 << PD5);									//cell motor pin off
	PORTD &= ~(1 << PD3);									//data led pin off

 }