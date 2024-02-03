using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asdome_desktop
{


    public partial class Form1 : Form , ISender
    {
        Asdome_protocol asdptl;
        public Form1()
        {
            InitializeComponent();
            asdptl = new Asdome_protocol(this);

        }

        public void send(byte[] buff)
        {
            serialPort1.Write(buff, 0, buff.Length);     
        }
        private void initSerialPort(string portName)
        {
            string[] defserialPortSettings = new  string [6];
            defserialPortSettings[0] = portName;
            defserialPortSettings[1] = serialPort1.BaudRate.ToString();
            defserialPortSettings[2] = serialPort1.Parity.ToString();
            defserialPortSettings[3] = serialPort1.DataBits.ToString();
            defserialPortSettings[4] = serialPort1.StopBits.ToString();
            defserialPortSettings[5] = serialPort1.Handshake.ToString();
            
            Console.WriteLine(string.Join(Environment.NewLine, defserialPortSettings));
            serialPort1.Open();
            Console.WriteLine("isOpen:" + serialPort1.IsOpen.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //serialPort1.WriteLine(textBox1.Text);
            asdptl.status();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            label1.Text = serialPort1.IsOpen ? "Открыт" : "Закрыт";
            foreach (string s in SerialPort.GetPortNames())
            {
                listBox1.Items.Add(s);
            }
            listBox1.Items.Add("COM10");
            listBox1.Items.Add("COM15");
            listBox1.SetSelected(0, true);
            foreach (string s in asdptl.getCmdList().ToArray())
            {
                listBox2.Items.Add(s);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            initSerialPort(listBox1.SelectedItem.ToString());
            label1.Text = serialPort1.IsOpen ? "Открыт" : "Закрыт";
            listBox1.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            label1.Text = serialPort1.IsOpen ? "Открыт" : "Закрыт";
            listBox1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                string[] param = textBox2.Text.Split(',');
                List<byte> dparam = new List<byte>();
                foreach(string s in param) {
                    dparam.Add(byte.Parse(s));
                    dparam.Add(0x2c);
                }
                dparam.RemoveAt(dparam.Count - 1);
                List<byte> buff = Encoding.ASCII.GetBytes(listBox2.SelectedItem.ToString()).ToList();
                byte[] pbuff = buff.Concat(dparam).ToArray();   
                asdptl.send(pbuff) ;
            }
        }
    }
}
