using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Neftm.Azsk.Hardware.CardTerminalIntf.Messages;
using Neftm.Azsk.Hardware.CardTerminalDriver.Messages;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public class OcpMessageFactory: MessageFactory
    {
        public OcpMessageFactory()
        {
        }

        public override IMessage CreateENQMessage()
        {
            return
                new OcpEnqueryMessage();
        }

        public override IMessage ParseDataMessage(byte[] message)
        {
            OcpInfoResponse response = new OcpInfoResponse();
            response.ParseData(message);
            return
                response;
        }
    }
}
