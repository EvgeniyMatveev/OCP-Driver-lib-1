using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neftm.Azsk.Hardware.CardTerminalDriver.Messages;
using Neftm.Azsk.Hardware.CardTerminalDriver.Messages.Model;
using Neftm.Azsk.Hardware.CardTerminalIntf.Messages.Model;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public class OcpMessageQueue: IMessageQueue
    {
        private byte[] pending;
        private int pendix;
        private int pendlen;

        private readonly Queue<byte[]> _messageQueue = new Queue<byte[]>();

        public OcpMessageQueue()
        {
            pending = new byte[16000];
        }

        #region IMessageQueue Members

        public int GetMessageCount()
        {
            return
                _messageQueue.Count;
        }

        public object PopMessage()
        {
            return _messageQueue.Dequeue();
        }

        public void PushData(byte[] inBuffer)
        {
            int idx = 0;
            PushData(inBuffer, ref idx);
        }

        public void Reset()
        {
            _messageQueue.Clear();
        }

        public Neftm.Azsk.Hardware.CardTerminalDriver.Messages.Model.UnformattedMessageType GetLastMessageType()
        {
            return UnformattedMessageTypeFactory.GetItem((char)_messageQueue.First()[0]);
        }

        #endregion

        private void PushData(byte[] buffer, ref int idx)
        {
            if (pendlen > 0)
            {
                // If message is pending, read to pending buffer
                PushPending(buffer, ref idx);
                if (pendlen > 0)
                    // If still pending, wait for other data
                    return;
            }

            while (idx < buffer.Length)
                {
                    byte c = buffer[idx];
                    switch (c)
                    {
                        case (byte)SpecSymbol.ACK:
                        case (byte)SpecSymbol.BEL:
                        case (byte)SpecSymbol.CAN:
                        case (byte)SpecSymbol.EOT:
                        case (byte)SpecSymbol.ETX:
                        case (byte)SpecSymbol.NAK:
                            Push(c);
                            idx += 1;
                            break;

                        case (byte)SpecSymbol.ENQ:
                            // Read CV and PV
                            Push(buffer, idx, 3);
                            idx += 3;
                            break;

                        case (byte)SpecSymbol.STX:
                            // read DataLength
                            int i = idx;
                            idx += 1;
                            if (idx < buffer.Length)
                            {
                                int len = OcpUtil.ReadDataSize(buffer, ref idx);
                                if (len > pending.Length)
                                    throw
                                        new NotSupportedException("Too large message");

                                pendlen = len + (idx - i) + 1 /* ETX */ + 2 /* CRC */;
                                pendix = 0;
                                idx = i;
                                PushPending(buffer, ref idx);
                            }
                            else
                            {
                                pendlen = 1; // At least one sym for len
                                pendix = 0;
                                PushPending(buffer, ref idx);
                            }
                            break;
                    }
                }
        }

        private void PushPending(byte[] buffer, ref int idx)
        {
            int rem = buffer.Length - idx;
            if (rem >= pendlen)
            {
                Array.Copy(buffer, idx, pending, pendix, pendlen);
                Push(pending, 0, pendix + pendlen);
                idx += pendlen;
                pendlen = 0;
                pendix = 0;
            }
            else
            {
                Array.Copy(buffer, idx, pending, pendix, rem);
                pendix += rem;
                pendlen -= rem;
                idx += rem;
            }
        }

        private void Push(byte b)
        {
            _messageQueue.Enqueue(new byte[]{ b } );
        }

        private void Push(byte[] buffer, int index, int len)
        {
            byte[] b = new byte[len];
            Array.Copy(buffer, index, b, 0, len);
            _messageQueue.Enqueue(b);
        }
    }
}
