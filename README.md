# led-matrix
The LED Matrix library allows you to easily interact with an 8x8 LED Matrix using .NET

Feature highlights:<br>
  - Write Bitmaps to display
  - Write byte arrays to display
  - Clear display
  - Set rows
  - Set Columns

And more helpful features

## Setup:

To use the LedMatrix you first grab the dll file above, and put it into your project.
For more information on how to set up a dll file, refer [here](https://www.c-sharpcorner.com/UploadFile/1e050f/creating-and-using-dll-class-library-in-C-Sharp/).

You also have to set up the [LedControl](https://github.com/wayoda/LedControl) library. How?

Grab the zip file from that repo, then in the Arduino IDE, navigate to:

Sketch > Include Library > Add .ZIP Library. Then select "Add .ZIP Library".

Find the .ZIP file, select it, paste the "ledcontrol.ino" file into the IDE, and you're good to upload.

For more information on .ZIP libraries, refer [here](https://www.arduino.cc/en/guide/libraries).

## Getting started

### Initialize Display

### LedMatrix(string comPort, int baudRate, int din, int cs, int clk, int brightness)

```C#
LedMatrix lm = new LedMatrix("COM8", 9600);
// --stuff--
```

### Hello World
```C#
LedMatrix lm = new LedMatrix("COM8", 9600);
byte[] data = {
  0b10101010,
  0b01010101,
  0b00000000,
  0b00000000,
  0b00000000,
  0b00000000,
  0b00000000,
  0b00000000
};
lm.Write(data);
stuff
```

### Write(byte[] data)
### Write(string filePath, int pixelSize)


Takes an array of bytes and writes the bytes to the ledmatrix display.

```C#
LedMatrix lm = new LedMatrix("COM8", 9600);
lm.Write(new byte[] {
  0b10101010,
  0b01010101,
  0b00000000,
  0b00000000,
  0b00000000,
  0b00000000,
  0b00000000,
  0b00000000
});
```


Takes a filepath to a bitmap as well as the pixelwidth, converts to a byte array, then writes the data to the ledmatrix display.
```C#
LedMatrix lm = new LedMatrix("COM8", 9600);
lm.Write("C:\dev\my_img.png", 1);
```

### ClearScreen()

Sets all leds on the display to off
```C#
LedMatrix lm = new LedMatrix("COM8", 9600);
lm.ClearScreen();
```

### SetRow(int row, byte value)

Takes a row and a byte to write to the row,
each bit corresponding to an LED
```C#
LedMatrix lm = new LedMatrix("COM8", 9600);
lm.SetRow(5, 0b00101010);
```

### SetColumn(int column, byte value)

Takes a row and a byte to write to the column,
each bit corresponding to an LED
```C#
LedMatrix lm = new LedMatrix("COM8", 9600);
lm.SetColumn(5, 0b00101010);
```

### WriteAnimation(string filePath, int frameDelay, int pixelSize)

Takes the path to the .gif, the length of time before the next frame, and the pixel size.

```C#
LedMatrix lm = new LedMatrix("COM8", 9600);
lm.WriteAnimation("C:\dev\my_gif.gif", 1000, 1);
```
