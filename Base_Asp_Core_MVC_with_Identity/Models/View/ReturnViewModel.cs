namespace Base_Asp_Core_MVC_with_Identity.Models.View
{
    public class ReturnViewModel
    {
        [Key]
        public Guid ID { get; set; }
        [Display(Name = "Mã nhập hàng")]

        public string ExportId { get; set; }
        public DateTime? ExportDate { get; set; }
        public string UserId { get; set; }
        public string SupplierId { get; set; }
        public string Description { get; set; }
        [Display(Name = "Tổng tiền")]

        public decimal TotalAmount { get; set; }
        [Display(Name = "Ngày trả hàng")]

        public DateTime? ReturnDate { get; set; }
        public List<Return_Product_Details> ReturnsDetails { get;set; } = new List<Return_Product_Details>();
    }
    public class Return_Product_Details
    {
        public Guid ID { get; set; }

        public string ExportProductId { get; set; }
        public string ProductId { get; }
        public string Description { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int Unit { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
