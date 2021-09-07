/*
 * i2c.c
 *
 * Created: 17.08.2021 13:36:29
 *  Author: Andrii
 */ 

 #include "includes.h"

static char ID[] = "ElvaX 2 carousel 1.0.0\0x00";

#define TWI_ADDRESS 0x70
#define TWI_BUFFER_SIZE 150

volatile uint8_t twi_buffer[TWI_BUFFER_SIZE];
volatile uint8_t temp_buffer[TWI_BUFFER_SIZE];
volatile uint8_t twi_input_ptr;
volatile uint8_t twi_output_ptr;

void init_twi(void){
	TWBR = 0x48; // SCL=50kHz [p.235,216]
	TWCR = 0x00;
	TWAR = TWI_ADDRESS;
	TWSR = 0x00;
	TWCR = (1 << TWINT) | (1 << TWEN) | (1 << TWIE) | (1 << TWEA);
}

uint8_t calculate_checksum(uint8_t length,volatile uint8_t* data){
	uint8_t crc = 0;
	for(uint8_t i = 0; i < length; i++)
	crc += data[i];
	return crc;
}

void prepare_output_buffer(uint8_t command, uint8_t status, uint8_t length, uint8_t* data){
	uint8_t crc = 0;
	memset(twi_buffer,0x00,TWI_BUFFER_SIZE);
	twi_buffer[0] = status;
	twi_buffer[1] = command;
	memcpy(&twi_buffer[2],data,length);
	for(uint8_t i = 0; i < length + 2; i++)
	crc += twi_buffer[i];
	crc = (uint8_t)(0 - crc);
	twi_buffer[length + 2] = crc;
}

void twi_engine(){
	uint8_t status = STATUS_OK;
	uint8_t tx_length = 0;
	uint8_t twi_command;
	uint8_t twi_crc;
	twi_command = twi_buffer[0];
	if(calculate_checksum(twi_input_ptr,twi_buffer) != 0)
		status|=STATUS_CRC_ERR;
	if(status != STATUS_OK){
		prepare_output_buffer(twi_command,status,tx_length,temp_buffer);
		return;
	}
	switch(twi_command){
		case CAR_GET_ID:
			if(twi_input_ptr != 2){
				status|=STATUS_WRONG_LENGTH;
				break;
			}
			strcpy((char*)temp_buffer,ID);
			tx_length = 128;
			break;
		case CAR_GET_STATUS:
			if(twi_input_ptr != 2){
				status|=STATUS_WRONG_LENGTH;
				break;
			}
			temp_buffer[0] = (uint8_t)get_cell_state();	 // cell_rotator_state;
			temp_buffer[1] = (uint8_t)get_lift_state();	// lift_state;
			temp_buffer[2] = (uint8_t)get_disk_state();	// table_rotator_state;
			temp_buffer[3] = 0;
			tx_length = 4;
			break;
		case CAR_CELL_ROTATOR_POSITION:
			if((twi_input_ptr != (2 + 2))){
				status|=STATUS_WRONG_LENGTH;
				break;
			}
			cell_update_control_values((bool)twi_buffer[1], twi_buffer[2]);
			break;
		case CAR_LIFT_UP_DOWN:
			if(twi_input_ptr != 3){
				status|=STATUS_WRONG_LENGTH;
				break;
			}
			set_new_lift_position((tLiftCommandPosition)twi_buffer[1]);
			break;
		case CAR_SET_POSITION:
			if(twi_input_ptr != 4){
				status|=STATUS_WRONG_LENGTH;
				break;
			}
			disk_set_new_position(twi_buffer[1]);
			break;
		case CAR_SET_SPEED:
			if(twi_input_ptr != 5){
				status|=STATUS_WRONG_LENGTH;
				break;
			}
			break;
		default:
			status|=STATUS_WRONG_COMMAND;
			break;
	}
	prepare_output_buffer(twi_command,status,tx_length,temp_buffer);
}

ISR (TWI_vect)
{
	switch(TWSR)
	{
		case 0x68:
		case 0x60:
		twi_input_ptr = 0;
		memset(twi_buffer,0,TWI_BUFFER_SIZE);
		TWCR = (1 << TWEA)|(0 << TWSTA)|(0 << TWSTO)|(0 << TWWC)|(1 << TWEN)|(1 << TWIE)|(1 << TWINT);
		return;
		case 0x80:
		*(twi_buffer + twi_input_ptr++) = TWDR;
		TWCR = (1 << TWEA)|(0 << TWSTA)|(0 << TWSTO)|(0 << TWWC)|(1 << TWEN)|(1 << TWIE)|(1 << TWINT);
		return;
		case 0xA0:
		twi_engine();
		TWCR = (1 << TWEA)|(0 << TWSTA)|(0 << TWSTO)|(0 << TWWC)|(1 << TWEN)|(1 << TWIE)|(1 << TWINT);
		return;
		case 0xA8:
		case 0xB0:
		twi_output_ptr = 0;
		TWDR = *(twi_buffer + twi_output_ptr++);
		TWCR = (1 << TWEA)|(0 << TWSTA)|(0 << TWSTO)|(0 << TWWC)|(1 << TWEN)|(1 << TWIE)|(1 << TWINT);
		return;
		case 0xB8:
		TWDR = *(twi_buffer + twi_output_ptr++);
		TWCR = (1 << TWEA)|(0 << TWSTA)|(0 << TWSTO)|(0 << TWWC)|(1 << TWEN)|(1 << TWIE)|(1 << TWINT);
		return;
		case 0xC0:
		TWCR = (1 << TWEA)|(0 << TWSTA)|(0 << TWSTO)|(0 << TWWC)|(1 << TWEN)|(1 << TWIE)|(1 << TWINT);
		return;
		case 0xC8:
		TWCR = (1 << TWEA)|(0 << TWSTA)|(0 << TWSTO)|(0 << TWWC)|(1 << TWEN)|(1 << TWIE)|(1 << TWINT);
		return;
		case 0x00:
		TWCR = (1 << TWEA)|(0 << TWSTA)|(1 << TWSTO)|(0 << TWWC)|(1 << TWEN)|(1 << TWIE)|(1 << TWINT);
		return;
		default:
		TWCR = (1 << TWEA)|(0 << TWSTA)|(0 << TWSTO)|(0 << TWWC)|(1 << TWEN)|(1 << TWIE)|(1 << TWINT);
	}
}