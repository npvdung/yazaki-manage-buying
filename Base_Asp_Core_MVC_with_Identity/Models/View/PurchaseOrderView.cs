using Base_Asp_Core_MVC_with_Identity.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangagerBuyProduct.Models.View
{
    public class PurchaseOrderView
    {
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Mã đơn mua")]
        public string PurchaseOrderCode { get; set; }

        [Required]
        [Display(Name = "Hợp đồng mua")]
        public string PurchaseContractId { get; set; }

        [Required]
        [Display(Name = "Nhân viên lập")]
        public string? EmployeeId { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public int Status { get; set; }

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

        [Display(Name = "Chi tiết đơn mua")]
        public List<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
    }
}
