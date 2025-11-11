using MessagePack;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Manufacture
    {
        [Key]
        public Guid ID { get; set; }
        [Display(Name = "Mã nhà sản xuất")]
        [Required]
        public string ManufactureCode { get; set; }
        [Display(Name = "Tên nhà sản xuất")]
        [Required]
        public string ManufactureName { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required]
        public string? Address { get; set; }
        [Display(Name = "SDT")]
        [Required]
        public string? PhoneNumber { get; set; }
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}
