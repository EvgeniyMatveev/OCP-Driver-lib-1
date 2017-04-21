using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public class OcpDebetRequest : OcpMessage
    {
        public OcpDebetRequest()
        {
            Add(OcpTag.ID, (byte)4);
            DateTime = DateTime.Now;
        }

        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public readonly OcpFiscalReceipt FiscalReceipt = new OcpFiscalReceipt();

        protected override void PrepareData()
        {
            Add(OcpTag.Amount, Amount);
            Add(OcpTag.DateTime, DateTime);
            Add(OcpTag.FiscalReceipt, FiscalReceipt);
        }
    }
}
