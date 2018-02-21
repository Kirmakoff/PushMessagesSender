using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushMessagesSender
{
    class Program
    {
       // private static readonly log4net.ILog log =
        //    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            PushMessaging pm = new PushMessaging();
            pm.SendMessage(pm.GetSampleMessage(SendingOptions.SendToTopic));


        }
    }
}
