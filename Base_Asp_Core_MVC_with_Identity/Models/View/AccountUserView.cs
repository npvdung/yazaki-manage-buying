using System.Xml.Linq;

namespace Base_Asp_Core_MVC_with_Identity.Models.View
{
    public class AccountUserView
    {
        [Required]
        public Guid Id { get; set; }
        [Display(Name = "Tên Nhân viên")]
        [Required(ErrorMessage = "Tên Nhân viên là bắt buộc.")]
        public string FirstName { get; set; }
        [Display(Name = "Họ và đệm")]
        [Required(ErrorMessage = "Họ và đệm là bắt buộc.")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail không hợp lệ")]
        public string Email { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string Password { get; set; }
        [Display(Name = "SDT")]
        public string? PhoneNumber { get; set; }
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        [Display(Name = "Mã người dùng")]
        public string? Code { get; set; }
        [Display(Name = "Ngày sinh")]
        public DateTime? BirthDate { get; set; }
        [Display(Name = "Phòng ban")]
        public string? Department { get; set; }
    }
}
