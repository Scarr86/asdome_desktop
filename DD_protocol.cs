using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asdome_desktop
{
    delegate void Resolve(params int[] str);
    internal class DD_protocol
    {
        ISender sender;
        Resolve resolve;
        string[] cmd;
        List<int> data = new List<int>();

        public DD_protocol(ISender sender) {
            this.sender = sender;
        }

        public DD_protocol send(params string[] cmd)
        {
            //this.resolve = resolve;
            this.cmd = cmd;
            //sender.send(Encoding.ASCII.GetBytes(cmd[0]));
            //resolve(cmd, cmd);
            return this;
        }
        public void on(Resolve resolve)
        {
            this.resolve = resolve;
            data.Clear();
            sender.send(Encoding.ASCII.GetBytes(cmd[0]));
            
        }
        public void answer(byte[] b)
        {
            string anwrStr = Encoding.ASCII.GetString(b);

            for (int i = 1; i < cmd.Length; i++)
            {
                if (anwrStr.Contains(cmd[i]))
                {
                    anwrStr = anwrStr.Remove(0, anwrStr.IndexOf(cmd[i]) + cmd[i].Length + 1);

                    data.Add(int.Parse(anwrStr.Substring(0, anwrStr.IndexOf("\r"))));
                }
            }
            resolve(data.ToArray());
        }
    }
}
