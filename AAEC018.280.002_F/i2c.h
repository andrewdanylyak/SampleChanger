/*
 * i2c.h
 *
 * Created: 17.08.2021 13:36:11
 *  Author: Andrii
 */ 


#ifndef I2C_H_
#define I2C_H_

enum CAROUSEL_COMMANDS{
	CAR_GET_ID,  // 0x00
	CAR_GET_STATUS,
	CAR_CELL_ROTATOR_POSITION,
	CAR_LIFT_UP_DOWN,
	CAR_SET_POSITION,
	CAR_SET_SPEED  // 0x05
};

enum TWI_STATUS_ERRORS{
	STATUS_OK,
	STATUS_CRC_ERR = (1 << 1),
	STATUS_WRONG_COMMAND = (1 << 2),
	STATUS_WRONG_DATA = (1 << 3),
	STATUS_WRONG_LENGTH = (1 << 4)
};

void init_twi(void);



#endif /* I2C_H_ */