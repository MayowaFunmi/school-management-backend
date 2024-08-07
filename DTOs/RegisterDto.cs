using System.ComponentModel.DataAnnotations;

namespace SchoolManagementApi.DTOs
{
  public class RegisterDto
  {
    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "FirstName is required")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "FirstName is required")]
    public string MiddleName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastName is required")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone Number is required")]
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    [Required(ErrorMessage = "OrganizationUniqueId is required")]
    public string OrganizationUniqueId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
  }
}