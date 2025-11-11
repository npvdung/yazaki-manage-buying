using System.ComponentModel.DataAnnotations;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class BillPayment
    {
        [Key]
        [Display(Name = "Mã định danh")]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "Mã phiếu thanh toán là bắt buộc.")]
        [Display(Name = "Mã phiếu thanh toán")]
        public string BillPaymentCode { get; set; }

        [Required(ErrorMessage = "Phiếu nhập kho là bắt buộc.")]
        [Display(Name = "Phiếu nhập kho")]
        public string ItemReceiptId { get; set; }

        [Required(ErrorMessage = "Nhân viên thanh toán là bắt buộc.")]
        [Display(Name = "Nhân viên thanh toán")]
        public string? EmployeeId { get; set; }

        [Required(ErrorMessage = "Ngày thanh toán là bắt buộc.")]
        [Display(Name = "Ngày thanh toán")]
        public DateTime? BillPaymentDate { get; set; }

        [Required(ErrorMessage = "Tổng tiền là bắt buộc.")]
        [Display(Name = "Tổng tiền")]
        public decimal? TotalAmount { get; set; }

        [Required(ErrorMessage = "Số tiền thanh toán là bắt buộc.")]
        [Display(Name = "Số tiền thanh toán")]
        public decimal? BillPaymentAmount { get; set; }

        [Required(ErrorMessage = "Điều khoản thanh toán là bắt buộc.")]
        [Display(Name = "Điều khoản thanh toán")]
        public string? PaymentTerm { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [Display(Name = "Trạng thái")]
        public int? Status { get; set; }
    }
}
