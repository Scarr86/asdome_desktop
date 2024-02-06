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


    public partial class Form1 : Form, IAnswerVisitor,  ILoggerView, ISender
    {
        IASDProtocol asdptl;
        List<byte> buffer = new List<byte>();
        public Form1()
        {
            InitializeComponent();
            asdptl = new Asdome_protocol(this, this);
            Logger.view = this;
        }
        public void send(byte[] buffer, int len) 
        {
            serialPort1.Write(buffer, 0, len);
        }

        public void visit(StatusAnswer answer)
        {
            //TODO

            //DELETE
            visitLog(answer);
        }
        public void visit(StopdomeAnswer answer)
        {
            //TODO

            //DELETE
            visitLog(answer);
        }

        public void visitLog(Answer answer)
        {
            Console.WriteLine(string.Format("handler: {0} | {1}", answer.cmd, BitConverter.ToString(answer.param)));
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
            //ASDRequest req = 
            asdptl.status();
            //serialPort1.Write(req.buff, 0, req.len);           
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
                switch(listBox2.SelectedIndex)
                {
                    case 0: asdptl.status(); break;
                    default: asdptl.send(listBox2.SelectedIndex, textBox2.Text); break;
                }

            }
        }

        private void listBox3_DoubleClick(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (serialPort1.BytesToRead != 0)
            {
                byte rx = Convert.ToByte(serialPort1.ReadByte());
                if (rx == 0x0d)
                {
                    Console.WriteLine(string.Format("recv: {0:HH:mm:ss} | {1}", DateTime.Now, BitConverter.ToString(buffer.ToArray()) + "-0D"));
                    listBox1.Invoke(new EventHandler(delegate
                    {
                        listBox1.Items.Add(string.Format("{0:HH:mm:ss} | {1}", DateTime.Now, BitConverter.ToString(buffer.ToArray()) + "-0D"));

                    }));
                    asdptl.answer(buffer.ToArray());
                    buffer.Clear();
                }
                else
                {
                    buffer.Add(rx);
                }
            }

        }
    }
}
