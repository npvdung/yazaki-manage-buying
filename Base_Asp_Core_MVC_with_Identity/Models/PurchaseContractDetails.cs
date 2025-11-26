using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class PurchaseContractDetails
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Hợp đồng mua")]
        public string PurchaseContractId { get; set; }

        [Required]
        [Display(Name = "Sản phẩm")]
        public string ProductId { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        public decimal? Quantity { get; set; }

        [Display(Name = "Thuế")]
        public decimal? TaxAmount { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        [Required]
        [Display(Name = "Đơn giá")]
        public decimal? Price { get; set; }

        [Display(Name = "Chiết khấu")]
        public decimal? DiscountAmount { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        [Required]
        [Display(Name = "Thành tiền")]
        public decimal? TotalAmount { get; set; }
    }
}
