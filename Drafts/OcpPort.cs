using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Neftm.Azsk.Hardware.CardTerminalDriver.Messages;
using Neftm.Azsk.Hardware.CardTerminalDriver.Messages.Model;
using Neftm.Azsk.Hardware.CardTerminalDriver.Properties;
using Neftm.Azsk.Hardware.CardTerminalIntf.Exceptions;
using Neftm.Azsk.Hardware.CardTerminalIntf.Messages;

using Neftm.Azsk.Hardware.CardTerminalDriver.Port;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public class OcpPort: WrappedAsyncPort
    {
        public OcpPort()
        {
            mfactory = new OcpMessageFactory();
            formatter = new OcpMessageFormatter();
            FinalizeOnSend = false;
            FinalizeOnReceive = true;
        }

        protected override IMessageQueue CreateMessageQueue()
        {
            return new OcpMessageQueue();
        }

        /*
        public override void SendData(string s)
        {
            SendData(Encoding.GetEncoding(866).GetBytes(s));
        }
        */
    }
}
