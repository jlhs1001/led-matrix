# led-matrix
The LED Matrix library allows you to easily interact with an 8x8 LED Matrix using .NET

Feature highlights:<br>
  - Write Bitmaps to display
  - Write byte arrays to display
  - Clear display
  - Set rows
  - Set Columns

And more helpful features

## How to install:

### Using Nuget

Using Nuget, you can find the ledmatrix package with the following command:
```powershell
Find-Package ledmatrix
```

Run the install command:
```powershell
Install-Package ledmatrix -ProjectName <project to install to>
```

### From github
Install .xip file
put in project
etc.
TODO: Finish

## Getting started

### Initialize Display

### LedMatrix(string comPort, int baudRate, int din, int cs, int clk, int brightness)

```C#
LedMatrix lm = new LedMatrix("COM8", 9600, 11, 7, 13, 0);
// --stuff--
```

### Hello World
```C#
LedMatrix lm = new LedMatrix("COM8", 9600, 11, 7, 13, 0);
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
### Write(string filePath, int pixelWidth)


Takes an array of bytes and writes the bytes to the ledmatrix display.

```C#
LedMatrix lm = new LedMatrix("COM8", 9600, 11, 7, 13, 0);
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
LedMatrix lm = new LedMatrix("COM8", 9600, 11, 7, 13, 0);
lm.Write("C:\dev\my_bmp.bmp", 1);
```

### ClearScreen()

Sets all leds on the display to off
```C#
LedMatrix lm = new LedMatrix("COM8", 9600, 11, 7, 13, 0);
lm.ClearScreen();
```

### SetRow(int row, byte value)

Takes a row and a byte to write to the row,
each bit corresponding to an LED
```C#
LedMatrix lm = new LedMatrix("COM8", 9600, 11, 7, 13, 0);
lm.SetRow(5, 0b00101010);
```

### SetColumn(int column, byte value)

Takes a row and a byte to write to the column,
each bit corresponding to an LED
```C#
LedMatrix lm = new LedMatrix("COM8", 9600, 11, 7, 13, 0);
lm.SetColumn(5, 0b00101010);
```

### ConvertToBitmap(string filePath, string outPath)
Takes a path to an image and saves it as a bitmap
```C#
LedMatrix lm = new LedMatrix("COM8", 9600, 11, 7, 13, 0);
lm.ConvertToBitmap("C:\dev\my_png.png", "C:\dev\my_bmp.bmp");
```
