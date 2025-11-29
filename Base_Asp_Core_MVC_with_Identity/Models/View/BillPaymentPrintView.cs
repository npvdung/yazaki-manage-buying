using System;

namespace MangagerBuyProduct.Models.View
{
    public class BillPaymentPrintView
    {
        public string BillPaymentCode   { get; set; }
        public DateTime BillPaymentDate { get; set; }

        public decimal TotalAmount       { get; set; }
        public decimal BillPaymentAmount { get; set; }

        public string? PaymentTerm  { get; set; }
        public string? StatusText   { get; set; }

        public string? ShipmentCode { get; set; }
        public string? StockName    { get; set; }
        public string? OrderCode { get; set; }   // Mã đơn hàng (ORDER_...)
        public string? OrderName { get; set; }
        public string? VendorName    { get; set; }
        public string? VendorAddress { get; set; }
        public string? VendorPhone   { get; set; }

        // Bên A
        public string CompanyAName  { get; set; }
    }
}
