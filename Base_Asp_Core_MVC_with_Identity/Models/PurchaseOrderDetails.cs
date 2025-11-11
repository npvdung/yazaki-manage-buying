using MessagePack;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class PurchaseOrderDetails
    {
        [Key]
        public Guid ID { get; set; }
        public string PurchaseOrderId { get; set; }
        [Required]
        public string ProductId { get; set; }
        [Required]
        public decimal? Quantity { get; set; }
        public decimal? TaxAmount { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public decimal? DiscountAmount { get; set; }
        [Required]
        public decimal? TotalAmount { get; set; }
    }
}
