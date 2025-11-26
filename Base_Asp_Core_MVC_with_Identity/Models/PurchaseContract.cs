using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class PurchaseContract
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Mã hợp đồng mua")]
        public string PurchaseContractCode { get; set; }

        [Required]
        [Display(Name = "Tên hợp đồng mua")]
        public string PurchaseContractName { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Nhà cung cấp")]
        public string? VenderId { get; set; }

        [Required]
        [Display(Name = "Nhân viên lập")]
        public string? EmployeeId { get; set; }

        [Display(Name = "Đơn vị tính")]
        public string? Unit { get; set; }

        [Display(Name = "Thuế")]
        public decimal? TotalAmountIncludeTax { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        [Required]
        [Display(Name = "Tổng tiền")]
        public decimal? TotalAmount { get; set; }

        [Display(Name = "Chiết khấu")]
        public decimal? TotalDiscoutAmount { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tổng tiền sau thuế & CK")]
        public decimal? TotalAmountIncludeTaxAndDiscount { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public int? Status { get; set; }
    }
}
