using Capstone_v1.MVVM;
using Capstone_v1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App
{
    public partial class Form1 : Form
    {
        OperateClass controller_x1;
        OperateClass controller_x2;
        OperateClass controller_y;
        public Form1()
        {
            InitializeComponent();
        }
        private void AddLogToListBox(string sLog)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    lbLogBox.Items.Add(sLog);
                }));
            }
            else
            {
                lbLogBox.Items.Add(sLog);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            controller_x1 = new OperateClass(RaspberryPiGPI0Pin.GPIO21);
            controller_x2 = new OperateClass(RaspberryPiGPI0Pin.GPIO20);
            controller_y = new OperateClass(RaspberryPiGPI0Pin.GPIO16);


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

            switch (btn.Name)
            {
                case "btnLeft":
                    controller_x1.MoveRelative("CW", dFactor);
                    controller_x2.MoveRelative("CW", dFactor);
                    break;

                case "btnRight":
                    controller_x1.MoveRelative("CCW", dFactor);
                    controller_x2.MoveRelative("CCW", dFactor);
                    break;

                case "btnUp":
                    controller_y.MoveRelative("CCW", dFactor);
                    break;
                case "btnDown":
                    controller_y.MoveRelative("CW", dFactor);
                    break;
            }

        }

        private void btnSetPulseFactor_Click(object sender, EventArgs e)
        {
            dFactor = double.Parse(tbPulseFactor.Text);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            controller_x1.MoveRelative("Stop", 0);
            controller_x2.MoveRelative("Stop", 0);
            controller_y.MoveRelative("Stop", 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool x1 = controller_x1.Init();
            bool x2 = controller_x2.Init();
            bool y = controller_y.Init();

            if (x1 == true)
            {
                lbLogBox.Items.Add("Connected X 1 Motor");
            }
            else
            {
                lbLogBox.Items.Add("Failed Connecting X 1 Motor");
            }

            if (x2 == true)
            {
                lbLogBox.Items.Add("Connected X 2 Motor");
            }
            else
            {
                lbLogBox.Items.Add("Failed Connecting X 2 Motor");
            }
            if (y == true)
            {
                lbLogBox.Items.Add("Connected Y Motor");
            }
            else
            {
                lbLogBox.Items.Add("Failed Connecting Y Motor");
            }
        }
    }
}
