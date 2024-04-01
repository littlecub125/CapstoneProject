using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Device.Gpio;
using System.Device.Pwm;

namespace CapstoneV2.Model
{
    public class SG90MotorController
    {
        public static GpioController controller = new GpioController();
        private PwmChannel pwmChannel;

        public enum RotateServer
        {
            RotateToCW = 0,
            RotateToStop = 1,
            RotateToCCW = 2,
        }



        private ulong _ticksPerMilliSecond = (ulong)(Stopwatch.Frequency) / 1000; //Number of ticks per millisecond this is different for different processor


        public int RaspberryGPIOpin { get; set; }

        public bool GpioInitialized
        {
            get;
            private set;
        }

        #region Constructors


        /// <summary>
        /// Create a Motor contoller that is connected to 
        /// a sepcified GPIO Pin
        /// </summary>
        /// <param name="gpioPin"></param>
        public SG90MotorController(int gpioPin)
        {
            RaspberryGPIOpin = gpioPin;
        }

        public bool InitMotor()
        {
            bool bResult = GpioInit();
            return bResult;
        }
        #endregion
   
        /// <summary>
        /// Initialize the GPIO pin
        /// </summary>
        public bool GpioInit()
        {
            bool bResult = false;
            try
            {
                //WiringPi.Core.PinMode(RaspberryGPIOpin, PinMode.Output);
                if (!controller.IsPinOpen(RaspberryGPIOpin))
                {
                    controller.OpenPin(RaspberryGPIOpin, PinMode.Output);
                }
                

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
            pwmChannel.DutyCycle = 50;

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

            TotalPulseTime = 20000;
            timeToWait = TotalPulseTime - motorPulse;

           
            controller.Write(RaspberryGPIOpin, PinValue.High);
            MillisecondToWait(motorPulse);

            controller.Write(RaspberryGPIOpin, PinValue.Low);
            MillisecondToWait(timeToWait);
            controller.Write(RaspberryGPIOpin, PinValue.Low);

          
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
        public void MoveRelative(string sCmd, double dFactor)
        {
            RotateServer Direction;

            switch (sCmd)
            {
                case "CW":
                    Direction = RotateServer.RotateToCW;
                    break;

                case "CCW":
                    Direction = RotateServer.RotateToCCW;
                    break;
                case "Stop":
                    Direction = RotateServer.RotateToStop;
                    break;
                default:
                    Direction = RotateServer.RotateToStop;
                    break;
            }

            PulseMotor(Direction, dFactor);

            return;
        }
    }
}
