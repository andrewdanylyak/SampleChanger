/*
 * lift.c
 *
 * Created: 13.08.2021 13:06:54
 *  Author: Andrii
 */ 

#include "includes.h"

volatile tLiftStructure LiftStructure;

void init_lift_pwm(){
	TCCR1A |= (1 << COM1A1)|(1 << COM1A0)|(0 << COM1B1)|(0 << COM1B0)|(1 << WGM11)|(1 << WGM10);
	TCCR1B |= (0 << ICNC1)|(0 << ICES1)|(0 << WGM13)|(0 << WGM12)|(0 << CS12)|(1 << CS11)|(1 << CS10);
	OCR1A = 0;
}

tLiftCommandPosition get_lift_position(){
	if((PINC&(1 << PIN3)) == (1 << PIN3))
		return LiftCommandDown;
	else
		return LiftCommandUp;
}

void init_lift(){
	init_lift_pwm();
	LiftStructure.command_position = get_lift_position();
	LiftStructure.new_command_position = LiftStructure.command_position;
	LiftStructure.new_command = false;
	LiftStructure.timeout = 0;
	LiftStructure.state = Lift_OK;
	LiftStructure.autocheck_state = astmLiftNotChecked;	
	LiftStructure.state_mashine = stmLiftIddle;
}

void autotest_lift(){
		LiftStructure.new_command = false;
		LiftStructure.timeout = 0;
		LiftStructure.state = Lift_OK;
		LiftStructure.autocheck_state = astmLiftNotChecked;
		LiftStructure.state_mashine = stmLitStart;
}

void set_new_lift_position(tLiftCommandPosition new_pos){
	if(check_lift_bussy())
		return;
	LiftStructure.new_command_position = new_pos;
	LiftStructure.new_command = true;
	LiftStructure.state_mashine = stmLitStart;
}

void lift_isr_routine(){
	static uint8_t pwm = 0;
	static uint8_t pwm_timeout = 0;
	switch(LiftStructure.state_mashine){
		case stmLiftIddle:
		case stmLiftError:
			return;
		case stmLitStart:
			if(LiftStructure.autocheck_state == astmLiftCheckedOk){
				LiftStructure.timeout = LIFT_TIMEOUT;
				LiftStructure.state_mashine = stmLiftRunningCeckPosition;
			}
			else if((LiftStructure.autocheck_state != astmLiftCheckedOk)||(LiftStructure.autocheck_state != astmLiftCheckFailed)){
				switch (LiftStructure.autocheck_state){
				case astmLiftNotChecked:
					LiftStructure.new_command_position = LiftCommandUp;
					LiftStructure.timeout = LIFT_START_TIMEOUT;
					LiftStructure.autocheck_state = astmLiftBlind0;
					set_lift_position_up();
					break;
				case astmLiftBlind0:
					if(!LiftStructure.timeout){
						LiftStructure.timeout = LIFT_TIMEOUT;
						LiftStructure.autocheck_state = astmLiftGotoNextPosition0;
					}
					break;
				case astmLiftGotoNextPosition0:
					if(!LiftStructure.timeout){
						LiftStructure.autocheck_state = astmLiftCheckFailed;
						LiftStructure.state_mashine = stmLiftError;
						disable_lift();
						break;
					}
					LiftStructure.command_position = get_lift_position();
					if(LiftStructure.command_position == LiftStructure.new_command_position){
						if(LiftStructure.command_position == LiftCommandUp){
							LiftStructure.timeout = LIFT_TIMEOUT;
							LiftStructure.autocheck_state = astmLiftDelayBeforePosition1;
							break;
						}
					}
					break;
				case astmLiftDelayBeforePosition1:
					if(!LiftStructure.timeout){
						LiftStructure.timeout = LIFT_START_TIMEOUT;
						LiftStructure.autocheck_state = astmLiftBlind1;
						LiftStructure.new_command_position = LiftCommandDown;
						set_lift_position_down();
					}
					break;
				case astmLiftBlind1:
					if(!LiftStructure.timeout){
						LiftStructure.timeout = LIFT_TIMEOUT;
						LiftStructure.autocheck_state = astmLiftGotoNextPosition1;
					}
					break;
				case astmLiftGotoNextPosition1:
					if(!LiftStructure.timeout){
						LiftStructure.autocheck_state = astmLiftCheckFailed;
						LiftStructure.state_mashine = stmLiftError;
						disable_lift();
						break;
					}
					LiftStructure.command_position = get_lift_position();
					if(LiftStructure.command_position == LiftStructure.new_command_position){
						LiftStructure.autocheck_state = astmLiftCheckedOk;
						LiftStructure.state_mashine = stmLiftIddle;
						disable_lift();
					}
					break;
				}
			}
			break;
		case stmLiftRunningCeckPosition:
			LiftStructure.command_position = get_lift_position();
			if(LiftStructure.command_position != LiftStructure.new_command_position){
				LiftStructure.state_mashine = stmLiftRunCommand;
			}
			else{
				if(LiftStructure.new_command_position == LiftCommandUp)
					LiftStructure.state = Lift_UP;
				else
					LiftStructure.state = Lift_DOWN;
				LiftStructure.new_command = false;
				LiftStructure.state_mashine = stmLiftIddle;
			}
			break;
		case stmLiftRunCommand:
			if(LiftStructure.new_command_position == LiftCommandUp){
				set_lift_position_up();
				LiftStructure.state_mashine = stmLiftRunCheckTimeout;
			}
			else {
#ifdef FUTABA_S3003
				pwm = 50;
#else
				pwm = 135;
#endif
				set_lift_enble();
				set_lift_pwm_direct(pwm);
				pwm_timeout = 10;
				LiftStructure.state_mashine = stmLiftDownSlowly;
			}
			break;
		case stmLiftDownSlowly:
				if(pwm_timeout == 0){
#ifdef FUTABA_S3003
					if(pwm++ < 135){
#else
					if(pwm-- > 50){
#endif
						set_lift_pwm_direct(pwm);
						pwm_timeout = LIFT_ADDED_DELAY;
						LiftStructure.timeout += LIFT_ADDED_DELAY;
					}
					else
						LiftStructure.state_mashine = stmLiftRunCheckTimeout;
				}
				break;
		case stmLiftRunCheckTimeout:
			if(LiftStructure.timeout > (LIFT_TIMEOUT - LIFT_START_TIMEOUT))
				break;
			LiftStructure.command_position = get_lift_position();
			if(LiftStructure.command_position == LiftStructure.new_command_position){
				disable_lift();
				if(LiftStructure.new_command_position == LiftCommandUp)
				LiftStructure.state = Lift_UP;
				else
				LiftStructure.state = Lift_DOWN;
				LiftStructure.new_command = false;
				LiftStructure.state_mashine = stmLiftIddle;
			}
			if (LiftStructure.timeout == 0){
				disable_lift();
				LiftStructure.state = Lift_ERROR;
				LiftStructure.new_command = false;
				LiftStructure.state_mashine = stmLiftError;
			}
			break;
	}
	if(LiftStructure.timeout)
		LiftStructure.timeout--;
	if(pwm_timeout)
		pwm_timeout--;
}

bool check_lift_bussy(){
	return LiftStructure.new_command;
}

tLiftState get_lift_state(){
	return LiftStructure.state;
}

tLiftStructure get_lift_structure(){
	return LiftStructure;
}

void set_lift_pwm_direct(uint8_t pwm){
	if(pwm < 50)
		pwm = 50;
	if(pwm > 135)
		pwm = 135;
	OCR1A = pwm;
}

void set_lift_enble(){
	PORTC |= (1 << PORT2);
}

void disable_lift(){
	OCR1A = 0;
	PORTC &= ~(1 << PORT2);											// lift enable pwm pin off
}

#ifdef FUTABA_S3003
void set_lift_position_up(){
	OCR1A = 50;
	PORTC |= (1 << PORT2);											// lift enable pwm pin on
}
#else
void set_lift_position_up(){
	OCR1A = 135;
	PORTC |= (1 << PORT2);											// lift enable pwm pin on
}
#endif

#ifdef FUTABA_S3003
void set_lift_position_down(){
	OCR1A = 135;
	PORTC |= (1 << PORT2);											// lift enable pwm pin on
}
#else
void set_lift_position_down(){
	OCR1A = 50;
	PORTC |= (1 << PORT2);											// lift enable pwm pin on
}
#endif