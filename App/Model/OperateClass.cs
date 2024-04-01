using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace Capstone_v1.MVVM
{
    public class OperateClass
    {
        SG90MotorController motion;

        public OperateClass(RaspberryPiGPI0Pin gpioPin)
        {
            motion = new SG90MotorController(gpioPin);

        }

        public bool Init()
        {
            bool bResult = motion.GpioInit();
            return bResult;
        }
        public void MoveRelative(string sCmd, double dMotorFactor)
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

            motion.PulseMotor(Direction, dMotorFactor);



        }

    }

}
