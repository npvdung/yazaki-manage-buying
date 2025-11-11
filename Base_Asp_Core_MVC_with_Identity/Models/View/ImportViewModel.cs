
namespace Base_Asp_Core_MVC_with_Identity.Models.View
{
    public class ImportViewModel
    {
        [Key]
        public Guid ID { get; set; }
        [Display(Name = "Mã nhập hàng")]
        public string ImportCode { get; set; }
        [Display(Name = "Tên phiếu nhập")]
        public string ImportName { get; set; }
        [Display(Name = "Ngày nhập")]
        public DateTime? ImportDate { get; set; }
        public string UserId { get; set; }
        public string SupplierId { get; set; }
        public string Description { get; set; }
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }
        public List<Import_Product_Details> ProductDetails { get; set; } = new List<Import_Product_Details>();
    }
    public class Import_Product_Details
    {
        public Guid ID { get; set; }
        [Display(Name = "Mã phiếu nhập")]
        public string ImportProductId { get; set; }
        [Display(Name = "Tên sản phẩm")]
        public string ProduceId { get; set; }
        public string Description { get; set; }
        [Display(Name = "Ngày nhập")]
        public DateTime? ProductionBatch { get; set; }
        [Display(Name = "Ngày sản xuất")]
        public DateTime? ManufacturingDate { get; set; }
        [Display(Name = "HSD")]
        public DateTime? ExpirationData { get; set; }
        public int Unit { get; set; }
        [Display(Name = "SL")]
        public int Quantity { get; set; }
        [Display(Name = "Giá bán")]
        public decimal Price { get; set; }
    }
}
