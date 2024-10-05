using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Drivers.Sitronix.ST7735;
using GHIElectronics.TinyCLR.Pins;
using System.Drawing;
using System.Threading;

namespace FEZMan
{
    class Program
    {
        private static PacmanGame _pacmanGame = null;
        private static ST7735Controller st7735;
        private const int SCREEN_WIDTH = 160;
        private const int SCREEN_HEIGHT = 128;

        static void Main()
        {
            var spi = SpiController.FromName(SC20100.SpiBus.Spi4);
            var gpio = GpioController.GetDefault();

            st7735 = new ST7735Controller(
                spi.GetDevice(ST7735Controller.GetConnectionSettings
                (SpiChipSelectType.Gpio, gpio.OpenPin(SC20100.GpioPin.PD10))), //CS pin.
                gpio.OpenPin(SC20100.GpioPin.PC4), //RS pin.
                gpio.OpenPin(SC20100.GpioPin.PE15) //RESET pin.
            );

            var backlight = gpio.OpenPin(SC20100.GpioPin.PA15);
            backlight.SetDriveMode(GpioPinDriveMode.Output);
            backlight.Write(GpioPinValue.High);

            st7735.SetDataAccessControl(true, true, false, false); //Rotate the screen.
            st7735.SetDrawWindow(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
            st7735.Enable();
            Graphics.OnFlushEvent += Graphics_OnFlushEvent;

            var surface = Graphics.FromImage(new Bitmap(160, 128));
            
            _pacmanGame = new PacmanGame(surface);
            _pacmanGame.InputManager.AddInputProvider(new FEZBitInputProvider());
            _pacmanGame.Initialize();
            Thread.Sleep(Timeout.Infinite);
        }

        private static void Graphics_OnFlushEvent(Graphics sender, byte[] data, int x, int y, int width, int height, int originalWidth)
        {
            st7735.DrawBuffer(data);
        }
    }
}
