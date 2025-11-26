using System.ComponentModel.DataAnnotations.Schema;
namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Invoice_Details
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string InvoiceId { get; set; }
        [Required]
        public string ProductId { get; set; }
        [Required]
        public string ImportId { get; set; }
        [Required]
        public string Description { get; set; }
        public int Unit { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public decimal TotalAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserId { get; set; }
    }
}
