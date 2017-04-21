using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public class OcpInfoResponse: OcpResponse
    {
        private int fID;
        private int fResponse;
        private decimal fAmount;
        private string fScreenMessage;
        private string fPrinterMessage;
        private string fPurchaseID;
        private string fCardID;

        public OcpInfoResponse()
        {
        }

        public int ID { get { return fID; } }
        public int Response { get { return fResponse; } }
        public decimal Amount { get { return fAmount; } }
        public string ScreenMessage { get { return fScreenMessage; } }
        public string PrinterMessage { get { return fPrinterMessage; } }
        public string PurchaseID { get { return fPurchaseID; } }
        public string CardID { get { return fCardID; } }

        public override void ParseData(byte[] data)
        {
            base.ParseData(data);

            if (tlv.ContainsKey(OcpTag.ID))
                fID = Convert.ToInt32(tlv[OcpTag.ID]);
            if (tlv.ContainsKey(OcpTag.Response))
                fResponse = Convert.ToInt32(tlv[OcpTag.Response]);
            if (tlv.ContainsKey(OcpTag.Amount))
                fAmount = Convert.ToDecimal(tlv[OcpTag.Amount]);
            if (tlv.ContainsKey(OcpTag.ScreenMessage))
                fScreenMessage = Convert.ToString(tlv[OcpTag.ScreenMessage]);
            if (tlv.ContainsKey(OcpTag.PrinterMessage))
                fPrinterMessage = Convert.ToString(tlv[OcpTag.PrinterMessage]);
            if (tlv.ContainsKey(OcpTag.PurchaseID))
                fPurchaseID = Convert.ToString(tlv[OcpTag.PurchaseID]);
            if (tlv.ContainsKey(OcpTag.CardID))
                fCardID = Convert.ToString(tlv[OcpTag.CardID]);
        }



    }
}
