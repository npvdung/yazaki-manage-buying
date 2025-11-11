namespace Base_Asp_Core_MVC_with_Identity.Models
{
    public class Return_Product
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [Display(Name = "Mã phiếu xuất")]
        public string ExportId { get; set; }

        [Required]
        [Display(Name = "Ngày xuất")]
        public DateTime? ExportDate { get; set; }

        [Display(Name = "Người thực hiện")]
        public string UserId { get; set; }

        [Display(Name = "Nhà cung cấp")]
        public string SupplierId { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Ngày trả lại")]
        public DateTime? ReturnDate { get; set; }

    }
}
