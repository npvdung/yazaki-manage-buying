using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Quota
    {
        [Key]
        public Guid ID { get; set; }
      
        [Display(Name = "Mã yêu cầu mua")]
        public string QuotaCode { get; set; }

        [Required]
        [Display(Name = "Tên yêu cầu mua")]
        public string QuotaName { get; set; }

        [Required]
        [Display(Name = "Loại vật tư")]
        public string? CategoryId { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Nhân viên lập")]
        public string? EmployeeId { get; set; }

        [Required]
        [Display(Name = "Tên mặt hàng")]
        public string? ProductId { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        public int? Quantity { get; set; }

        [Required]
        [Display(Name = "Số lượng đã sử dụng")]
        public int? UsedQuantity { get; set; }

        [Required]
        [Display(Name = "Số lượng còn lại")]
        public int? RemainingQuantity { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public int? Status { get; set; }

        [NotMapped]
        [Display(Name = "Mã mặt hàng")]
        public string? ProductData { get; set; }

        [NotMapped]
        [Display(Name = "Tên nhân viên")]
        public string? EmployeeName { get; set; }
    }
}
