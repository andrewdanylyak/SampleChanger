/*
 * AAEC018.280.002_F.c
 *
 * Created: 13.08.2021 13:03:07
 * Author : Andrii
 */ 

#include "includes.h"

static void init_clock();

int main(void){
	init_clock();
	init_gpio();
	init_lift();
	init_cell();
	init_disk();
	init_timers();
	init_twi();
	sei();
	cell_update_control_values(true, 100);
	autotest_lift();
	init_disk_power_on_value();

	while (true){
	}
}

void init_clock(){
	MCUCR = 0;
	CLKPR=(1<<CLKPCE);
	CLKPR=(0<<CLKPCE) | (0<<CLKPS3) | (0<<CLKPS2) | (0<<CLKPS1) | (0<<CLKPS0);
}


