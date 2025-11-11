using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class ShipmentRequest 
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Mã yêu cầu giao hàng")]
        public string ShipmentRequestCode { get; set; }

        [Required]
        [Display(Name = "Đơn mua")]
        public string? PurchaseOrderId { get; set; }

        [Display(Name = "Nhân viên lập")]
        public string? EmplouyeeId { get; set; }

        [Required]
        [Display(Name = "Loại giao hàng")]
        public string? ShipmentRequestType { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public string? Status { get; set; }

        [Required]
        [Display(Name = "Phương thức giao")]
        public string? Method { get; set; }

        [Required]
        [Display(Name = "Địa chỉ giao")]
        public string? Address { get; set; }

        [Required]
        [Display(Name = "Người giao")]
        public string? DelyveryPersonName { get; set; }

        [Required]
        [Display(Name = "SĐT người giao")]
        public string? DelyveryPersonPhone { get; set; }

        [Required]
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Required]
        [Display(Name = "Người nhận")]
        public string? RecipientsName { get; set; }

        [Required]
        [Display(Name = "SĐT người nhận")]
        public string? RecipientsPhone { get; set; }

        [Required]
        [Display(Name = "Ngày giao")]
        public DateTime? DateShip { get; set; }

        [Required]
        [Display(Name = "Số tiền")]
        public decimal? Amount { get; set; }

        [Required]
        [Display(Name = "Số tiền giảm")]
        public decimal? DiscountAmout { get; set; }

        [Required]
        [Display(Name = "Tổng chi phí")]
        public decimal TotalCost { get; set; }

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
