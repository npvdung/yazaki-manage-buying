using System.ComponentModel.DataAnnotations.Schema;
namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Return_Product_Details
    {
        [Key]
        public Guid ID { get; set; }
        
        [Required]
        [Display(Name = "Mã xuất chi tiết")]
        public string ExportProductId { get; set; }

        [Required]
        [Display(Name = "Sản phẩm")]
        public string ProductId { get; set; }

        [Required]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Ngày trả")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Đơn vị tính")]
        public int Unit { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Đơn giá")]
        public decimal Price { get; set; }
    }
}
