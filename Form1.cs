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
using System.Runtime.InteropServices;

namespace asdome_desktop
{


    public partial class Form1 : Form, IAnswerVisitor, ILoggerView, ISender
    {
        IASDProtocol asdptl = null;
        Simulator simulator;
        Asdome_protocol dome_ptl;
        DD_protocol dd_ptl;
        List<byte> buffer = new List<byte>();
        public Form1()
        {
            InitializeComponent();
            simulator = new Simulator(this, this);
            dome_ptl = new Asdome_protocol(this, this);
            dd_ptl = new DD_protocol(this);
            Logger.view = this;
        }
        public void send(byte[] buffer)
        {
            if (serialPort1.IsOpen)
                serialPort1.Write(buffer, 0, buffer.Length);
        }

        public void visit(StatusAnswer answer)
        {
            //TODO

            //DELETE
            visitLog(answer);
        }
        public void visit(MoveParamsAnswer answer)
        {
            //TODO

            //DELETE
            visitLog(answer);

        }
        public void visit(TLMAnswer answer)
        {
            //TODO

            //DELETE
            visitLog(answer);
        }

        public void visitLog(Answer answer)
        {
            Console.WriteLine(string.Format("handler: {0} | {1}", answer.cmd, BitConverter.ToString(answer.param)));
            listBox1.Invoke(new EventHandler(delegate
            {
                listBox1.Items.Add(string.Format("{0:HH:mm:ss} | {1}", DateTime.Now,  answer.req + string.Join(",",answer.param)));

            }));
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
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = serialPort1.IsOpen ? "Открыт" : "Закрыт";
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox2.Items.Add(s);
            }
            comboBox2.Items.Add("Simulator");

            comboBox2.SelectedIndex = 0;

            comboBox1.Items.AddRange(new string[] {"7200","9600", "115200"});
            comboBox1.SelectedText = "9600";

            foreach (Cmd s in dome_ptl.getCmdList().ToArray())
            {
                listBox2.Items.Add(s.cmd);
            }
            listBox2.Items.Add("state protocol watchdog".ToUpper());
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

            if(comboBox2.SelectedIndex == comboBox2.Items.Count - 1)
            {
                asdptl = simulator;
                simulator.Start();
            }
            else
            {
                asdptl = dome_ptl;
                initSerialPort(comboBox2.Text, int.Parse(comboBox1.Text));
                label1.Text = serialPort1.IsOpen ? "Открыт" : "Закрыт";
            }      
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
            comboBox2.Enabled = true;  
            comboBox1.Enabled = true;
            asdptl = null;
            simulator.Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] prm = textBox2.Text.Trim().Length == 0 ? new string[0] : textBox2.Text.Split(',');
            List<byte> dparam = new List<byte>();
            foreach (string s in prm)
            {
                dparam.Add(byte.Parse(s));
            }

            //TODO SEND COMMAND
            if (asdptl != null)
            {
                switch(listBox2.SelectedIndex)
                {
                    case 0: asdptl.status().send(); break;
                    case 1: asdptl.StopDome().send(); break;
                    case 2: asdptl.OpenDome().send(); break;
                    case 3: asdptl.CloseDome().send();break;
                    case 4:
                        {
                            ShutterMoveDeg cmd = asdptl.ShutterMoveDeg();
                            if (dparam.Count > 1)
                            {
                                cmd.numShutter = dparam[0];
                                cmd.angle = dparam[1];
                            }
                            cmd.send();
                            break;
                        }
                    case 5:
                        {
                            SwitchToggle cmd = asdptl.SwitchToggle();
                            if (dparam.Count > 1)
                            {
                                cmd.number = dparam[0];
                                cmd.state = dparam[1];
                            }
                            cmd.send();
                            break;
                        }
                    case 6:
                        {
                            Armrain cmd = asdptl.Armrain();
                            if (dparam.Count > 0)
                            {
                                cmd.on = BitConverter.ToBoolean(dparam.ToArray(), 0);
                            }
                            cmd.send();
                            break;
                        }
                    case 7:
                        {
                            asdptl.GetMoveParams().send();
                            break;
                        }
                    case 8:
                        {
                            SetMoveParams cmd = asdptl.SetMoveParams();
                            if (dparam.Count > 3)
                            {
                                cmd.pwm_break = dparam[0];
                                cmd.pwm_full = dparam[1];
                                cmd.pwm_accel = dparam[2];
                                cmd.angle_break = dparam[3];
                                cmd.RainInf = dparam[4];
                            }
                            cmd.send();
                            break;
                        }
                    case 9:
                        {
                            asdptl.GetTLM().send();
                            break;
                        }
                    case 10:
                        {
                            int i = 0;
                            dd_ptl.send("/? p\r", "state", "timeout").on((int[] param) =>
                            {
                                for (; i < param.Length; i++)
                                {
                                    Console.WriteLine(string.Format("param({0}): {1}",i, param[i]));
                                }
                            });
                            break;
                        }

                    default: asdptl.send(listBox2.SelectedIndex, textBox2.Text); break;
                }

            }
        }
        //cmdList.Add(new Cmd(this, "STATUS"));
        //    cmdList.Add(new Cmd(this, "STOPDOME"));
        //    cmdList.Add(new Cmd(this, "OPENDOME"));
        //    cmdList.Add(new Cmd(this, "CLOSEDOME"));
        //    cmdList.Add(new ShutterMoveDeg(this));
        //    cmdList.Add(new SwitchToggle(this));
        //    cmdList.Add(new Armrain(this));
        //    cmdList.Add(new Cmd(this, "GETMOVEPARAMS"));
        //    cmdList.Add(new SetMoveParams(this));
        //    cmdList.Add(new Cmd(this, "GETTLM"));

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
                    Console.WriteLine(string.Format("recv: {0:HH:mm:ss}  |  {1}", DateTime.Now, BitConverter.ToString(buffer.ToArray()) + "-0D"));

                    asdptl.answer(buffer.ToArray());
                    dd_ptl.answer(Encoding.ASCII.GetBytes("\r\nstate: 50\r\ntimeout: 100\r\n"));
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
