using System.ComponentModel.DataAnnotations;

namespace SchoolManagementApi.DTOs
{
    public class RegisterDto
  {
    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "FirstName is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone Number is required")]
    public string PhoneNumber { get; set; }
    public string Role { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
  }
}