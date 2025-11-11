namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Import_Product_Details
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public string ImportProductId { get; set; }
        [Required]
        [Display(Name = "Tên sản phẩm")]
        public string ProduceId { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime? ProductionBatch { get; set; }
        [Required]
        public DateTime? ManufacturingDate { get; set; }
        [Required]
        public DateTime? ExpirationData { get; set; }
        public int Unit { get; set; }
        [Display(Name = "SL")]
        public int Quantity { get; set; }
        [Display(Name = "Đơn giá")]
        public decimal Price { get; set; }
    }
}
