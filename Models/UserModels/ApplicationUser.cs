using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SchoolManagementApi.Models.UserModels
{
  public class ApplicationUser : IdentityUser
  {
    public string OrganizationId { get; set; } = string.Empty;
    public string UniqueId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int PercentageCompleted { get; set; }
    public bool IsActive { get; set; } = true;
    public int LoginCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLogin { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}