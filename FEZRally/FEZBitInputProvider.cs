// Copyright (c) 2012 Chris Taylor

using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using TinyCLR.Game.Input;

namespace FEZRally
{
    class FEZBitInputProvider : IInputProvider
    {
        private readonly GpioPin _btnLeft;
        private readonly GpioPin _btnRight;
        private readonly GpioPin _btnUp;
        private readonly GpioPin _btnDown;
        private readonly GpioPin _btnA;
        private readonly GpioPin _btnB;
        public FEZBitInputProvider()
        {
            var gpio = GpioController.GetDefault();
            _btnLeft = gpio.OpenPin(SC20100.GpioPin.PE3);
            _btnRight = gpio.OpenPin(SC20100.GpioPin.PB7);
            _btnUp = gpio.OpenPin(SC20100.GpioPin.PE4);
            _btnDown = gpio.OpenPin(SC20100.GpioPin.PA1);
            _btnA = gpio.OpenPin(SC20100.GpioPin.PE5);
            _btnB = gpio.OpenPin(SC20100.GpioPin.PE6);

            _btnLeft.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _btnRight.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _btnUp.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _btnDown.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _btnA.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _btnB.SetDriveMode(GpioPinDriveMode.InputPullUp);
        }

        public double X
        {
            get
            {
                var l = _btnLeft.Read() == GpioPinValue.Low ? -1 : 0;
                var r = _btnRight.Read() == GpioPinValue.Low ? 1 : 0;
                return l + r;
            }
        }

        public double Y
        {
            get
            {
                var u = _btnUp.Read() == GpioPinValue.Low ? -1 : 0;
                var d = _btnDown.Read() == GpioPinValue.Low ? 1 : 0;
                return u + d;
            }
        }

        public bool Button1
        {
            get { return _btnA.Read() == GpioPinValue.Low; }
        }

        public bool Button2
        {
            get { return _btnB.Read() == GpioPinValue.Low; }
        }

        public bool Button3
        {
            get { return false; }
        }

        public bool Button4
        {
            get { return false; }
        }

        public void Update(float elapsedTime)
        {
           
        }
    }
}
