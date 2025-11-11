namespace Base_Asp_Core_MVC_with_Identity.Models.View
{
    public class InvoiceViewModel
    {
        [Key]
        public Guid ID { get; set; }
        [Display(Name = "Mã hoá đơn")]
        public string InvoiceCode { get; set; }
        public string Description { get; set; }
        [Display(Name = "Ngày mua hàng")]
        public DateTime? InvoiceDate { get; set; }
        public int Quantity { get; set; }
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Tên khách hàng")]
        public string CustomerId { get; set; }
        public List<Invoice_Details> ProductDetails { get; set; } = new List<Invoice_Details>();
    }

    public class Invoice_Details
    {
        public Guid ID { get; set; }

        public string InvoiceId { get; set; }
        [Display(Name = "Tên sản phẩm")]
        public string ProductId { get; set; }
        public string ImportId { get; set; }
        public string Description { get; set; }
        public int Unit { get; set; }
        [Display(Name = "SL")]
        public int Quantity { get; set; }
        [Display(Name = "Đơn giá")]
        public decimal Price { get; set; }
        [Display(Name = "Giá")]
        public decimal TotalAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserId { get; set; }
    }
}
