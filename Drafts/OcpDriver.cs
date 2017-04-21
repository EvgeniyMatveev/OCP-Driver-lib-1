using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Neftm.Azsk.Hardware.CardTerminalDriver.Port;
using Neftm.Azsk.Hardware.CardTerminalDriver.Messages;
using Neftm.Azsk.Hardware.CardTerminalIntf;
using Neftm.Azsk.Hardware.CardTerminalIntf.Messages;
using Neftm.Azsk.Hardware.CardTerminalIntf.Messages.Model;
using Neftm.Azsk.Hardware.CardTerminalIntf.Messages.Additional.CardInfo;
using Neftm.Azsk.Hardware.CardTerminalIntf.Messages.DebetMessages;
using Neftm.Azsk.Hardware.CardTerminalIntf.Messages.Additional.ReturnMessages;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public class OcpDriver: DriverBoss, ITerminal
    {
        private string fLastPurchaseID;

        public OcpDriver()
        {

        }

        public override WrappedAsyncPort CreatePort()
        {
            WrappedAsyncPort result = new OcpPort();
            return
                result;
        }

        public CardInfoMessageResponse CardInfoQuery(CardAction cardAction, string goodCode)
        {
            // CardAction and GoodCode are ignored in Opc driver
            OcpCardInfoRequest msg = new OcpCardInfoRequest();
            IMessage r = WrappedAsyncPort.SendMessage(msg);
            OcpInfoResponse resp = (OcpInfoResponse)r;
            CardInfoMessageResponse result = new CardInfoMessageResponse();
            if (resp != null)
            {
                result.cardNumber = resp.CardID.ToCharArray();
            }
            else
            {
                result.returnCode = ReturnCode.HostConnectionError;
            }
            return
                result;
        }

        public DebetResponseMessage SendDebet(CardAction cardAction, double sum, double amount, string goodCode)
        {
            fLastPurchaseID = null; // Reset PurchaseID

            string gname = goodCode;
            switch (gname)
            {
                case "10": gname = "Дт"; break;
                case "11": gname = "Дт-е"; break;
                case "92": gname = "Аи-92"; break;
                case "93": gname = "Аи-92-е"; break;
                case "95": gname = "Аи-95"; break;
                case "96": gname = "Аи-95-е"; break;
                case "98": gname = "Аи-98"; break;
            }

            OcpDebetRequest msg = new OcpDebetRequest();
            msg.Amount = Convert.ToDecimal(sum);
            msg.FiscalReceipt.AmountWithoutDiscount = Convert.ToDecimal(sum);
            msg.FiscalReceipt.Articles.Add(new OcpFiscalReceiptArticle()
                {
                    AmountWithoutDiscount = Convert.ToDecimal(sum),
                    GoodsCode = goodCode,
                    GoodsName = gname,
                    PaymentType = OcpPaymentType.PrepaidAccount,
                    PriceWithoutDiscount = Decimal.Round(Convert.ToDecimal(sum / amount), 2),
                    Quantity = Convert.ToDecimal(amount)
                }
                );
            msg.FiscalReceipt.Payments.AmountPrepaidAccount = Convert.ToDecimal(sum);
            IMessage r = WrappedAsyncPort.SendMessage(msg);
            OcpInfoResponse resp = (OcpInfoResponse)r;
            DebetResponseMessage result = new DebetResponseMessage();
            if (resp != null)
            {
                result.cardNumber = resp.CardID.ToCharArray();
                switch (resp.Response)
                {
                    case 0: result.returnCode = ReturnCode.Accepted; break;
                    case 1:
                    case 2: result.returnCode = ReturnCode.HostConnectionError; break;
                    case 3: result.returnCode = ReturnCode.OperationCancelled; break;
                    case 6: result.returnCode = ReturnCode.PinError; break;
                    default:
                        result.returnCode = ReturnCode.OrderCancelled; break;
                }
                fLastPurchaseID = resp.PurchaseID;
            }
            else
            {
                result.returnCode = ReturnCode.HostConnectionError;
            }
            return
                result;
        }

        public ReturnResponseMessage SendReturnMessage(CardAction cardAction, ReturnType returnType, int cardTypeCode, string cardNumber, double sum, double amount, string goodCode)
        {
            OcpCancelRequest msg = new OcpCancelRequest();
            msg.CardID = cardNumber;
            msg.OriginalID = fLastPurchaseID;
            IMessage r = WrappedAsyncPort.SendMessage(msg);
            OcpInfoResponse resp = (OcpInfoResponse)r;
            ReturnResponseMessage result = new ReturnResponseMessage();
            if (resp != null)
            {
                result.cardNumber = resp.CardID.ToCharArray();
                switch (resp.Response)
                {
                    case 0: result.returnCode = ReturnCode.Accepted; break;
                    case 1:
                    case 2: result.returnCode = ReturnCode.HostConnectionError; break;
                    case 3: result.returnCode = ReturnCode.OperationCancelled; break;
                    case 6: result.returnCode = ReturnCode.PinError; break;
                    default:
                        result.returnCode = ReturnCode.OrderCancelled; break;
                }
            }
            else
            {
                result.returnCode = ReturnCode.HostConnectionError;
            }
            return
                result;
        }
    }
}
