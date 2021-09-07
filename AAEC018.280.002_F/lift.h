/*
 * lift.h
 *
 * Created: 13.08.2021 13:06:32
 *  Author: Andrii
 */ 


#ifndef LIFT_H_
#define LIFT_H_

#define LIFT_START_TIMEOUT	250
#define LIFT_TIMEOUT				2000
#define LIFT_ADDED_DELAY		20

typedef enum{
	LiftCommandDown,
	LiftCommandUp
}tLiftCommandPosition;

typedef enum{
	Lift_OK,
	Lift_DOWN,
	Lift_UP,
	Lift_ERROR
}tLiftState;

typedef enum{
	stmLiftIddle,
	stmLitStart,
	stmLiftRunningCeckPosition,
	stmLiftRunCommand,
	stmLiftDownSlowly,
	stmLiftRunCheckTimeout,
	stmLiftError
}tLiftStateMashine;

typedef enum{
	astmLiftNotChecked,
	astmLiftBlind0,
	astmLiftGotoNextPosition0,
	astmLiftDelayBeforePosition1,
	astmLiftBlind1,
	astmLiftGotoNextPosition1,
	astmLiftCheckedOk,
	astmLiftCheckFailed
}tLiftAutotestState;

typedef struct{
	tLiftState state;
	tLiftAutotestState autocheck_state;
	tLiftCommandPosition command_position;
	tLiftCommandPosition new_command_position;
	bool new_command;
	uint16_t timeout;
	tLiftStateMashine state_mashine;
}tLiftStructure;

void init_lift_pwm();
void init_lift();
tLiftStructure get_lift_structure();
void update_lift_structure(tLiftStructure state);
tLiftCommandPosition get_lift_position();
tLiftState get_lift_state();
void disable_lift();
void set_lift_position_up();
void set_lift_position_down();
void set_lift_pwm_direct(uint8_t pwm);
void set_lift_enble();
void set_new_lift_position(tLiftCommandPosition new_pos);
bool check_lift_bussy();

void lift_isr_routine();

/*new added 03.09.2021*/
void autotest_lift();

#endif /* LIFT_H_ */