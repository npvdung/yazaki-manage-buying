using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class ReturnAuthorization
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Mã yêu cầu trả hàng")]
        public string ReturnAuthorizationCode { get; set; }

        [Required]
        [Display(Name = "Đơn mua")]
        public string PurchaseOrderId { get; set; }

        [Required]
        [Display(Name = "Phương thức vận chuyển")]
        public string? ShipingMethod { get; set; }

        [Required]
        [Display(Name = "Địa chỉ")]
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
        [Display(Name = "Ngày giao/trả")]
        public DateTime? DateShip { get; set; }

        [Required]
        [Display(Name = "Số tiền trả lại")]
        public decimal? AmountReturn { get; set; }

        [NotMapped]
        [Display(Name = "Chi tiết đơn mua")]
        public List<PurchaseOrderDetails> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetails>();
    }
}
