using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Models
{
  public class Organization
  {
    [Key]
    public Guid OrganizationId { get; set; }
    public string OrganizationUniqueId { get; set; } = string.Empty;
    [ForeignKey("AdminId")]
    [Required]
    public string AdminId { get; set; } = string.Empty;
    public ApplicationUser? Admin { get; set; }
    public List<Zone>? Zones { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
  } 
}