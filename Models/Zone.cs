using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Models
{
  public class Zone
  {
    [Key]
    public Guid ZoneId { get; set; }
    public string AdminId { get; set; } = string.Empty;

    [ForeignKey("OrganizationId")]
    [Required]
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<School> Schools { get; set; } = [];
    public string State { get; set; } = string.Empty;
    public List<string> LocalGovtAreas { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
  }
}