using System;
using System.Text;

using Neftm.Azsk.Hardware.CardTerminalIntf.Messages;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public enum OcpTag: byte
    {
        ID = 0x01,
        Response = 0x02,
        Amount = 0x03,
        Flags = 0x04,
        DateTime = 0x05,
        ScreenMessage = 0x06,
        PrinterMessage = 0x07,
        FiscalReceipt = 0x08,
        PurchaseID = 0x09,
        CancellationID = 0x0A,
        OriginalID = 0x0B,
        Timeout = 0x0C,
        Operation = 0x0D,
        PossibleBehaviour = 0x0E,
        SelectedBehaviourID = 0x0F,
        AdditionalCardInfo = 0x10,
        CardType = 0x11,
        AllowedPaymentTypes = 0x12,
        CardID = 0x13,
        RRN = 0x14,
        AuthCode = 0x15,
        Track1 = 0x16,
        Track2 = 0x17,
        CardAction = 0x18,
        ReceiptNumber = 0x20,
        ECRID = 0x21,
        ShopID = 0x22,
    }

    public enum OcpTagType
    {
        a,
        n,
        an,
        ans,
        asc,
        ascISO,
        b,
        bs,
        bin
    }

    public class OcpTagInfo
    {
        public static readonly Encoding EncodingAsc = Encoding.GetEncoding(866);

        public OcpTagType Type { get; set; }
        public int Len { get; set; }
        public int Precision { get; set; }
        public Type ComplexType { get; set; }

        public byte[] FormatValue(object value)
        {
            byte[] res = null;
            switch (Type)
            {
                case OcpTagType.b:
                    if (value is byte)
                    {
                        res = new byte[] { (byte)value };
                        break;
                    }
                    if (value is DateTime)
                    {
                        DateTime dt = new DateTime(1970, 1, 1);
                        int r = Convert.ToInt32(((DateTime)value - dt).TotalSeconds);
                        res = BitConverter.GetBytes(r);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(res);
                        break;
                    }
                    if (value is Decimal)
                    {
                        decimal v = (decimal)value;
                        for (int i = 0; i < Precision; i++) v *= 10.0m;
                        res = BitConverter.GetBytes(Decimal.ToUInt32(v));
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(res);
                        break;
                    }
                    if (value is byte[])
                    {
                        res = (byte[])value;
                        break;
                    }

                    uint u = Convert.ToUInt32(value);
                    byte[] b = BitConverter.GetBytes(u);
                    res = new byte[Len];
                    for (int i = 0; (i < b.Length) && (i < res.Length); i++)
                    {
                        res[res.Length - 1 - i] = b[i];
                    }
                    break;

                case OcpTagType.bin:
                    if (value is IMessage)
                    {
                        res = ((IMessage)value).GetMessageData();
                    }
                    break;

                case OcpTagType.bs:
                    if (value is int)
                    {
                        res = BitConverter.GetBytes((int)value);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(res);
                        break;
                    }
                    if (value is Decimal)
                    {
                        decimal v = (decimal)value;
                        for (int i = 0; i < Precision; i++) v *= 10.0m;
                        res = BitConverter.GetBytes(Decimal.ToInt32(v));
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(res);
                        break;
                    }
                    break;

                case OcpTagType.asc:
                    string s = Convert.ToString(value);
                    res = EncodingAsc.GetBytes(s);
                    break;

                case OcpTagType.n:
                    if (value is String)
                        value = ((String)value).ToCharArray();
                    if (value is char[])
                    {
                        char[] lv = (char[])value;
                        res = new byte[(lv.Length + 1) / 2];
                        for (int i = 0; i < res.Length; i += 1)
                        {
                            byte c1 = (byte)(lv[i * 2] - '0');
                            byte c2 = (byte)0;
                            if (i * 2 + 1 < lv.Length)
                                c2 = (byte)(lv[i * 2 + 1] - '0');
                            byte x = (byte)((int)c1 << 4);
                            res[i] = (byte)(x + c2);
                        }
                    }
                    break;

            }
            return
                res;
        }

        public object GetValue(byte[] data)
        {
            object result = null;
            switch (Type)
            {
                case OcpTagType.b:
                    if (Len == 1)
                        result = data[0];
                    else
                        result = data;
                    break;

                case OcpTagType.bs:
                    if (Len == 2)
                    {
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(data, 0, 2);
                        result = BitConverter.ToInt16(data, 0);
                        break;
                    }
                    if (Len == 4)
                    {
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(data, 0, 4);
                        result = BitConverter.ToInt32(data, 0);
                        break;
                    }
                    break;

                case OcpTagType.asc:
                    result = EncodingAsc.GetString(data);
                    break;

                case OcpTagType.n:
                    char[] lv = new char[data.Length * 2];
                    for (int i = 0; i < data.Length; i++)
                    {
                        byte c1 = (byte)(((byte)(data[i] & (byte)0xF0)) >> 4);
                        byte c2 = (byte)(data[i] & (byte)0x0F);
                        lv[i * 2] = (char)('0' + (char)c1);
                        lv[i * 2 + 1] = (char)('0' + (char)c2);
                    }
                    result = new String(lv);
                    break;

                case OcpTagType.bin:
                    if (ComplexType != null)
                    {
                        if (ComplexType.IsSubclassOf(typeof(OcpMessageBase)))
                        {
                            OcpMessageBase m = (OcpMessageBase)Activator.CreateInstance(ComplexType);
                            m.ReadData(data);
                            result = m;
                            break;
                        }
                    }
                    result = null;
                    break;

                default:
                    result = 0;
                    break;
            }
            return
                result;
        }

        public string GetValueString(object value)
        {
            if (value is char[])
                return
                    new string((char[])value);
            return
                Convert.ToString(value);
        }
    }

    public static class OcpUtil
    {
        public static byte[] CalculateDataSize(byte[] data)
        {
            byte[] result = null;
            byte[] b;
            if (data != null)
            {
                int len = data.Length;
                if (len < 0x80)
                {
                    result = new byte[] { (byte)len };
                    return
                        result;
                }
                if (len < 0x100)
                {
                    result = new byte[] { (byte)0x81, (byte)len };
                    return
                        result;
                }
                if (len < 0x1000)
                {
                    b = BitConverter.GetBytes(len);
                    result = new byte[3];
                    result[0] = (byte)0x82;
                    result[1] = b[0];
                    result[2] = b[1];
                    return
                        result;
                }

                b = BitConverter.GetBytes(len);
                result = new byte[3];
                result[0] = (byte)0x84;
                result[1] = b[0];
                result[2] = b[1];
                result[3] = b[2];
                result[4] = b[3];
                return
                    result;
            }
            return
                null;
        }

        public static char[] BytesToChars(byte[] data)
        {
            char[] result = Encoding.GetEncoding(866).GetChars(data);
            return
                result;
        }

        public static byte[] CharsToBytes(char[] data)
        {
            byte[] result = Encoding.GetEncoding(866).GetBytes(data);
            return
                result;
        }

        public static int ReadDataSize(byte[] data, ref int index)
        {
            int result = 0;
            byte lf = data[index++];
            if (lf < 0x80)
            {
                result = (int)lf;
            }
            else
            {
                switch (lf)
                {
                    case 0x81:
                        result = (int)data[index++];
                        break;

                    case 0x82:
                        if (BitConverter.IsLittleEndian)
                        {
                            byte[] lb = new byte[2];
                            lb[1] = data[index++];
                            lb[0] = data[index++];
                            result = (int)BitConverter.ToUInt16(lb, 0);
                        }
                        else
                        {
                            result = (int)BitConverter.ToUInt16(data, index);
                            index += 2;
                        }
                        break;

                    case 0x84:
                        if (BitConverter.IsLittleEndian)
                        {
                            byte[] lb = new byte[4];
                            lb[3] = data[index++];
                            lb[2] = data[index++];
                            lb[1] = data[index++];
                            lb[0] = data[index++];
                            result = (int)BitConverter.ToUInt32(lb, index);
                        }
                        else
                        {
                            result = (int)BitConverter.ToUInt32(data, index);
                            index += 4;
                        }
                        break;
                }
            }
            return
                result;
        }

        public static int ReadDataSize(char[] data, ref int index)
        {
            byte[] d = CharsToBytes(data);
            return
                ReadDataSize(d, ref index);
        }
    }
}
