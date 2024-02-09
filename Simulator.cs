using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace asdome_desktop
{





    internal class Simulator: Asdome_protocol 
    {

        StatusAnswer stanwr = new StatusAnswer("STATUS", "STATUS", new byte[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MoveParamsAnswer mpanwr = new MoveParamsAnswer("GETMOVEPARAMS", "MOVEPARAMS", new byte[] {1, 1, 1, 60, 0, 0, 1});
        TLMAnswer tlmpanwr = new TLMAnswer("GETTLM", "TLM", new byte[] {0xFF, 0, 0, 0, 0,});
        SortedDictionary<string, Answer> cmdAwr = new SortedDictionary<string, Answer>();

        int num = 0;
        Timer timer = null;
        public Simulator(ISender sender, IAnswerVisitor visitor):base(sender, visitor) {

            cmdAwr.Add("STATUS", stanwr);
            cmdAwr.Add("GETMOVEPARAMS", mpanwr);
            cmdAwr.Add("GETTLM", tlmpanwr);

        }


        override public void send(Cmd cmd)
        {

            switch (cmd.cmd)
            {
                case "OPENDOME": SimOpenDome(); break;
                case "STOPDOME": SimStopDome(); break;
                case "CLOSEDOME": SimCloseDome(); break;
                case "SETMOVEPARAMS":
                    cmd.param.CopyTo(mpanwr.param, 0);
                 break;
                case "ARMRAIN":
                    stanwr.ArmRainSensor = ((Armrain)cmd).on;
                    break;
            }

            Logger.log(cmd.cmd);
            Console.WriteLine(string.Format("send: {0:HH:mm:ss} | {1}", DateTime.Now, BitConverter.ToString(cmd.Request())));
            if (cmdAwr.ContainsKey(cmd.cmd))
            {
                Console.WriteLine(string.Format("recv: {0:HH:mm:ss} | {1}", DateTime.Now, BitConverter.ToString(cmdAwr[cmd.cmd].GetBuffer())));
                cmdAwr[cmd.cmd].Accept(visitor);
            }

        }
        public void Simulation(object obj)
        {

            Console.WriteLine($"{num}");
            num++;
            //if (num > 3)
            //    ((AutoResetEvent)obj).Set();

        }

        void  SimOpenDome()
        {
            timer = new Timer(DecAngle, null, 0, 100);
        }
        void SimCloseDome()
        {
            timer = new Timer(IncAngle, null, 0, 100);
        }

        void SimStopDome()
        {
            if (timer != null)
                timer.Dispose();
        }

        public void DecAngle(object obj)
        {
            if ( stanwr.encoder1 < 1)
            {
                stanwr.encoder1 = 0;
                stanwr.stateSh1 = true;
            }
            else
            {
                stanwr.encoder1 -= 1;
            }

            if (stanwr.encoder2 < 1)
            {
                stanwr.encoder2 = 0;
                stanwr.stateSh2 = true;
            }
            else
            {
                stanwr.encoder2 -= 1;
            }
            if(stanwr.stateSh1 == true && stanwr.stateSh2 == true)
            {
                timer.Dispose();
            }
        }
        public void IncAngle(object obj)
        {
            if((stanwr.encoder1 + 1) > 90)
            {
                stanwr.encoder1 = 90;
                stanwr.stateSh1 = false;
            }
            else
            {
                stanwr.encoder1 += 1;
            }

            if ((stanwr.encoder2 + 1) > 90)
            {
                stanwr.encoder2 = 90;
                stanwr.stateSh2 = false;
            }
            else
            {
                stanwr.encoder2 += 1;
            }
            if (stanwr.stateSh1 == false && stanwr.stateSh2 == false)
            {
                timer.Dispose();
            }
        }

        public void Start()
        {
            //var autoEvent = new AutoResetEvent(false);
            //timer = new Timer(Simulation, null, 0, 1000);
            //autoEvent.WaitOne();
            //timer.Dispose();
        }
        public void Stop()
        {
            if(timer != null)
                timer.Dispose();
        }

    }
}
