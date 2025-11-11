using System.ComponentModel.DataAnnotations;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Inventory
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public string InventoryCode { get; set; }
        [Required]
        public string Name { get; set;}
    }
}
