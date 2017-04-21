using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neftm.Azsk.Hardware.CardTerminalDriver.Ocp
{
    public class OcpFiscalReceipt: OcpMessage<OcpFiscalReceipt.OcpTag>
    {
        public enum OcpTag : byte
        {
            Article = 0x01,
            Flags = 0x02,
            AmountWithoutDiscount = 0x03,
            DiscountForAmount = 0x04,
            Payments = 0x05
        };

        static OcpFiscalReceipt()
        {
            Info.Add(OcpTag.Article, new OcpTagInfo() { Type = OcpTagType.bin, ComplexType = typeof(OcpFiscalReceiptArticle) });
            Info.Add(OcpTag.Flags, new OcpTagInfo() { Type = OcpTagType.b, Len = 1 });
            Info.Add(OcpTag.AmountWithoutDiscount, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 2 });
            Info.Add(OcpTag.DiscountForAmount, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 2 });
            Info.Add(OcpTag.Payments, new OcpTagInfo() { Type = OcpTagType.bin, ComplexType = typeof(OcpFiscalReceiptPayments) });
        }

        public readonly List<OcpFiscalReceiptArticle> Articles = new List<OcpFiscalReceiptArticle>();
        public byte Flags { get; set; }
        public decimal AmountWithoutDiscount { get; set; }
        public decimal DiscountForAmount { get; set; }
        public readonly OcpFiscalReceiptPayments Payments = new OcpFiscalReceiptPayments();

        protected override byte TagToByte(OcpFiscalReceipt.OcpTag tag)
        {
            return (byte)(tag);
        }

        protected override OcpFiscalReceipt.OcpTag ByteToTag(byte b)
        {
            return (OcpFiscalReceipt.OcpTag)(b);
        }

        protected override void PrepareData()
        {
            foreach (OcpFiscalReceiptArticle article in Articles)
            {
                Add(OcpTag.Article, article);
            }
            if (Flags != 0)
                Add(OcpTag.Flags, Flags);
            Add(OcpTag.AmountWithoutDiscount, AmountWithoutDiscount);
            if (DiscountForAmount != 0m)
                Add(OcpTag.DiscountForAmount, DiscountForAmount);
            Add(OcpTag.Payments, Payments);
        }
    }

    public class OcpFiscalReceiptArticle : OcpMessage<OcpFiscalReceiptArticle.OcpTag>
    {
        public enum OcpTag : byte
        {
            GoodsCode = 0x01,
            Quantity = 0x02,
            PriceWithoutDiscount = 0x03,
            AmountWithoutDiscount = 0x04,
            GoodsName = 0x05,
            Flags = 0x06,
            DiscountForPrice = 0x07,
            DiscountForAmount = 0x08,
            Bonuses = 0x0A,
            PaymentType = 0x0B,
            MeasureUnit = 0x0C,
            QuantityPrecision = 0x0D
        };

        static OcpFiscalReceiptArticle()
        {
            Info.Add(OcpTag.GoodsCode, new OcpTagInfo() { Type = OcpTagType.asc, Len = 17 });
            Info.Add(OcpTag.Quantity, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 3 });
            Info.Add(OcpTag.PriceWithoutDiscount, new OcpTagInfo() { Type = OcpTagType.b, Len = 4, Precision = 2 });
            Info.Add(OcpTag.AmountWithoutDiscount, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 2 });
            Info.Add(OcpTag.GoodsName, new OcpTagInfo() { Type = OcpTagType.asc, Len = 127 });
            Info.Add(OcpTag.Flags, new OcpTagInfo() { Type = OcpTagType.b, Len = 1 });
            Info.Add(OcpTag.DiscountForPrice, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 2 });
            Info.Add(OcpTag.DiscountForAmount, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 2 });
        }

        public string GoodsCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceWithoutDiscount { get; set; }
        public decimal AmountWithoutDiscount { get; set; }
        public string GoodsName { get; set; }
        public byte Flags { get; set; }
        public decimal DiscountForPrice { get; set; }
        public decimal DiscountForAmunt { get; set; }
        public decimal Bonuses { get; set; }
        public OcpPaymentType PaymentType { get; set; }

        protected override byte TagToByte(OcpFiscalReceiptArticle.OcpTag tag)
        {
            return (byte)tag;
        }

        protected override OcpFiscalReceiptArticle.OcpTag ByteToTag(byte b)
        {
            return (OcpFiscalReceiptArticle.OcpTag)(b);
        }

        protected override void PrepareData()
        {
            Add(OcpTag.GoodsCode, GoodsCode);
            Add(OcpTag.Quantity, Quantity);
            Add(OcpTag.PriceWithoutDiscount, PriceWithoutDiscount);
            Add(OcpTag.AmountWithoutDiscount, AmountWithoutDiscount);
            if (!String.IsNullOrEmpty(GoodsName))
                Add(OcpTag.GoodsName, GoodsName);
            if (Flags != 0)
                Add(OcpTag.Flags, Flags);
            if (DiscountForPrice != 0m)
                Add(OcpTag.DiscountForPrice, DiscountForPrice);
            if (DiscountForAmunt != 0m)
                Add(OcpTag.DiscountForAmount, DiscountForAmunt);
        }
    }

    public enum OcpPaymentType : byte
    {
        None    = 0,
        Cash    = 1,
        BankingCard = 2,
        Bonuses = 3,
        Credit = 4,
        PrepaidAccount = 5
    }

    public class OcpFiscalReceiptPayments : OcpMessage<OcpFiscalReceiptPayments.OcpTag>
    {
        public enum OcpTag
        {
            AmountCash = 0x01,
            AmountBankingCard = 0x02,
            AmountBonuses = 0x03,
            AmountCredit = 0x04,
            AmountPrepaidAccount = 0x05,
            PaymentTypeForDiscount = 0x06
        };

        static OcpFiscalReceiptPayments()
        {
            Info.Add(OcpTag.AmountCash, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4 , Precision = 2 });
            Info.Add(OcpTag.AmountBankingCard, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 2 });
            Info.Add(OcpTag.AmountBonuses, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 2 });
            Info.Add(OcpTag.AmountCredit, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 2 });
            Info.Add(OcpTag.AmountPrepaidAccount, new OcpTagInfo() { Type = OcpTagType.bs, Len = 4, Precision = 2 });
            Info.Add(OcpTag.PaymentTypeForDiscount, new OcpTagInfo() { Type = OcpTagType.b, Len = 1 });
        }

        public decimal AmountCash { get; set; }
        public decimal AmountBankingCard { get; set; }
        public decimal AmountBonuses { get; set; }
        public decimal AmountCredit { get; set; }
        public decimal AmountPrepaidAccount { get; set; }
        public OcpPaymentType PaymentTypeForDiscount { get; set; }

        protected override byte TagToByte(OcpFiscalReceiptPayments.OcpTag tag)
        {
            return (byte)tag;
        }

        protected override OcpFiscalReceiptPayments.OcpTag ByteToTag(byte b)
        {
            return (OcpFiscalReceiptPayments.OcpTag)(b);
        }


        protected override void PrepareData()
        {
            if (AmountCash != 0m)
                Add(OcpTag.AmountCash, AmountCash);
            if (AmountBankingCard != 0m)
                Add(OcpTag.AmountBankingCard, AmountBankingCard);
            if (AmountBonuses != 0m)
                Add(OcpTag.AmountBonuses, AmountBonuses);
            if (AmountCredit != 0m)
                Add(OcpTag.AmountCredit, AmountCredit);
            if (AmountPrepaidAccount != 0m)
                Add(OcpTag.AmountPrepaidAccount, AmountPrepaidAccount);
        }
    }
}
