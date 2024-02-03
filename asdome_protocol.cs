using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asdome_desktop
{

    internal interface ISender
    {
        void send(byte[] buff);
    }

    internal class Asdome_protocol
    {
        ISender sender;
        List<string> cmdList = new List<string>();
         
        public Asdome_protocol(ISender sender)
        {
            this.sender = sender;
            cmdList.Add("STATUS");
            cmdList.Add("STOPDOME");
            cmdList.Add("OPENDOME");
            cmdList.Add("CLOSEDOME");
            cmdList.Add("SHUTTERMOVEDEG");
            cmdList.Add("SWITCHTOGGILE");
            cmdList.Add("ARMRAIN");
            cmdList.Add("GETMOVEPARAMS");
            cmdList.Add("SETMOVEPARAMS");
            cmdList.Add("GETTLM");
        }
        public void status()
        {
            byte[] buff = Encoding.ASCII.GetBytes("STATUS");
            sender.send(buff);
        }
        public void send(byte[] buff)
        {
            sender.send(buff);
        }
        public List<string> getCmdList()
        {
            return cmdList;
        }
    }
}
