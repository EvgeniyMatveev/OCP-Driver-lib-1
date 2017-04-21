using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Neftm.Azsk.Hardware.CardTerminalIntf.Messages;
using Neftm.Azsk.Hardware.CardTerminalIntf.Messages.SpecialMessages;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Messages
{
    public class OcpEnqueryMessage: EnqueryMessage
    {
        public OcpEnqueryMessage()
        {
        }

        protected override void AddEnqueryParams(StringBuilder sb)
        {
            base.AddEnqueryParams(sb);
            sb.Append((char)0); // PV
            sb.Append((char)0); // CV
        }
    }
}
