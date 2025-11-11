using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Product
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Mã hàng hoá")]
        public string ProductCode { get; set; }
        [Required]
        [Display(Name = "Tên mặt hàng")]
        public string ProductName { get; set; }
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
        //[Required]
        [Display(Name = "Loại vật tư")]
        public string? QuotaId { get; set; }
        [Display(Name = "Đơn vị đo lường")]
        public string? Units { get; set; }
        [Required]
        [Display(Name = "Tên nhà sản xuất")]
        public string? ManufacturerId { get; set; }
        [Display(Name = "Tên nhà cung cấp")]
        public string? SupplierId { get; set; }
        [Required]
        [Display(Name = "Tên nhà cung cấp")]
        public string? VenderId { get; set; }
        [NotMapped]
        [Display(Name = "Loại tiền thanh toán")]
        public string CURRENCYNAME { get; set; }
        [NotMapped]
        [Display(Name = "Mã nhà sản xuất")]
        public string MANUFACTURER_Data { get; set;}

    }
}
