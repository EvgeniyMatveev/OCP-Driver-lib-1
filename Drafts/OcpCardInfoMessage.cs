using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Neftm.Azsk.Hardware.CardTerminalIntf.Messages;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public class OcpCardInfoRequest: OcpMessage
    {
        public OcpCardInfoRequest()
        {
            Add(OcpTag.ID, (byte)14);
        }

        public string CardID { get; set; }

        protected override void PrepareData()
        {
            if (!String.IsNullOrEmpty(CardID))
            {
                Add(OcpTag.CardID, CardID);
            }
        }
    }
}
