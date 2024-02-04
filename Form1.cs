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


    public partial class Form1 : Form , ISender, ILoggerView
    {
        //Asdome_protocol asdptl;
        Controler ctr;
        public Form1()
        {
            InitializeComponent();
            //asdptl = new Asdome_protocol(this);
            Logger.view = this;
            ctr = new Controler(this);
        }

        public void send(byte[] buff)
        {
            serialPort1.Write(buff, 0, buff.Length);     
        }
        public void logShow(string msg)
        {
            listBox3.Items.Add(msg);
        }

        private void initSerialPort(string portName, int BaudRate)
        {
            serialPort1.PortName = portName;    
            serialPort1.BaudRate = BaudRate;    
            string[] defserialPortSettings = new  string [6];
            defserialPortSettings[0] = portName;
            defserialPortSettings[1] = BaudRate.ToString();
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
            //asdptl.status();
            ctr.status();

           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            label1.Text = serialPort1.IsOpen ? "Открыт" : "Закрыт";
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox2.Items.Add(s);
            }
            comboBox2.SelectedIndex = 0;

            comboBox1.Items.AddRange(new string[] {"7200","9600", "115200"});
            comboBox1.SelectedText = "9600";

            foreach (string s in Asdome_protocol.getCmdList().ToArray())
            {
                listBox2.Items.Add(s);
            }
            listBox2.SetSelected(0, true);
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
            
            initSerialPort(comboBox2.Text, int.Parse(comboBox1.Text));
            label1.Text = serialPort1.IsOpen ? "Открыт" : "Закрыт";
            comboBox2.Enabled = false;
            comboBox1.Enabled = false;  

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            label1.Text = serialPort1.IsOpen ? "Открыт" : "Закрыт";
            comboBox2.Enabled = false;  
            comboBox1.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                  
                //asdptl.send(pbuff);
                //asdptl.send(listBox2.SelectedIndex, textBox2.Text);
                ctr.send(listBox2.SelectedIndex, textBox2.Text);
                //Logger.log(listBox2.SelectedItem.ToString() + textBox2.Text);
            }
        }

        private void listBox3_DoubleClick(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
        }
    }
}
