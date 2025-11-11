
namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Import_Product
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public string ImportCode { get; set; }
        [Required]
        public string ImportName { get; set; }
        public DateTime? ImportDate { get; set; }
        public string UserId { get; set; }
        public string SupplierId { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
