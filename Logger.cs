using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asdome_desktop
{
    internal interface ILoggerView
    {
        void logShow(string message);
    }
    internal class Logger
    {
        static public ILoggerView view;
        static int count = 0;
        public Logger()
        {
        }

        static public void log(string message)
        {
            view.logShow(string.Format("{0:000}. {1:HH:mm:ss} | {2}", ++count, DateTime.Now, message) );
        } 
    }
}
