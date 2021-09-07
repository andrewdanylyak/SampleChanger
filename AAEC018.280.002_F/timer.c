/*
 * timer.c
 *
 * Created: 13.08.2021 13:36:28
 *  Author: Andrii
 */ 

 #include "includes.h"

 volatile uint32_t m_delay = 0;

 void init_timers(){
		//----------Timer0 init----------//
		TCCR0A = (0 << COM0A1)|(0 << COM0A0)|(0 << COM0B1)|(0 << COM0B0)|(1 << WGM01)|(0 << WGM00);
		TCCR0B = (0 << FOC0A)|(0 << FOC0B)|(0 << WGM02)|(0 << CS02)|(1 << CS01)|(1 << CS00); //  clkI/O/64 (From prescaler); Ttimer0=(8MHz/64)^-1=8us  [p.109]
		OCR0A = 62*2; // 62*Ttimer0=0.496ms
		TIMSK0 = (0 << OCIE0B)|(1 << OCIE0A)|(0 << TOIE0);
		//----------Timer2 init----------//
		TCCR2A = (0 << COM2A1)|(0 << COM2A0)|(0 << COM2B1)|(0 << COM2B0)|(1 << WGM21)|(0 << WGM20);
		TCCR2B = (0 << FOC2A)|(0 << FOC2B)|(0 << WGM22)|(0 << CS22)|(1 << CS21)|(0 << CS20); // clkT2S/8 (From prescaler); Ttimer2=(8MHz/8)^-1=1us [p.158]
		OCR2A = 80; // 80*Ttimer2=80us;
		TIMSK2 = (0 << OCIE2B)|(1 << OCIE2A)|(0 << TOIE2);
 }

 ISR (TIMER0_COMPA_vect){
		if(m_delay != 0)
			m_delay--;
		lift_isr_routine();
		disk_isr_working_cycle();
		isr_calibrate_motor_pwm();
 }

 void m_delay_ms(uint32_t t)
 {
	m_delay = t;
	while(true)
		if(m_delay == 0)
			return;
 }

ISR (TIMER2_COMPA_vect){
	disk_isr_update_motor_pwm();
	disk_isr_update_led_sync_pwm();
	disk_isr_update_led_data_pwm();
	cell_isr_routine();
}