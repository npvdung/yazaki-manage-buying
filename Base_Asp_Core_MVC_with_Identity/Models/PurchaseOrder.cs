using MessagePack;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class PurchaseOrder
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public string PurchaseOrderCode { get; set; }
        [Required]
        public string PurchaseContractId { get; set; }
        [Required]
        public string? EmployeeId { get; set; }
        public string? Description { get; set;}
        [Required]
        public int Status { get; set; }
    }
}
