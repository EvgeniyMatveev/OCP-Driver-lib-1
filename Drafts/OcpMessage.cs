using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;

using Neftm.Azsk.Hardware.CardTerminalIntf.Messages;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public abstract class OcpMessageBase
    {
        private byte[] data;
        private int index;

        public OcpMessageBase()
        {
        }

        public virtual void ParseData(byte[] data)
        {
            this.data = data;
            index = 0;

            ReadStartTag();
            int len = ReadDataSize();
            ReadData(len);
        }

        public virtual void ReadData(byte[] data)
        {
            this.data = data;
            index = 0;

            int len = data.Length;
            ReadData(len);
        }

        protected virtual byte ReadStartTag()
        {
            // Skip start tag
            byte b = data[index++];
            return
                b;
        }

        private int ReadDataSize()
        {
            int result = OcpUtil.ReadDataSize(data, ref index);
            return
                result;
        }

        private void ReadData(int len)
        {
            while (len > 0)
            {
                len -= ReadTLV();
            }
        }

        private int ReadTLV()
        {
            int result = index;
            byte bT = data[index++];
            int bL = ReadDataSize();
            byte[] bV = new byte[bL];
            Array.Copy(data, index, bV, 0, bL);
            index += bL;

            Add(bT, bV);

            result = index - result;
            return
                result;
        }

        protected abstract void Add(byte bT, byte[] bV);
    }

    public class OcpMessage<TagNamespace>: OcpMessageBase, IMessage
    {
        public static readonly Dictionary<TagNamespace, OcpTagInfo> Info = new Dictionary<TagNamespace,OcpTagInfo>();

        private byte[] data;
        private int index;

        protected Dictionary<TagNamespace, object> tlv;

        public OcpMessage()
        {
            tlv = new Dictionary<TagNamespace, object>();
        }

        public void Add(TagNamespace tag, object value)
        {
            tlv.Add(tag, value);
        }

        public void Add(TagNamespace tag, byte[] data)
        {
            object value = GetValue(tag, data);
            tlv.Add(tag, value);
        }

        public void Remove(TagNamespace tag)
        {
            tlv.Remove(tag);
        }

        public object this[TagNamespace key]
        {
            get { return tlv[key]; }
        }

        protected virtual void PrepareData()
        {
        }

        protected virtual byte TagToByte(TagNamespace tag)
        {
            return
                ((byte)0);
        }

        protected virtual TagNamespace ByteToTag(byte b)
        {
            return
                default(TagNamespace);
        }

        public void Trace()
        {
            System.Diagnostics.Trace.WriteLine("Message TLV table:");
            foreach (var x in tlv)
            {
                OcpTagInfo ti = Info[x.Key];
                System.Diagnostics.Trace.WriteLine(String.Format("T={0}:V=[{1}]{2}", x.Key, x.Value.GetType().Name, ti.GetValueString(x.Value)));
            }
            System.Diagnostics.Trace.WriteLine("END Message TLV table");
        }

        #region IMessage Members

        public byte[] GetMessageData()
        {
            PrepareData();

            List<byte> buf = new List<byte>();
            foreach (var e in tlv)
            {
                AddFormatEntry(buf, e.Key, e.Value);
            }
            return
                buf.ToArray();
        }

        private void AddFormatEntry(List<byte> buf, TagNamespace tag, object value)
        {
            buf.Add(TagToByte(tag));
            byte[] v = FormatValue(tag, value);
            byte[] len = OcpUtil.CalculateDataSize(v);
            buf.AddRange(len);
            buf.AddRange(v);
        }

        private byte[] FormatValue(TagNamespace tag, object value)
        {
            OcpTagInfo ti;
            try
            {
                ti = Info[tag];
            }
            catch (KeyNotFoundException)
            {
                throw
                    new ApplicationException(String.Format("Unknown tag ({0})", tag));
            }
            byte[] result = ti.FormatValue(value);
            return
                result;
        }

        private object GetValue(TagNamespace tag, byte[] data)
        {
            OcpTagInfo ti;
            try
            {
                ti = Info[tag];
            }
            catch (KeyNotFoundException)
            {
                throw
                    new ApplicationException(String.Format("Unknown tag ({0})", tag));
            }
            object result = ti.GetValue(data);
            return
                result;
        }

        #endregion

        protected override void Add(byte bT, byte[] bV)
        {
            Add(ByteToTag(bT), bV);
        }
    }

    public class OcpMessage : OcpMessage<OcpTag>
    {
        static OcpMessage()
        {
            Info.Add(OcpTag.ID, new OcpTagInfo() { Type = OcpTagType.b, Len = 1 });
            Info.Add(OcpTag.Response, new OcpTagInfo() { Type = OcpTagType.b, Len = 1 });
            Info.Add(OcpTag.Amount, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 2 });
            Info.Add(OcpTag.Flags, new OcpTagInfo() { Type = OcpTagType.b, Len = 2 });
            Info.Add(OcpTag.DateTime, new OcpTagInfo() { Type = OcpTagType.b, Len = 4 });
            Info.Add(OcpTag.ScreenMessage, new OcpTagInfo() { Type = OcpTagType.asc, Len = 2048 });
            Info.Add(OcpTag.PrinterMessage, new OcpTagInfo() { Type = OcpTagType.asc, Len = 2048 });
            Info.Add(OcpTag.FiscalReceipt, new OcpTagInfo() { Type = OcpTagType.bin, ComplexType = typeof(OcpFiscalReceipt) });
            Info.Add(OcpTag.PurchaseID, new OcpTagInfo() { Type = OcpTagType.n, Len = 27 });
            Info.Add(OcpTag.CancellationID, new OcpTagInfo() { Type = OcpTagType.n, Len = 27 });
            Info.Add(OcpTag.OriginalID, new OcpTagInfo() { Type = OcpTagType.n, Len = 27 });
            Info.Add(OcpTag.Timeout, new OcpTagInfo() { Type = OcpTagType.b, Len = 4 });
            Info.Add(OcpTag.Operation, new OcpTagInfo() { Type = OcpTagType.b, Len = 1 });
            Info.Add(OcpTag.PossibleBehaviour, new OcpTagInfo() { Type = OcpTagType.bin });
            Info.Add(OcpTag.SelectedBehaviourID, new OcpTagInfo() { Type = OcpTagType.b, Len = 1 });
            Info.Add(OcpTag.AdditionalCardInfo, new OcpTagInfo() { Type = OcpTagType.bin });
            Info.Add(OcpTag.CardType, new OcpTagInfo() { Type = OcpTagType.b, Len = 1 });
            Info.Add(OcpTag.AllowedPaymentTypes, new OcpTagInfo() { Type = OcpTagType.b, Len = 2 });
            Info.Add(OcpTag.CardID, new OcpTagInfo() { Type = OcpTagType.asc, Len = 50 });
            Info.Add(OcpTag.RRN, new OcpTagInfo() { Type = OcpTagType.an, Len = 12 });
            Info.Add(OcpTag.AuthCode, new OcpTagInfo() { Type = OcpTagType.an, Len = 6 });
            Info.Add(OcpTag.Track1, new OcpTagInfo() { Type = OcpTagType.asc, Len = 77 });
            Info.Add(OcpTag.Track2, new OcpTagInfo() { Type = OcpTagType.asc, Len = 38 });
            Info.Add(OcpTag.CardAction, new OcpTagInfo() { Type = OcpTagType.bin });
            Info.Add(OcpTag.ReceiptNumber, new OcpTagInfo() { Type = OcpTagType.b, Len = 4 });
            Info.Add(OcpTag.ECRID, new OcpTagInfo() { Type = OcpTagType.b, Len = 4 });
            Info.Add(OcpTag.ShopID, new OcpTagInfo() { Type = OcpTagType.b, Len = 4 });
        }

        protected override byte TagToByte(OcpTag tag)
        {
            return (byte)tag;
        }

        protected override OcpTag ByteToTag(byte b)
        {
            return (OcpTag)(b);
        }
    }
}

