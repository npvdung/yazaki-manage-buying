using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class PurchaseOrderDetails
    {
        [Key]
        public Guid ID { get; set; }
        public string PurchaseOrderId { get; set; }
        [Required]
        [Display(Name = "Mã đặt hàng")]
        public string ProductId { get; set; }
        [Required]
        [Display(Name = "Số lượng")]
        public decimal? Quantity { get; set; }
        public decimal? TaxAmount { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        [Required]
        [Display(Name = "Đơn giá")]
        public decimal? Price { get; set; }
        public decimal? DiscountAmount { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        [Required]
        [Display(Name = "Thành tiền")]
        public decimal? TotalAmount { get; set; }
    }
}
