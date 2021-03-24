using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;


namespace ledmatrix
{
    public class LedMatrix
    {
        private readonly SerialPort _serialPort;

        private bool delay = false;

        /// <summary>
        /// Initialize LED Matrix
        /// </summary>
        /// <param name="comPort">Microcontrollers COM port</param>
        /// <param name="baudRate">Microcontrollers baud rate (default)</param>
        public LedMatrix(string comPort, int baudRate)
        {
            var sp = new SerialPort(comPort, baudRate);
            _serialPort = sp;
        }

        /// <summary>
        /// Writes byte[] to serial port asynchronously.
        /// </summary>
        /// <param name="data">Bytes to write to serial port</param>
        public void Write(byte[] data)
        {
            var task = Task.Run(() =>
            {
                // wait for delay to be over
                while (delay)
                {
                }

                _serialPort.Open();

                // Next 8 bytes will be displayed on LED matrix
                _serialPort.Write(new byte[] {0b00000001}, 0, 1);
                _serialPort.Write(data, 0, data.Length);

                _serialPort.Close();
            });
        }

        /// <summary>
        /// Write contents of image to serial port asynchronously.
        /// </summary>
        /// <param name="file">Path of file to be written to serial port.</param>
        /// <param name="pixelSize">The nth pixel to sample</param>
        public void Write(string file, int pixelSize)
        {
            // Convert image to bytearray
            var bitmap = new Bitmap(file, true);
            byte[] byteImage = BitmapToByteArray(bitmap, pixelSize);

            // Write final product to serial port.
            _serialPort.Open();
            _serialPort.Write(new byte[] {0b00000010}, 0, 1);
            _serialPort.Write(byteImage, 0, 8);
            _serialPort.Close();
        }


        /// <summary>
        /// Set single led to HIGH or LOW
        /// </summary>
        /// <param name="x">LED X value (0-7)</param>
        /// <param name="y">LED Y value (0-7)</param>
        /// <param name="val">Value to set LED to (1 or 0)</param>
        public void SetLed(int x, int y, int val)
        {
            // Sets a single led to either HIGH or LOW depending
            // on the input value
            _serialPort.Open();
            _serialPort.Write(new byte[] {0b00000100}, 0, 1);
            _serialPort.Write(new byte[] {(byte) x, (byte) y, (byte) val}, 0, 3);
            _serialPort.Close();
        }

        /// <summary>
        /// Sets a selected row to the value of a byte.
        /// </summary>
        /// <param name="row">Row to set</param>
        /// <param name="value">Value to set row to</param>
        public void SetRow(int row, byte value)
        {
            _serialPort.Open();
            _serialPort.Write(new byte[] {0b00000101}, 0, 1);
            _serialPort.Write(new byte[] {(byte) row, value}, 0, 2);
            _serialPort.Close();
        }

        /// <summary>
        /// Set a selected column to the value of a byte.
        /// </summary>
        /// <param name="column">Column to set</param>
        /// <param name="value">Value to set column to</param>
        public void SetColumn(int column, byte value)
        {
            _serialPort.Open();
            _serialPort.Write(new byte[] {0b00000110}, 0, 1);
            _serialPort.Write(new byte[] {(byte) column, value}, 0, 2);
            _serialPort.Close();
        }

        /// <summary>
        /// Set an asynchronous delay without affecting the rest of your program.
        /// </summary>
        /// <param name="length">The delay length</param>
        /// <returns></returns>
        public async Task Delay(int length)
        {
            await Task.Run(() =>
            {
                delay = true;
                Thread.Sleep(length);
                delay = false;
            });
        }

        /// <summary>
        /// Returns a byte array based on the bitmap given and pixel size.
        /// </summary>
        /// <param name="bitmap">Bitmap to convert to byte[]</param>
        /// <param name="pixelSize">the nth pixel to sample</param>
        /// <returns>Returns a byte[] to be written to LED Matrix</returns>
        public static byte[] BitmapToByteArray(Bitmap bitmap, int pixelSize)
        {
            BitArray bitArray = new BitArray(new byte[8]);
            byte[] result = new byte[8];

            int index = 0;
            for (int y = pixelSize - 1; y < bitmap.Height; y += pixelSize)
            {
                for (int x = pixelSize - 1; x < bitmap.Width; x += pixelSize)
                {
                    Console.WriteLine($"{x} {y}");
                    bitArray[index] = bitmap.GetPixel(x, y).R != 0;
                    index++;
                }
            }
            bitArray.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// Write an animation to the LED Matrix display
        /// </summary>
        /// <param name="filePath">Path to GIF</param>
        /// <param name="frameDelay">Delay between each frame</param>
        /// <param name="pixelSize">The nth pixel to sample</param>
        public void WriteAnimation(string filePath, int frameDelay, int pixelSize)
        {
            // Create a new thread, then animate.
            var task = Task.Run(() =>
            {
                List<byte[]> frames = new List<byte[]>();

                // Create a bitmap from the given file path,
                // get dimensions, and frame count.
                var bitmap = new Bitmap(filePath, true);
                var dimension = new FrameDimension(bitmap.FrameDimensionsList[0]);
                var frameCount = bitmap.GetFrameCount(dimension);

                // loop over bitmap frames, then add them to an array.
                for (int i = 0; i < frameCount; i++)
                {
                    bitmap.SelectActiveFrame(FrameDimension.Time, i);
                    frames.Add(BitmapToByteArray(bitmap, pixelSize));
                }

                // open serial port, then tell microcontroller
                // it will be receiving an animation.
                try
                {
                    // try to open the serial port, then
                    // try to tell the device to enter animation mode.
                    _serialPort.Open();
                    _serialPort.Write(new byte[] {0b00001000}, 0, 1);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to write to serial port: " + e);
                    throw;
                }

                // loop over each frame, then each row
                // in the frame.
                foreach (byte[] frame in frames)
                {
                    // Tell device it will receive a new frame.
                    _serialPort.Write(new byte[] {0b00001000}, 0, 1);
                    foreach (byte row in frame)
                    {
                        try
                        {
                            // try to write frame row to the device
                            _serialPort.Write(new byte[] {row}, 0, 1);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Failed to write frame to device: " + e);
                            throw;
                        }
                    }

                    // Add the delay between frames.
                    Thread.Sleep(frameDelay);
                }

                // finished writing animation, close serial port.
                _serialPort.Close();
            });
            task.Wait();
        }

        /// <summary>
        /// Clears the LED Matrix display.
        /// </summary>
        public void ClearScreen()
        {
            this._serialPort.Open();
            this._serialPort.Write(new byte[] {0b00000111}, 0, 1);
            this._serialPort.Close();
        }

        /// <summary>
        /// Close the serial port if needed.
        /// </summary>
        public void Dispose()
        {
            _serialPort.Close();
        }
    }
}
