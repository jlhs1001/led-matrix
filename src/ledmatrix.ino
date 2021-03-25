#include <LedControl.h>

int incomingbyte = 0;

int DIN = 11;
int CS = 7;
int CLK = 13;

byte data[10] = {};
int dataIndex = 0;

bool dataMode = false;
bool delayMode = false;
bool setupMode = false;
bool singleLedMode = false;
bool rowMode = false;
bool columnMode = false;
bool animationDataMode = false;

LedControl lc = LedControl(DIN, CLK, CS, 0);

void commandRecieved(int incomingByte) {
    if (setupMode == true) {
        data[dataIndex] = incomingByte;
        dataIndex++;
        if (dataIndex > 2) {
            DIN = data[0];
            CS = data[1];
            CLK = data[2];
            lc = LedControl(DIN, CS, CLK, 0);
            dataIndex = 0;
            setupMode = false;
        }
    } else if (dataMode == true) {
        data[dataIndex] = incomingByte;
        dataIndex++;
        if (dataIndex > 7) {
            for (int i = 0; i < 7; i++) {
              lc.setRow(0, i, data[i]);
            }
            dataIndex = 0;
            dataMode = false;
        }
    } else if (delayMode == true) {
        data[dataIndex] = incomingByte;
        dataIndex++;
        if (dataIndex > 1) {
            int value = 0;
            value += data[0] << 8;
            value += data[1];
            delay(value);
            dataIndex = 0;
            delayMode = false;
        }
    } else if (singleLedMode == true) {
        data[dataIndex] = incomingByte;
        dataIndex++;
        if (dataIndex > 2) {
            lc.setLed(0, data[0], data[1], (data[2] == 1));
            dataIndex = 0;
            dataMode = false;
        }
    } else if (rowMode == true) {
        data[dataIndex] = incomingByte;
        dataIndex++;
        if (dataIndex > 1) {
            lc.setRow(0, data[0], data[1]);
            dataIndex = 0;
            dataMode = false;
        }
    } else if (columnMode == true) {
        data[dataIndex] = incomingByte;
        dataIndex++;
        if (dataIndex > 1) {
            lc.setColumn(0, data[0], data[1]);
            dataIndex = 0;
            dataMode = false;
        }
    } else if (animationMode == true) {
        if (incomingbyte == B00001001 && animationDataMode == false) {
            animationMode = false;
        } else {
            if (animationDataMode == true) {
                data[dataIndex] = incomingByte;
                dataIndex++;
            }
            if (dataIndex > 7) {
                for (int i = 0; i < 7; i++) {
                    lc.setRow(0, i, data[i]);
                }
                dataIndex = 0;
            }
        }
    }
    switch (incomingByte) {
        case B00000001:
            setupMode = true;
            break;
        case B00000010:
            dataMode = true;
            break;
        case B00000011:
            delayMode = true;
            break;
        case B00000100:
            singleLedMode = true;
            break;
        case B00000101:
            rowMode = true;
            break;
        case B00000110:
            columnMode = true;
            break;
        case B00000111:
            lc.clearDisplay(0);
            break;
        case B00001000:
            animationMode = true;
            break;
    }
}

void setup() {
  lc.shutdown(0, false);
  lc.setIntensity(0, 0);
  lc.clearDisplay(0);
  Serial.begin(9600);

  pinMode(LED_BUILTIN, OUTPUT);
}

void loop() {
    if (Serial.available() > 0) {
        incomingbyte = Serial.read();
        commandRecieved(incomingbyte);
    }
}