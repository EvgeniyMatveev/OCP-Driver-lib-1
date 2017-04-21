using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public class OcpCancelRequest : OcpMessage
    {
        public OcpCancelRequest()
        {
            Add(OcpTag.ID, (byte)10);
            Flags = 128;
        }

        public ushort Flags { get; set; }
        public string CardID { get; set; }
        public string OriginalID { get; set; }

        protected override void PrepareData()
        {
            Add(OcpTag.Flags, Flags);
            Add(OcpTag.CardID, CardID);
            Add(OcpTag.OriginalID, OriginalID);
        }
    }
}
