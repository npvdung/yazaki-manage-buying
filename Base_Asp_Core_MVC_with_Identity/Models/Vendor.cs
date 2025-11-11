using MessagePack;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Vendor
    {
        [Key]
        public Guid ID { get; set; }
        [Display(Name = "Mã nhà cung cấp")]
        [Required(ErrorMessage = "Vender Code is required")]
        public string VendorCode { get; set; }
        [Required]
        [Display(Name = "Tên nhà cung cấp")]
        //[Required(ErrorMessage = "Tên nhà cung cấp là bắt buộc.")]
        public string VendorName { get; set; }
        [Display(Name = "Vendor Name")]
        public string? Address { get; set; }
        [Display(Name = "Số điện thoại")]
        //[Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        public string? Phone { get; set; }
        [Display(Name = "Mô tả")]
        public string? Description { get; set;}
        public string? Email { get; set; }
        [Display(Name = "Tax")]

        public string? TaxNumber { get; set; }
        [Display(Name = "Hạn thanh toán")]
        public string? PaymentTerms { get; set; }
        [Display(Name = "Tài khoản ngân hàng")]
        public string? BankAccount { get; set; }
        [Display(Name = "Trạng thái")]
        [Required]
        public int? Status { get; set; }
        [Display(Name = "Loại tiền thanh toán")]
        [Required(ErrorMessage = "Currency is required")]
        public string? CurrencyId { get; set; }
    }
}
