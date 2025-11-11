using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class ItemReceipt
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Phiếu yêu cầu giao hàng")]
        public string ShipmentRequestId { get; set; }

        [Required]
        [Display(Name = "Kho nhận")]
        public string? StockId { get; set; }

        [Display(Name = "Nhân viên nhận")]
        public string? EmployeeId { get; set; }

        [Display(Name = "Mô tả")]
        public string? Discription { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public int? Status { get; set; }

        [NotMapped]
        [Display(Name = "Thuế")]
        public decimal? TotalAmountIncludeTax { get; set; }

        [NotMapped]
        [Display(Name = "Tổng tiền")]
        public decimal? TotalAmount { get; set; }

        [NotMapped]
        [Display(Name = "Chiết khấu")]
        public decimal? TotalDiscoutAmount { get; set; }

        [NotMapped]
        [Display(Name = "Tổng tiền sau thuế & CK")]
        public decimal? TotalAmountIncludeTaxAndDiscount { get; set; }
    }
}
