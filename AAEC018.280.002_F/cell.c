/*
 * cell.c
 *
 * Created: 16.08.2021 14:19:38
 *  Author: Andrii
 */ 

 #include "includes.h"

 volatile tCellStructure CellStructure;

 void init_cell(){
	CellStructure.state = CellOk;
	CellStructure.cell_motor_pwm = 0;
 }

 void cell_update_control_values(bool park, uint8_t motor_pwm){
	if(park){
		CellStructure.state = CellPark;
		CellStructure.cell_motor_pwm = 0;
		PORTD |= (1 << PD7);														//brake pin
	}
	else{
		CellStructure.cell_motor_pwm = motor_pwm;
		CellStructure.state = CellRotate;
		PORTD &= ~(1 << PD7);														//brake pin
		PORTB |= (1 << PB0);														//direction pin
	}
}

 tCellState get_cell_state(){
	return CellStructure.state;
}

 void cell_isr_routine(){
 static uint8_t cell_pwm_counter = 0;
 static tSoftPwmState cell_pwm_state = 0;
 switch(cell_pwm_state){
		 case pwmSetUpdate:
			 if(CellStructure.cell_motor_pwm == 0){
				 PORTD &= ~(1 << PD5);											//cell motor pin
				 return;
			 }
			 if(CellStructure.cell_motor_pwm>= 100){
				 PORTD |= (1 << PD5);												//cell motor pin
				 return;
			 }
			 PORTD |= (1 << PD5);													//cell motor pin
			 cell_pwm_counter = CellStructure.cell_motor_pwm;
			 cell_pwm_state = pwmClear;
			 break;
		 case pwmClear:
			 if(cell_pwm_counter)
			 cell_pwm_counter--;
			 else{
				 PORTD &= ~(1 << PD5);											//cell motor pin
				 cell_pwm_counter = 100 - CellStructure.cell_motor_pwm;
				 cell_pwm_state = pwmReset;
			 }
			 break;
		 case pwmReset:
			 if(cell_pwm_counter)
			 cell_pwm_counter--;
			 else
			 cell_pwm_state = pwmSetUpdate;
			 break;
	 }
 }