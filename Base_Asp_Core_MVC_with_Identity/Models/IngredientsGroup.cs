using MessagePack;

namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class IngredientsGroup
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public string IngredientsGroupCode { get; set; }
        [Required]
        public string IngredientsGroupName { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public string? Status { get; set; }
    }
}
