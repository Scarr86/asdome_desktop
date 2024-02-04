using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asdome_desktop
{
    internal class Controler
    {
        IProtocol asdptl;
        ISender sender;
        bool isSimulator = false;
        public Controler(ISender sender)
        {
            this.sender = sender;
            asdptl = new Asdome_protocol(sender);
        }
        public void send(int idCmd, string param)
        {
            asdptl.send(idCmd, param);
        }

        public void status()
        {
            asdptl.status();
        }
    }
}
