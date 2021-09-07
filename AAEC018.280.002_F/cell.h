/*
 * cell.h
 *
 * Created: 16.08.2021 14:19:25
 *  Author: Andrii
 */ 


#ifndef CELL_H_
#define CELL_H_

typedef enum{
	CellOk,
	CellHandMoove,
	CellRotate,
	CellPark,
	CellError
}tCellState;

typedef struct{
	tCellState state;
	uint8_t cell_motor_pwm;
}tCellStructure;

void init_cell();
void cell_isr_routine();
void cell_update_control_values(bool park, uint8_t motor_pwm);
tCellState get_cell_state();

#endif /* CELL_H_ */