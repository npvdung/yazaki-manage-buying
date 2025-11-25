using System.ComponentModel.DataAnnotations;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Inventory
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Mã kho")]
        public string InventoryCode { get; set; }
        [Required]
        [Display(Name = "Tên kho")]
        public string Name { get; set;}
    }
}
