using MessagePack;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class PurchaseOrder
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Mã đặt hàng")]
        public string PurchaseOrderCode { get; set; }
        [Required]
        [Display(Name = "Mã hợp đồng")]
        public string PurchaseContractId { get; set; }
        [Required]
        [Display(Name = "Nhân viên")]
        public string? EmployeeId { get; set; }
        [Display(Name = "Mô tả")]
        public string? Description { get; set;}
        [Required]
        public int Status { get; set; }
    }
}
