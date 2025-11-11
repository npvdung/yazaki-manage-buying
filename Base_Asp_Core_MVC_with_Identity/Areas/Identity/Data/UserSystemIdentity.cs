using Microsoft.AspNetCore.Identity;

namespace Base_Asp_Core_MVC_with_Identity.Areas.Identity.Data
{
    public class UserSystemIdentity : IdentityUser
    {
        [Display(Name = "Tên")]
        public string? FirstName { get; set; }

        [Display(Name = "Họ")]
        public string? LastName { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public byte[]? ProfilePicture { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Mã nhân viên")]
        public string? Code { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Phòng ban")]
        public string? Department { get; set; }

        [Display(Name = "Trạng thái")]
        public int? Status { get; set; }
    }
}
