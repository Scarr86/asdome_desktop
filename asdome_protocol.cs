using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asdome_desktop
{

    //public struct ASDRequest
    //{
    //    public byte[] buff;
    //    public int len;
    //}
    public interface IAnswerVisitor
    {
        void visit(StatusAnswer answer);
        void visit(StopdomeAnswer answer);
    }

    public abstract class Answer
    {
        public string cmd;
        public byte[] param = new byte[0];
        public Answer(string name)
        {
            cmd = name;
        }
        public abstract void Accept(IAnswerVisitor visitor);
        public abstract Answer Clone(byte[] param);
    }
    public class StopdomeAnswer : Answer
    {
        public StopdomeAnswer(string name) : base(name)
        {

        }
        public override void Accept(IAnswerVisitor visitor)
        {
            visitor.visit(this);
        }
        public override Answer Clone(byte[] param)
        {
            return new StopdomeAnswer(cmd);
        }

    }
    public class StatusAnswer: Answer
    {
        
        public bool stateSh1 { get { return BitConverter.ToBoolean(param, 0); } }
        public bool stateSh2 { get { return BitConverter.ToBoolean(param, 1); } }
        public byte encoder1;
        public byte encoder2;
        public bool switch1;
        public bool switch2;
        public bool switch3;
        public byte ArmRainSensor;
        public bool isRain;

        //byte[] param = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0};

        public StatusAnswer(string name) : this(name, new byte[0]) { }
        public StatusAnswer(string name, byte[] param) : base(name)
        {
            if (param.Length == 9)
                this.param = param;
            else
                this.param = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }
        public override void Accept(IAnswerVisitor visitor)
        {
            visitor.visit(this);
        }
        public override Answer Clone(byte[] param)
        {
            return new StatusAnswer(this.cmd, param);
        }

    }

    internal abstract class IASDProtocol
    {
        public IAnswerVisitor visitor;

        public IASDProtocol(IAnswerVisitor visitor)
        {
            this.visitor = visitor;
        }
        abstract public void answer(byte[] b);
        // abstract public ASDRequest send(int idCmd, string param);
        abstract public void send(int idCmd, string param);
        //abstract public ASDRequest status();
        abstract public void status();
    }

    public interface ISender {

        void send(byte[] buffer, int len);
    }


    internal class Asdome_protocol: IASDProtocol
    {
        ISender sender;
        static  List<string> cmdList = new List<string> {
            "STATUS",
            "STOPDOME",
            "OPENDOME",
            "CLOSEDOME",
            "SHUTTERMOVEDEG",
            "SWITCHTOGGILE",
            "ARMRAIN",
            "GETMOVEPARAMS",
            "SETMOVEPARAMS",
            "GETTLM"
            };

        static List<Answer> answerList = new List<Answer> {
                new StatusAnswer("STATUS"),
                new StopdomeAnswer("STOPDOME")
            };
        SortedDictionary<string, string> logStr = new SortedDictionary<string, string>();

        public Asdome_protocol(ISender sender, IAnswerVisitor visitor):base(visitor)
        {
            this.sender = sender;
            logStr.Add(cmdList[0], "STATUS");
            logStr.Add(cmdList[1], "STOPDOME");
            logStr.Add(cmdList[2], "OPENDOME");
            logStr.Add(cmdList[3], "CLOSEDOME");
            logStr.Add(cmdList[4], "SHUTTERMOVEDEG");
            logStr.Add(cmdList[5], "SWITCHTOGGILE");
            logStr.Add(cmdList[6], "ARMRAIN");
            logStr.Add(cmdList[7], "GETMOVEPARAMS");
            logStr.Add(cmdList[8], "SETMOVEPARAMS");
            logStr.Add(cmdList[9], "GETTLM");

        }

        public override void answer(byte[] b)
        {
            string cmd = Encoding.ASCII.GetString(b);

            int id = answerList.FindIndex(0, s => cmd.Contains(s.cmd));
            if (id != -1)
            {
                cmd = cmd.Remove(cmd.IndexOf(cmdList.ElementAt(id)), cmdList.ElementAt(id).Length);
                string[] p = cmd.Split(new char[] { ',' });
                answerList.ElementAt(id).Clone(Encoding.Default.GetBytes(string.Join("", p))).Accept(visitor);
                //Console.WriteLine(id + "|" + BitConverter.ToString(Encoding.Default.GetBytes(string.Join("", p))));
            }
        }
        public override void status()
        {
           send(cmdList.FindIndex(0, x => x == "STATUS"), "");
        }


        public override void send(int idCmd, string param)
        {
            if(idCmd < 0 || idCmd > cmdList.Count - 1) {
                return;
                //return new ASDRequest()
                //{
                //    buff = new byte[0],
                //    len = 0
                //};
            }

            string[] prm = param.Trim().Length == 0 ? new string[0] : param.Split(',');
            List<byte> dparam = new List<byte>();
            foreach (string s in prm)
            {
                dparam.Add(byte.Parse(s));
                dparam.Add(0x2c);
            }
            if (dparam.Count > 0)
                dparam.RemoveAt(dparam.Count -1);
            dparam.Add(0x0d);

            List<byte> buff = Encoding.ASCII.GetBytes(cmdList[idCmd]).ToList();

            byte[] pbuff = buff.Concat(dparam).ToArray();

            Logger.log(logStr[cmdList[idCmd]] + param);
            Console.WriteLine(string.Format("send: {0:HH:mm:ss} | {1}", DateTime.Now, BitConverter.ToString(pbuff)));

            sender.send(pbuff, pbuff.Length);
            //return new ASDRequest() { 
            //    buff = pbuff,
            //    len = pbuff.Length
            //};
        }
        static public List<string> getCmdList()
        {
            return cmdList;
        }
    }
}
