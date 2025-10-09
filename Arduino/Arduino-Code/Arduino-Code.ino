#include <Adafruit_LSM303_Accel.h>
#include <Adafruit_Sensor.h>
#include <Wire.h>

/* Assign a unique ID to this sensor at the same time */
Adafruit_LSM303_Accel_Unified accel = Adafruit_LSM303_Accel_Unified(54321);
int whichDirectionRight = 200;
int whichDirectionLeft = 200;

void displaySensorDetails(void) {
  sensor_t sensor;
  accel.getSensor(&sensor);
  Serial.println("------------------------------------");
  Serial.print("Sensor:       ");
  Serial.println(sensor.name);
  Serial.print("Driver Ver:   ");
  Serial.println(sensor.version);
  Serial.print("Unique ID:    ");
  Serial.println(sensor.sensor_id);
  Serial.print("Max Value:    ");
  Serial.print(sensor.max_value);
  Serial.println(" m/s^2");
  Serial.print("Min Value:    ");
  Serial.print(sensor.min_value);
  Serial.println(" m/s^2");
  Serial.print("Resolution:   ");
  Serial.print(sensor.resolution);
  Serial.println(" m/s^2");
  Serial.println("------------------------------------");
  Serial.println("");
  delay(500);
}

void setup(void) {
// #ifndef ESP8266
//   while (!Serial)
//     ; // will pause Zero, Leonardo, etc until serial console opens
// #endif
  Serial.begin(115200);
  Serial.println("Accelerometer Test");
  Serial.println("");

  /* Initialise the sensor */
  if (!accel.begin()) {
    /* There was a problem detecting the ADXL345 ... check your connections */
    Serial.println("Ooops, no LSM303 detected ... Check your wiring!");
    while (1)
      ;
  }

  /* Display some basic information on this sensor */
  displaySensorDetails();

  accel.setRange(LSM303_RANGE_4G);
  Serial.print("Range set to: ");
  lsm303_accel_range_t new_range = accel.getRange();
  switch (new_range) {
  case LSM303_RANGE_2G:
    Serial.println("+- 2G");
    break;
  case LSM303_RANGE_4G:
    Serial.println("+- 4G");
    break;
  case LSM303_RANGE_8G:
    Serial.println("+- 8G");
    break;
  case LSM303_RANGE_16G:
    Serial.println("+- 16G");
    break;
  }

  accel.setMode(LSM303_MODE_NORMAL);
  Serial.print("Mode set to: ");
  lsm303_accel_mode_t new_mode = accel.getMode();
  switch (new_mode) {
  case LSM303_MODE_NORMAL:
    Serial.println("Normal");
    break;
  case LSM303_MODE_LOW_POWER:
    Serial.println("Low Power");
    break;
  case LSM303_MODE_HIGH_RESOLUTION:
    Serial.println("High Resolution");
    break;
  }
}

void loop(void) {
  /* Get a new sensor event */
  sensors_event_t event;
  accel.getEvent(&event);

  /* Display the results (acceleration is measured in m/s^2) */
  // Serial.print("X: ");
  // Serial.print(event.acceleration.x);
  // Serial.print("  ");
  // Serial.print("Y: ");
  // Serial.print(event.acceleration.y);
  // Serial.print("  ");
  // Serial.print("Z: ");
  // Serial.print(event.acceleration.z);
  // Serial.print("  ");
  // Serial.println("m/s^2");

  float roll_rad = atan2(event.acceleration.y, event.acceleration.z);
  float pitch_rad = atan2(-event.acceleration.x, sqrt(event.acceleration.y*event.acceleration.y + event.acceleration.z*event.acceleration.z));

  // Convert radians to degrees
  float roll_deg = roll_rad * 180.0 / M_PI;
  float pitch_deg = pitch_rad * 180.0 / M_PI;

  // Print results to the Serial Monitor
  // Serial.print("Roll: ");
  // Serial.print(roll_deg);
  // Serial.print(" degrees, Pitch: ");
  Serial.print("PITCH:");
  Serial.println(pitch_deg);



  int rightArmVal = analogRead(A0);
  int leftArmVal = analogRead(A5);
  // Serial.print("leftArmVal: ");
  // Serial.print(leftArmVal);
  // Serial.print("   rightArmVal: ");
  // Serial.println(rightArmVal);

  if (rightArmVal >= 640 && rightArmVal <= 650) {
    whichDirectionRight = 4;
  } else if (rightArmVal >= 625 && rightArmVal <= 635) {
    whichDirectionRight = 3;
  } else if (rightArmVal >= 315 && rightArmVal <= 325) {
    whichDirectionRight = 2;
  } else if (rightArmVal >= 2 && rightArmVal <= 6) {
    whichDirectionRight = 1;
  } else {
    whichDirectionRight = 0;
  };

  if (leftArmVal >= 640 && leftArmVal <= 650) {
    whichDirectionLeft = 4;
  } else if (leftArmVal >= 625 && leftArmVal <= 635) {
    whichDirectionLeft = 3;
  } else if (leftArmVal >= 315 && leftArmVal <= 325) {
    whichDirectionLeft = 2;
  } else if (leftArmVal >= 2 && leftArmVal <= 6) {
    whichDirectionLeft = 1;
  } else {
    whichDirectionLeft = 0;
  };

  // Serial.print("Left Arm: ");
  // Serial.print(whichDirectionLeft);
  // Serial.print("   Right Arm: ");
  // Serial.println(whichDirectionRight);




  /* Delay before the next sample */
  delay(20);
}