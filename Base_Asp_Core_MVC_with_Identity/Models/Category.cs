

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Category
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Mã vật tư")]
        public string CategoryCode { get; set; }
        [Display(Name = "Tên loại vật tư")]
        [Required]
        public string CategoryName { get; set; }
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }
}
