using System.ComponentModel.DataAnnotations;

namespace StudentAdminPortal.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập Tên đăng nhập / MSSV")]
    [Display(Name = "Tên đăng nhập / MSSV")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = true;

    public string? ReturnUrl { get; set; }
}

public class StudentProfileViewModel
{
    public int Id { get; set; }

    [Display(Name = "Mã số sinh viên (MSSV)")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Giới tính")]
    public string? Gender { get; set; }

    [Display(Name = "Lớp sinh hoạt")]
    public string? ClassName { get; set; }

    [Display(Name = "Khoa / Viện")]
    public string? Faculty { get; set; }

    [Display(Name = "Số CCCD / CMND")]
    public string? CccdNumber { get; set; }

    [Display(Name = "Địa chỉ thường trú")]
    public string? Address { get; set; }

    [Display(Name = "Mật khẩu hiện tại")]
    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [Display(Name = "Mật khẩu mới")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    public string? NewPassword { get; set; }

    [Display(Name = "Xác nhận mật khẩu mới")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string? ConfirmPassword { get; set; }
}
