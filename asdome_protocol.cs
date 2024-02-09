
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace asdome_desktop
{

    static public class ASDConstant
    {
        public const int STATUS_ANSWER_LEN = 9;
        
    }



    //public struct ASDRequest
    //{
    //    public byte[] buff;
    //    public int len;
    //}
    public interface IAnswerVisitor
    {
        void visit(StatusAnswer answer);
        void visit(MoveParamsAnswer answer);
        void visit(TLMAnswer answer);

    }

    public interface ISender
    {
        void send(byte[] buffer);
    }

    public class Cmd
    {
        public string cmd;
        public byte[] param;
        IASDProtocol protocol;
        public Cmd(IASDProtocol protocol, string cmd, byte[] param)
        {
            this.cmd = cmd;
            this.param = param;
            this.protocol = protocol;
        }
        public Cmd(IASDProtocol protocol, string cmd):this(protocol, cmd, new byte[0])
        {
            
        }
        public byte [] Request()
        {
            List<byte> sparam = new List<byte>();
            foreach (var item in param.ToList())
            {
                sparam.Add(item);
                sparam.Add(0x2c);
            }
            if (sparam.Count > 0)
                sparam.RemoveAt(sparam.Count - 1);
            sparam.Add(0x0d);

            byte[] rqt = Encoding.ASCII.GetBytes(cmd).ToList().Concat(sparam).ToArray();
            return rqt;
        } 

        public void send()
        {
            protocol.send(this);
        }
    }

    public class ShutterMoveDeg: Cmd
    {
        public byte numShutter
        {
            get { return param[0]; }
            set { param[0] = value; }
        }
        public byte angle
        {
            get { return param[1]; }
            set { param[1] = value; }
        }
        public ShutterMoveDeg (IASDProtocol protocol, string cmd = "SHUTTERMOVEDEG") : base(protocol, cmd, new byte[2])
        {

        }
    }

    public class SwitchToggle : Cmd
    {
        public byte number
        {
            get { return param[0]; }
            set { param[0] = value; }
        }
        public byte state
        {
            get { return param[1]; }
            set { param[1] = value; }
        }
        public SwitchToggle(IASDProtocol protocol, string cmd = "SWITCHTOGGILE") : base(protocol, cmd, new byte[2])
        {

        }
    }

    public class Armrain : Cmd
    {
        public bool on
        {
            get { return BitConverter.ToBoolean(param, 0); }
            set { param[0] = Convert.ToByte(value); }
        }
   
        public Armrain(IASDProtocol protocol, string cmd = "ARMRAIN") : base(protocol, cmd, new byte[1])
        {

        }
    }

    public class SetMoveParams : Cmd
    {
        public byte pwm_break
        {
            get { return param[0]; }
            set { param[0] = value; }
        }
        public byte pwm_full
        {
            get { return param[1]; }
            set { param[1] = value; }
        }
        public byte pwm_accel
        {
            get { return param[2]; }
            set { param[2] = value; }
        }
        
        public byte angle_break
        {
            get { return param[3]; }
            set { param[3] = value; }
        }

        public byte RainInf
        {
            get { return param[6]; }
            set { param[6] = value; }
        }
        public SetMoveParams(IASDProtocol protocol, string cmd = "SETMOVEPARAMS") : base(protocol, cmd, new byte[7])
        {

        }
    }


    public abstract class Answer
    {
        public string cmd;
        public string req;
        public byte[] param;
        public Answer(string name, byte[] param, string req ="")
        {
            this.param = param ?? new byte[0];
            cmd = name;
            this.req = req;
        }
        public abstract void Accept(IAnswerVisitor visitor);
        public abstract Answer Clone(byte[] param);
        public byte[] GetBuffer()
        {
            List<byte> sparam = new List<byte>();
            foreach (var item in param.ToList())
            {
                sparam.Add(item);
                sparam.Add(0x2c);
            }
            if (sparam.Count > 0)
                sparam.RemoveAt(sparam.Count - 1);
            sparam.Add(0x0d);

            byte[] buff = Encoding.ASCII.GetBytes(req).ToList().Concat(param).ToArray();
            return buff;
        }

    }
    
    public class TLMAnswer : Answer
    {

        public bool c1
        {
            get { return checkBit(param[0], 7); }
            set { param[0] = setBit(param[0], 7, value); }
        }
        public bool c2
        {
            get { return checkBit(param[0], 6); }
            set { param[0] = setBit(param[0], 6, value); }
        }
        public bool c3
        {
            get { return checkBit(param[0], 5); }
            set { param[0] = setBit(param[0], 5, value); }
        }
        public bool c4
        {
            get { return checkBit(param[0], 4); }
            set { param[0] = setBit(param[0], 4, value); }
        }
        public bool o1
        {
            get { return checkBit(param[0], 3); }
            set { param[0] = setBit(param[0], 3, value); }
        }
        public bool o2
        {
            get { return checkBit(param[0], 2); }
            set { param[0] = setBit(param[0], 2, value); }
        }
        public bool o3
        {
            get { return checkBit(param[0], 1); }
            set { param[0] = setBit(param[0], 1, value); }
        }
        public bool o4
        {
            get { return checkBit(param[0], 0); }
            set { param[0] = setBit(param[0], 0, value); }
        }

        public byte odometr1
        {
            get { return param[1]; }
            set { param[1] = value; }
        }
        public byte odometr2
        {
            get { return param[2]; }
            set { param[2] = value; }
        }
        public byte odometr3
        {
            get { return param[3]; }
            set { param[3] = value; }
        }
        public byte odometr4
        {
            get { return param[4]; }
            set { param[4] = value; }
        }

        bool checkBit(byte b, int n)
        {
            return (b & (1 << n)) == 1 << n;
        }
        byte setBit(byte b, int n, bool isSet)
        { 
            if (isSet)
            {
                return b |= Convert.ToByte((1 << n));
            }
            else
            {
                return b &= Convert.ToByte(~(1 << n));
            }
        }
        public TLMAnswer(string name, string req, byte[] param = null) : base(name, param, req)
        {

        }
        public override void Accept(IAnswerVisitor visitor)
        {
            visitor.visit(this);
        }
        public override Answer Clone(byte[] param)
        {
            return new TLMAnswer(cmd, req, param);
        }

    }


    public class MoveParamsAnswer : Answer
    {
        public byte pwm_break
        {
            get { return param[0]; }
            set { param[0] = value; }
        }
        public byte pwm_full
        {
            get { return param[1]; }
            set { param[1] = value; }
        }
        public byte pwm_accel
        {
            get { return param[2]; }
            set { param[2] = value; }
        }

        public byte angle_break
        {
            get { return param[3]; }
            set { param[3] = value; }
        }
        public byte Koef1
        {
            get { return param[4]; }
            set { param[4] = value; }
        }
        public byte Koef2
        {
            get { return param[5]; }
            set { param[5] = value; }
        }

        public byte RainInf
        {
            get { return param[6]; }
            set { param[6] = value; }
        }
        //public MoveParamsAnswer(string name) : this(name, new byte[0])
        //{

        //}
        public MoveParamsAnswer(string name, string req, byte[] param = null) : base(name, new byte[7] , req)
        {

        }
        public override void Accept(IAnswerVisitor visitor)
        {
            visitor.visit(this);
        }
        public override Answer Clone(byte[] param)
        {
            return new MoveParamsAnswer(cmd, req, param);
        }

    }
    //public class CloseDomeAnswer : Answer
    //{
    //    public CloseDomeAnswer(string name) : base(name)
    //    {

    //    }
    //    public override void Accept(IAnswerVisitor visitor)
    //    {
    //        visitor.visit(this);
    //    }
    //    public override Answer Clone(byte[] param)
    //    {
    //        return new CloseDomeAnswer(cmd);
    //    }

    //}

    //public class OpenDomeAnswer : Answer
    //{
    //    public OpenDomeAnswer(string name) : base(name)
    //    {

    //    }
    //    public override void Accept(IAnswerVisitor visitor)
    //    {
    //        visitor.visit(this);
    //    }
    //    public override Answer Clone(byte[] param)
    //    {
    //        return new OpenDomeAnswer(cmd);
    //    }

    //}
    //public class StopDomeAnswer : Answer
    //{
    //    public StopDomeAnswer(string name) : base(name)
    //    {

    //    }
    //    public override void Accept(IAnswerVisitor visitor)
    //    {
    //        visitor.visit(this);
    //    }
    //    public override Answer Clone(byte[] param)
    //    {
    //        return new StopDomeAnswer(cmd);
    //    }

    //}
    public class StatusAnswer: Answer
    {
        
        public bool stateSh1 { 
            get { return BitConverter.ToBoolean(param, 0); }
            set { param[0] = Convert.ToByte(value); }
        }
        public bool stateSh2 { 
            get { return BitConverter.ToBoolean(param, 1); }
            set { param[1] = Convert.ToByte(value); }
        }
        public byte encoder1 { 
            get { return param[2]; }
            set { param[2] = value; }
        }
        public byte encoder2 { 
            get { return param[3]; }
            set { param[3] = value; }
        }
        public bool switch1 { 
            get { return BitConverter.ToBoolean(param, 10); }
            set { param[10] = Convert.ToByte(value); }
        }
        public bool switch2 { 
            get { return BitConverter.ToBoolean(param, 11); }
            set { param[11] = Convert.ToByte(value); }
        }
        public bool switch3 { 
            get { return BitConverter.ToBoolean(param, 12); }
            set { param[12] = Convert.ToByte(value); }
        }
        public bool ArmRainSensor { 
            get { return BitConverter.ToBoolean(param, 13); }
            set { param[13] = Convert.ToByte(value); }
        }
        public bool isRain { 
            get { return BitConverter.ToBoolean(param, 14); }
            set { param[14] = Convert.ToByte(value); }
        }

        //byte[] param = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0};

        //public StatusAnswer(string name) : this(name, new byte[0]) { }
        //public StatusAnswer(string name, byte[] param) : base(name, param)
        //{
        //    //if (param.Length == ASDConstant.STATUS_ANSWER_LEN)
        //    //    this.param = param;
        //    //else
        //    //    this.param = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //}
        public StatusAnswer(string name, string req, byte[] param = null ):base(name, new byte[15], req)
        {

        }
        public override void Accept(IAnswerVisitor visitor)
        {
            visitor.visit(this);
        }
        public override Answer Clone(byte[] param)
        {
            return new StatusAnswer(cmd, req, param);
        }
 

    }

    public abstract class IASDProtocol
    {
        protected  List<Cmd> cmdList = new List<Cmd>();
        public List<Cmd> getCmdList()
        {
            return cmdList;
        }
        public IAnswerVisitor visitor;

        public IASDProtocol(IAnswerVisitor visitor)
        {
            this.visitor = visitor;
        }
        abstract public void answer(byte[] b);
        // abstract public ASDRequest send(int idCmd, string param);
        abstract public void send(int idCmd, string param);
        abstract public void send(Cmd cmd);
        //abstract public ASDRequest status();
        abstract public Cmd status();
        abstract public Cmd StopDome();
        abstract public Cmd OpenDome();
        abstract public Cmd CloseDome();
        abstract public ShutterMoveDeg ShutterMoveDeg();
        abstract public SwitchToggle SwitchToggle();
        abstract public Armrain Armrain();
        abstract public Cmd GetMoveParams();
        abstract public SetMoveParams SetMoveParams();
        abstract public Cmd GetTLM();
    }




    internal class Asdome_protocol: IASDProtocol
    {
        protected ISender sender;
        protected List<Answer> answerList = new List<Answer> {
                new StatusAnswer("STATUS", "STATUS"),
                new MoveParamsAnswer("GETMOVEPARAMS", "MOVEPARAMS"),
                new TLMAnswer("GETTLM","TLM" )
            };

        public Asdome_protocol(ISender sender, IAnswerVisitor visitor):base(visitor)
        {
            this.sender = sender;

            cmdList.Add(new Cmd(this, "STATUS"));
            cmdList.Add(new Cmd(this, "STOPDOME"));
            cmdList.Add(new Cmd(this, "OPENDOME"));
            cmdList.Add(new Cmd(this, "CLOSEDOME"));
            cmdList.Add(new ShutterMoveDeg(this));
            cmdList.Add(new SwitchToggle(this));
            cmdList.Add(new Armrain(this));
            cmdList.Add(new Cmd(this, "GETMOVEPARAMS"));
            cmdList.Add(new SetMoveParams(this));
            cmdList.Add(new Cmd(this, "GETTLM"));
        }

        public override void answer(byte[] b)
        {
            string anwrStr = Encoding.ASCII.GetString(b);
            Answer answer = answerList.Find(anwr => anwrStr.Contains(anwr.cmd));
            if (answer != null)
            {
                anwrStr = anwrStr.Remove(anwrStr.IndexOf(answer.cmd), answer.cmd.Length);
                byte[] prm = new byte[b.Length - answer.cmd.Length];
                Array.Copy(b, answer.cmd.Length, prm, 0, prm.Length);
                answer.Clone(prm).Accept(visitor);
            }
        }
        public override Cmd status()
        {
            return cmdList.Find(x => x.cmd == "STATUS");
        }
    

        public override Cmd StopDome()
        {
            return cmdList.Find(x => x.cmd == "STOPDOME");
        }
        public override Cmd OpenDome()
        {
            return cmdList.Find(x => x.cmd == "OPENDOME");
        }
        public override Cmd CloseDome() {
            return cmdList.Find(x => x.cmd == "CLOSEDOME");
        }
        public override ShutterMoveDeg ShutterMoveDeg()
        {
            return cmdList.Find(x => x.cmd == "SHUTTERMOVEDEG") as ShutterMoveDeg;
        }
        public override SwitchToggle SwitchToggle()
        {
            return cmdList.Find(x => x.cmd == "SWITCHTOGGILE") as SwitchToggle;
        }
        public override Armrain Armrain()
        {
            return cmdList.Find(x => x.cmd == "ARMRAIN") as Armrain;
        }
        public override Cmd GetMoveParams()
        {
            return cmdList.Find(x => x.cmd == "GETMOVEPARAMS");
        }
        public override SetMoveParams SetMoveParams()
        {
            return cmdList.Find(x => x.cmd == "SETMOVEPARAMS") as SetMoveParams;
        }
        public override Cmd GetTLM()
        {
            return cmdList.Find(x => x.cmd == "GETTLM");
        }

        public override void send(Cmd cmd)
        {
            Logger.log(cmd.cmd);
            Console.WriteLine(string.Format("send: {0:HH:mm:ss} | {1}", DateTime.Now, BitConverter.ToString(cmd.Request())));
            sender.send(cmd.Request());
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

            List<byte> buff = Encoding.ASCII.GetBytes(cmdList[idCmd].cmd).ToList();

            byte[] pbuff = buff.Concat(dparam).ToArray();

            Logger.log(cmdList[idCmd].cmd);
            Console.WriteLine(string.Format("send: {0:HH:mm:ss} | {1}", DateTime.Now, BitConverter.ToString(pbuff)));

            sender.send(pbuff);
            //return new ASDRequest() { 
            //    buff = pbuff,
            //    len = pbuff.Length
            //};
        }
     
    }
}
