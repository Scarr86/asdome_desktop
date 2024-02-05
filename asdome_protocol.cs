using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asdome_desktop
{

    internal interface ISender
    {
        void send(byte[] buff);
    }
    internal interface IProtocol
    {
        void send(int idCmd, string param);
        void status();
    }

    internal class Asdome_protocol: IProtocol
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
        SortedDictionary<string, string> logStr = new SortedDictionary<string, string>();

        public Asdome_protocol(ISender sender)
        {
            this.sender = sender;
            //foreach (var cmd in cmdList) {
            //    logStr.Add(cmd, cmd);
            //}
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
        public void status()
        {
            //byte[] buff = Encoding.ASCII.GetBytes("STATUS");
            //sender.send(buff);
            send(cmdList.FindIndex(0, x => x == "STATUS"), "");
        }
        public void send(int idCmd, string param)
        {
            if(idCmd < 0 || idCmd > cmdList.Count - 1) {
                return;
            }

            string[] p = param.Trim().Length == 0 ? new string[0] : param.Split(',');
            List<byte> dparam = new List<byte>();
            foreach (string s in p)
            {
                dparam.Add(byte.Parse(s));
                dparam.Add(0x2c);
            }
            if (dparam.Count > 0)
                dparam.RemoveAt(dparam.Count - 1);
            List<byte> buff = Encoding.ASCII.GetBytes(cmdList[idCmd]).ToList();
            byte[] pbuff = buff.Concat(dparam).ToArray();
            sender.send(pbuff);
            Logger.log(logStr[cmdList[idCmd]] + param);
        }
        static public List<string> getCmdList()
        {
            return cmdList;
        }
    }
}
