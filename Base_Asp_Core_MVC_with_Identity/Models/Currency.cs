using MessagePack;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Currency
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Mã tiền tệ")]
        public string CurrencyCode { get; set; }
        [Required]
        [Display(Name = "Tên loại tiền")]
        public string CurrencyName { get; set; }
        [Required]
        [Display(Name = "Tỉ giá VND")]
        public double? ExchangeRate { get; set; }
        [Required]
        [Display(Name = "Ký hiệu")]
        public string? Symbol { get; set;}
    }
}
