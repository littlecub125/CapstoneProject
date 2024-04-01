using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

using static System.Math;
using static System.Convert;
using Capstone_v1.MVVM;
using System.Device.Gpio;

namespace Capstone_v1
{
        /// <summary>
    /// controls an SG90 Motor 
    ///
    /// When Creating this class always Create as a static object...
    /// There should only be one instance of this class for 
    /// each GPIO pin it represents
    ///    
    // </summary>
    public enum RotateServer
    {
        RotateToCW = 0,
        RotateToStop = 1,
        RotateToCCW = 2,
    }
    /// <summary>
    /// controls an SG90 Motor 
    ///
    /// When Creating this class always Create as a static object...
    /// There should only be one instance of this class for 
    /// each GPIO pin it represents
    ///    
    // </summary>
    public class SG90MotorController
    {
        private GpioController _gpioController = new GpioController();
        private GpioPin _motorPin = null;
        private ulong _ticksPerMilliSecond = (ulong)(Stopwatch.Frequency) / 1000; //Number of ticks per millisecond this is different for different processor

        
        public int RaspberryGPIOpin { get; }

        public bool GpioInitialized
        {
            get;
            private set;
        }

        #region Constructors
        /// <summary>
        /// Create a Motor contoller that is connected to 
        /// GPIO Pin 2
        /// </summary>
        public SG90MotorController()
        {
            RaspberryGPIOpin = RaspberryPiGPI0Pin.GPIO21;
            //GpioInit();
        }

        /// <summary>
        /// Create a Motor contoller that is connected to 
        /// a sepcified GPIO Pin
        /// </summary>
        /// <param name="gpioPin"></param>
        public SG90MotorController(RaspberryPiGPI0Pin gpioPin)
        {
            RaspberryGPIOpin = gpioPin;
            //GpioInit();
        }
        #endregion

        /// <summary>
        /// Initialize the GPIO pin
        /// </summary>
        public bool GpioInit()
        {
            bool bResult=false;
            try
            {
                if (!_gpioController.IsPinOpen(RaspberryGPIOpin))
                {
                    _gpioController.OpenPin
                }
                GpioInitialized = false;
                
                _motorPin = _gpioController.OpenPin(Convert.ToInt32(RaspberryGPIOpin));
                _motorPin.SetDriveMode(GpioPinDriveMode.Output);
                GpioInitialized = true;
                bResult = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: GpioInit failed - " + ex.ToString());
                bResult = false;
            }
            return bResult;
        }



        /// <summary>
        /// Sends a pulse to the server motor that will 
        /// turn it in one direction or another
        /// </summary>
        /// <param name="rotateServer">Enumeration for rotating the server</param>        
        public void PulseMotor(RotateServer rotateServer, double dmotorFactor)
        {
            double dTime;
            if (rotateServer == RotateServer.RotateToStop)
            {
                dTime = 1500;
                
            }
            else if (rotateServer == RotateServer.RotateToCW)
            {
                dTime = 700 * dmotorFactor;
                if (dTime >= 1500)
                {
                    dTime = 1450;
                }
            }
            else if (rotateServer == RotateServer.RotateToCCW)
            {
                dTime = 1500 * dmotorFactor;
                if (dTime > 2300)
                {
                    dTime = 2300;
                }
            }
            else
            {
                dTime = 0;
            }
            
            PulseMotor(dTime);
        }

        /// <summary>
        //Function to wait so many milliseconds, this is required because a task.delay
        // time to execute is too long. This is a blocking thread but since the time
        // to wait are so small for the SG90 it may not matter
        /// </summary>
        /// <param name="millisecondsToWait">Number of milliseconds before the function returns</param>
        private void MillisecondToWait(double millisecondsToWait)
        {
            var sw = new Stopwatch();
            double durationTicks = _ticksPerMilliSecond * millisecondsToWait;
            sw.Start();
            while (sw.ElapsedTicks < durationTicks)
            {
                int x = 3;
                x = x + x;
            }
        }

        /// <summary>
        /// Sends enough pulses to the server motor that will 
        /// turn it all the way in one direction or another or towards the center.
        /// </summary>
        /// <param name="motorPulse">number of milliseconds to wait to pulse the servo</param>
        public void PulseMotor(double motorPulse)
        {

            //Total amount of time for a pulse
            double TotalPulseTime;
            double timeToWait;

            TotalPulseTime = 2300;
            timeToWait = TotalPulseTime - motorPulse;

            //Send the pulse to move the servo over a given time span
            _motorPin.Write(GpioPinValue.High);
            MillisecondToWait(motorPulse);
            _motorPin.Write(GpioPinValue.Low);
            MillisecondToWait(timeToWait);
            _motorPin.Write(GpioPinValue.Low);
        }

        /// <summary>
        /// 
        /// Retrieves the number of milliconds to send as a pulse to turn the motor
        /// to the left right or middle
        /// 
        /// Values from Specification 
        ///     Position "0"   (1.5 ms pulse) is middle,
        ///     Position "90"  (~2 ms pulse) is all the way to the right
        ///     Position"-90" (~1 ms pulse) is all the way to the left
        ///     
        /// Values that were found to actually work
        ///     Position "0"   (1.2 ms pulse) is middle,
        ///     Position "90"  (~2 ms pulse) is all the way to the right
        ///     Position"-90" (~.4 ms pulse) is all the way to the left

 
    }
}
