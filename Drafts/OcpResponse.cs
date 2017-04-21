using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Neftm.Azsk.Hardware.CardTerminalIntf.Messages.Model;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public class OcpResponse: OcpMessage
    {
        public OcpResponse()
        {
        }

        protected override byte ReadStartTag()
        {
            byte b = base.ReadStartTag();
            if (b != (byte)SpecSymbol.STX)
                throw
                    new ApplicationException("Invalid response: STX missing");
            return
                b;
        }


    }
}
