using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using CapstoneV2.Model;


namespace CapstoneV2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SG90MotorController motion_X1;
        SG90MotorController motion_X2;
        SG90MotorController motion_Y;

        private void Form1_Load(object sender, EventArgs e)
        {
            OperatingSystem os = System.Environment.OSVersion;
            
            if (os.Platform !=PlatformID.Win32NT)
            {
                int result = 0;
                //int result = WiringPi.Core.Setup();
                if (result == -1)
                {
                    lbLogBox.Items.Add("Please Open in Raspberry Pi");
                }
                else
                {
                    motion_X1 = new SG90MotorController(16);
                    motion_X2 = new SG90MotorController(20);
                    motion_Y = new SG90MotorController(21);
                   

                    if (motion_X1.InitMotor() == true)
                    {
                        lbLogBox.Items.Add("Init motor X 1");
                    }
                    else
                    {
                        lbLogBox.Items.Add("Failed Init motor X 1");
                    }
                    if (motion_X2.InitMotor() == true)
                    {
                        lbLogBox.Items.Add("Init motor X 2");
                    }
                    else
                    {
                        lbLogBox.Items.Add("Failed Init motor X 2");
                    }
                    if (motion_Y.InitMotor() == true)
                    {
                        lbLogBox.Items.Add("Init motor Y");
                    }
                    else
                    {
                        lbLogBox.Items.Add("Failed Init motor Y");
                    }
                }
            }
            else
            {
                lbLogBox.Items.Add("You are not using Raspberry Pi");
            }
        }
        double _motionFactor;
        public double dFactor
        {
            get { return _motionFactor; }
            set { _motionFactor = value; }
        }


        private void btnLeft_Click(object sender, EventArgs e)
        {
            PictureBox btn = (PictureBox)sender;
            Thread moveThread1;
            Thread moveThread2;
            Thread moveThread3;
            switch (btn.Name)
            {
                case "btnLeft":
                    moveThread1 = new Thread(() => motion_X1.MoveRelative("CW", dFactor));
                    moveThread2 = new Thread(() => motion_X2.MoveRelative("CW", dFactor));

                    break;

                case "btnRight":
                    moveThread1 = new Thread(() => motion_X1.MoveRelative("CCW", dFactor));
                    moveThread2 = new Thread(() => motion_X2.MoveRelative("CCW", dFactor));

                    break;

                case "btnUp":
                    moveThread3 = new Thread(() => motion_Y.MoveRelative("CCW", dFactor));
         
                    break;
                case "btnDown":
                    moveThread3 = new Thread(() => motion_Y.MoveRelative("CW", dFactor));
  
                    break;
            }
            lbLogBox.Items.Add("Moved" + btn.Name);
            return;
        }

        private void btnSetPulseFactor_Click(object sender, EventArgs e)
        {
            dFactor = double.Parse(tbPulseFactor.Text);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

            motion_X1.MoveRelative("Stop", 0);
            motion_X2.MoveRelative("Stop", 0);
            motion_Y.MoveRelative("Stop", 0);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            lbLogBox.Items.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            motion_X1 = new SG90MotorController(16);
            motion_X2 = new SG90MotorController(20);
            motion_Y = new SG90MotorController(21);
       
            if (motion_X1.InitMotor() == true)
            {
                lbLogBox.Items.Add("Init motor X 1");
            }
            else
            {
                lbLogBox.Items.Add("Failed Init motor X 1");
            }
            if (motion_X2.InitMotor() == true)
            {
                lbLogBox.Items.Add("Init motor X 2");
            }
            else
            {
                lbLogBox.Items.Add("Failed Init motor X 2");
            }
            if (motion_Y.InitMotor() == true)
            {
                lbLogBox.Items.Add("Init motor Y");
            }
            else
            {
                lbLogBox.Items.Add("Failed Init motor Y");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
            for (int i = 0; i<26; i++)
            {
                if (SG90MotorController.controller.IsPinOpen(i))
                {
                    string str = i.ToString(); 
                    lbLogBox.Items.Add("Closed " + i);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SG90MotorController motion_test = new SG90MotorController(18);
            motion_test.InitMotor();
            motion_test.PulseMotor(1000);
        }
    }
}
