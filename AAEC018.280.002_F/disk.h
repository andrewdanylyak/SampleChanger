/*
 * disk.h
 *
 * Created: 16.08.2021 12:16:36
 *  Author: Andrii
 */ 


#ifndef DISK_H_
#define DISK_H_

#define										DISK_TIMEOUT								15000
#define										DISK_PWM										60
#define										DISK_PWM_SLOW								50
#define										DISK_DELAY_BD								500
#define										DISK_DELAY_BS								250
#define										DISK_PRESENT_COUNER					5000
#define										DISK_OFFEST_LEFT						45
#define										DISK_OFFEST_RIGHT						45
#define										DISK_CONST_SPEED_LARGE			80
#define										DISK_CONST_SPEED_SMALL			35

typedef enum{
	pwmSetUpdate,
	pwmClear,
	pwmReset
}tSoftPwmState;

typedef enum{
	DiskOk,
	DiskReady,
	DiskAbsend,
	DiskMooving,
	DiskError
}tDiskState;

typedef enum{
	stmDiskIddle,
	stmDiskStart,
	stmDiskUp,
	stmDiskCheckPosition,
	stmDelayBeforStart,
	stmDiskRotation,
	stmDelayBeforeDown,
	stmDiskDown,
	stmDiskError
} tDiskStateMashine;

typedef enum{
	RotationLeft,
	RotationRight
}tDiskRotation;

typedef struct {
	tDiskState state;
	uint16_t timeout;
	uint8_t motor_pwm;
	uint8_t motor_const_PWM;
	uint8_t led_sync_pwm;
	uint8_t led_data_pwm;
	tDiskStateMashine state_mashine;
	uint32_t sin_cos_counter;
	uint32_t prev_sin_cos_counter;
	bool enable_rotation;
	uint8_t current_position;
	uint8_t new_position;
	bool new_command;
	tDiskRotation disk_rotation;
	bool delayed_stop;
	uint8_t disk_present_counter;
	uint8_t delta;
}tDiskSturtture;

void init_disk();
void init_disk_power_on_value();
void disk_isr_update_motor_pwm();
void disk_isr_update_led_sync_pwm();
void disk_isr_update_led_data_pwm();
void disk_isr_working_cycle();
void disk_set_new_position(uint8_t pos);
uint8_t get_disk_label_position();
void disk_set_left_rotation(uint8_t pwm);
void disk_set_right_rotation(uint8_t pwm);
void disk_stop_rotation();
uint32_t disk_get_sin_cos();
void reset_sin_cos();
tDiskState get_disk_state();
void set_disk_enable_disk_rotation();
bool get_disk_busy();
tDiskRotation check_rotation_side(tDiskSturtture disk);
void isr_calibrate_motor_pwm();

#endif /* DISK_H_ */