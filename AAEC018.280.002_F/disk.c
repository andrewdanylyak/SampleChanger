/*
 * disk.c
 *
 * Created: 16.08.2021 12:17:09
 *  Author: Andrii
 */ 

 #include "includes.h"

 volatile tDiskSturtture DiskStrukture;

 void init_disk(){
	DiskStrukture.motor_pwm = 0;
	DiskStrukture.led_data_pwm = 100;
	DiskStrukture.led_sync_pwm = 100;
	DiskStrukture.prev_sin_cos_counter = 0;
	DiskStrukture.motor_const_PWM = DISK_PWM;
	PCICR |= (1 << PCIE0)|(1 << PCIE1);
	PCMSK0 |= (1 << PCINT6)|(1 << PCINT7);
	PCMSK1 |= (1 << PCINT8);
 }

 void init_disk_power_on_value(){
	DiskStrukture.current_position = get_disk_label_position();
 }

 void isr_calibrate_motor_pwm(){
	static uint16_t counter = 0;
	uint32_t sSinCoscounter = 0;
	if((counter < 10)&&(DiskStrukture.motor_pwm))
		counter++;
	else if(DiskStrukture.motor_pwm){
		sSinCoscounter = DiskStrukture.sin_cos_counter - DiskStrukture.prev_sin_cos_counter;
		DiskStrukture.prev_sin_cos_counter = DiskStrukture.sin_cos_counter;
		if(DiskStrukture.delta > 1){
			if((sSinCoscounter > (DISK_CONST_SPEED_LARGE + 5))&&(DiskStrukture.motor_pwm > 10))
				DiskStrukture.motor_pwm--;
			if((sSinCoscounter < (DISK_CONST_SPEED_LARGE - 5))&&(DiskStrukture.motor_pwm < 100))
				DiskStrukture.motor_pwm++;
			}
		else{
			if((sSinCoscounter > (DISK_CONST_SPEED_SMALL))&&(DiskStrukture.motor_pwm > 5))
				DiskStrukture.motor_pwm--;
			if((sSinCoscounter < (DISK_CONST_SPEED_SMALL))&&(DiskStrukture.motor_pwm < 100))
				DiskStrukture.motor_pwm++;
		}
		counter = 0;
	}
 }

 uint8_t get_disk_label_position(){
	uint8_t return_value;
	if(PINC&(1 << PC0) == (1 << PC0)){
		return 0xff;
	}
	else{
		return_value =  (PIND&0x07);							//m1,m2,m4
		return_value |= (PIND&0x10)>>1;						//m8
		return_value = 15 - return_value;
		return return_value;
	}
	return 0xff;
 }

 void disk_set_left_rotation(uint8_t pwm){
	DiskStrukture.disk_rotation = RotationLeft;
	reset_sin_cos();
	DiskStrukture.delayed_stop = false;
	PORTD	&=	~(1 << PD7);											// break pin
	PORTB |= (1 << PB0);													// directionPin
	DiskStrukture.motor_pwm = pwm;
 }

void disk_set_right_rotation(uint8_t pwm){
	DiskStrukture.disk_rotation = RotationRight;
	reset_sin_cos();
	DiskStrukture.delayed_stop = false;
	PORTD	&=	~(1 << PD7);												// break pin
	PORTB &= ~(1 << PB0);													// directionPin
	DiskStrukture.motor_pwm = pwm;
}

 void set_disk_enable_disk_rotation(){
	DiskStrukture.disk_present_counter = 0;
	DiskStrukture.enable_rotation = true;
 }

 void disk_stop_rotation(){
	reset_sin_cos();
	//DiskStrukture.motor_pwm = DiskStrukture.motor_pwm/2;
	DiskStrukture.delayed_stop = true;
 }

 uint32_t disk_get_sin_cos(){
	return DiskStrukture.sin_cos_counter;
 }

void reset_sin_cos(){
	DiskStrukture.sin_cos_counter = 0;
	DiskStrukture.prev_sin_cos_counter = 0;
}

 tDiskState get_disk_state(){
	return DiskStrukture.state;
 }

 void disk_set_new_position(uint8_t pos){
	if(get_disk_busy()){
		return;
	}
	if((pos == 16)&&(get_lift_state() == Lift_UP)){
		if((PIND&(1 << PD7)) == (1 << PD7)){
			PORTD &= ~(1 << PD7);
			DiskStrukture.motor_pwm = DiskStrukture.motor_const_PWM;
		}
		else{
			PORTD	|=	(1 << PD7);
			DiskStrukture.motor_pwm = 0;
		}
		return;
	}
	if((get_disk_label_position() == 0xff)||(get_disk_label_position() == pos)){
		DiskStrukture.state = DiskReady;
		DiskStrukture.state_mashine = stmDiskIddle;
		return;
	}
	DiskStrukture.new_position = pos;
	DiskStrukture.state_mashine = stmDiskStart;
	DiskStrukture.new_command = true;
 }

 void disk_isr_working_cycle(){
	static uint16_t delay_before = 0;
	switch(DiskStrukture.state_mashine){
		case stmDiskIddle:
			return;
		case stmDiskStart:
			DiskStrukture.timeout = DISK_TIMEOUT;
			if(get_lift_state() != Lift_UP)
				set_new_lift_position(LiftCommandUp);
			DiskStrukture.state_mashine = stmDiskUp;
			DiskStrukture.state = DiskMooving;
			break;
		case stmDiskUp:
			if(!check_lift_bussy()&&(get_lift_state() != Lift_ERROR))
				DiskStrukture.state_mashine = stmDiskCheckPosition;
			if(DiskStrukture.timeout == 0)
				DiskStrukture.state_mashine = stmDiskError;
			break;
		case stmDiskCheckPosition:
			DiskStrukture.current_position = get_disk_label_position();
			if(check_rotation_side(DiskStrukture) == RotationRight){
				if(DiskStrukture.delta > 1)
					disk_set_right_rotation(DISK_PWM);
				else
					disk_set_right_rotation(DISK_PWM_SLOW);
			}
			else{
				if(DiskStrukture.delta > 1)
					disk_set_left_rotation(DISK_PWM);
				else
					disk_set_left_rotation(DISK_PWM_SLOW);
			}
			delay_before = DISK_DELAY_BS;
			DiskStrukture.state_mashine = stmDelayBeforStart;
			break;
		case stmDelayBeforStart:
			if(delay_before == 0){
					set_disk_enable_disk_rotation();
					DiskStrukture.state_mashine = stmDiskRotation;
				}
			break;
		case  stmDiskRotation:
			if(DiskStrukture.enable_rotation == false){
				DiskStrukture.state_mashine = stmDelayBeforeDown;
				delay_before = DISK_DELAY_BD;
				}
			if(DiskStrukture.timeout == 0)
				DiskStrukture.state_mashine = stmDiskError;
			if((DiskStrukture.timeout < (DISK_TIMEOUT - DISK_PRESENT_COUNER))&&(DiskStrukture.disk_present_counter == 0))
				DiskStrukture.state_mashine = stmDiskError;
			break;
		case stmDelayBeforeDown:
			if(delay_before == 0){
				set_new_lift_position(LiftCommandDown);
				DiskStrukture.state_mashine = stmDiskDown;
			}
			break;
		case stmDiskDown:
			if(!check_lift_bussy()&&(get_lift_state() != Lift_ERROR)){
				DiskStrukture.state = DiskReady;
				DiskStrukture.state_mashine = stmDiskIddle;
				DiskStrukture.new_command = false;
			}
			if(DiskStrukture.timeout == 0)
				DiskStrukture.state_mashine = stmDiskError;
			break;
		case stmDiskError:
			PORTD	|=	(1 << PD7);
			DiskStrukture.motor_pwm = 0;
			if(DiskStrukture.disk_present_counter == 0)
				DiskStrukture.state = DiskAbsend;
			else
				DiskStrukture.state = DiskError;
			DiskStrukture.state_mashine = stmDiskIddle;
			DiskStrukture.new_command = false;
			break;
	}
	if(DiskStrukture.timeout)
		DiskStrukture.timeout--;
	if(delay_before)
		delay_before--;
 }

 bool get_disk_busy(){
	return DiskStrukture.new_command;
 }

 tDiskRotation check_rotation_side(tDiskSturtture disk){
		int8_t delta = 0;
		delta = disk.current_position - disk.new_position;
		if(delta < 0)
			DiskStrukture.delta = disk.new_position - disk.current_position;
		else
			DiskStrukture.delta = disk.current_position - disk.new_position;
		if(delta < 0){
			if(delta < -7)
				return RotationRight;
			else
				return RotationLeft;
		}
		else{
			if(delta > 7)
				return RotationLeft;
			else
				return RotationRight;
		}
		return RotationRight;
 }

 void disk_isr_update_motor_pwm(){
	static uint8_t motor_pwm_counter = 0;
	static tSoftPwmState motor_pwm_state = 0;
	switch(motor_pwm_state){
	case pwmSetUpdate:
		if(DiskStrukture.motor_pwm == 0){
			PORTD &= ~(1 << PD6);
			return;
		}
		if(DiskStrukture.motor_pwm >= 100){
			PORTD |= (1 << PD6);
			return;
		}
		PORTD |= (1 << PD6);
		motor_pwm_counter = DiskStrukture.motor_pwm;
		motor_pwm_state = pwmClear;	
		break;
	case pwmClear:
		if(motor_pwm_counter)
			motor_pwm_counter--;
		else{
			PORTD &= ~(1 << PD6);
			motor_pwm_counter = 100 - DiskStrukture.motor_pwm;
			motor_pwm_state = pwmReset;
		}
		break;
	case pwmReset:
		if(motor_pwm_counter)
			motor_pwm_counter--;
		else
			motor_pwm_state = pwmSetUpdate;
		break;
	}
 }

 void disk_isr_update_led_sync_pwm(){
 	static uint8_t sync_pwm_counter = 0;
 	static tSoftPwmState sync_pwm_state = 0;
 	switch(sync_pwm_state){
	 	case pwmSetUpdate:
	 	if(DiskStrukture.led_sync_pwm == 0){
		 	PORTB &= ~(1 << PB2);
		 	return;
	 	}
	 	if(DiskStrukture.led_sync_pwm >= 100){
		 	PORTB |= (1 << PB2);
		 	return;
	 	}
	 	PORTB |= (1 << PB2);
	 	sync_pwm_counter = DiskStrukture.led_sync_pwm;
	 	sync_pwm_state = pwmClear;
	 	break;
	 	case pwmClear:
	 	if(sync_pwm_counter)
	 	sync_pwm_counter--;
	 	else{
		 	PORTB &= ~(1 << PB2);
		 	sync_pwm_counter = 100 - DiskStrukture.led_sync_pwm;
		 	sync_pwm_state = pwmReset;
	 	}
	 	break;
	 	case pwmReset:
	 	if(sync_pwm_counter)
	 	sync_pwm_counter--;
	 	else
	 	sync_pwm_state = pwmSetUpdate;
	 	break;
 	}
 }

 void disk_isr_update_led_data_pwm(){
 	static uint8_t data_pwm_counter = 0;
 	static tSoftPwmState data_pwm_state = 0;
 	switch(data_pwm_state){
	 	case pwmSetUpdate:
	 	if(DiskStrukture.led_data_pwm == 0){
		 	PORTD &= ~(1 << PD3);
		 	return;
	 	}
	 	if(DiskStrukture.led_data_pwm >= 100){
		 	PORTD |= (1 << PD3);
		 	return;
	 	}
	 	PORTD |= (1 << PD3);
	 	data_pwm_counter = DiskStrukture.led_data_pwm;
	 	data_pwm_state = pwmClear;
	 	break;
	 	case pwmClear:
	 	if(data_pwm_counter)
	 	data_pwm_counter--;
	 	else{
		 	PORTD &= ~(1 << PD3);
		 	data_pwm_counter = 100 - DiskStrukture.led_data_pwm;
		 	data_pwm_state = pwmReset;
	 	}
	 	break;
	 	case pwmReset:
	 	if(data_pwm_counter)
	 	data_pwm_counter--;
	 	else
	 	data_pwm_state = pwmSetUpdate;
	 	break;
 	}
 }

ISR (PCINT0_vect){
	DiskStrukture.sin_cos_counter++;
	if(DiskStrukture.delayed_stop){
		if((DiskStrukture.sin_cos_counter > DISK_OFFEST_RIGHT)&&(DiskStrukture.disk_rotation == RotationLeft)){
			PORTD	|=	(1 << PD7);													// break pin
			DiskStrukture.motor_pwm = 0;
			DiskStrukture.enable_rotation = false;
			DiskStrukture.delayed_stop = false;
			DiskStrukture.new_position = get_disk_label_position();
		}
		if((DiskStrukture.sin_cos_counter > DISK_OFFEST_LEFT)&&(DiskStrukture.disk_rotation == RotationRight)){
			PORTD	|=	(1 << PD7);													// break pin
			DiskStrukture.motor_pwm = 0;
			DiskStrukture.enable_rotation = false;
			DiskStrukture.delayed_stop = false;
			DiskStrukture.new_position = get_disk_label_position();
		}
	}
}

ISR (PCINT1_vect){
			uint8_t return_value;
			if(PINC&(1 << PC0) == (1 << PC0)){
				return;
			}
			else if((DiskStrukture.sin_cos_counter > 2700)&&(DiskStrukture.enable_rotation)){
				DiskStrukture.disk_present_counter++;
				return_value =  (PIND&0x07);							//m1,m2,m4
				return_value |= (PIND&0x10)>>1;						//m8
				return_value = 15 - return_value;
				if(DiskStrukture.new_position == return_value)
					disk_stop_rotation();
			}
			return;
}