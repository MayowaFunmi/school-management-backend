using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SchoolManagementApi.Models.UserModels
{
  public class ApplicationUser : IdentityUser
  {
    public string OrganizationId { get; set; } = string.Empty;
    public string UniqueId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int PercentageCompleted { get; set; }
    //public Organization Organization { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
  }
}