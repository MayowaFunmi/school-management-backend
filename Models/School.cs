using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Models
{
  public class School
  {
    [Key]
    public Guid SchoolId { get; set; }
    [ForeignKey("AdminId")]
    public string AdminId { get; set; } = string.Empty;
    public ApplicationUser Admin { get; set; } = null!;
    public string OrganizationUniqueId { get; set; } = string.Empty;
    public string SchoolUniqueId { get; set; } = string.Empty;
    [ForeignKey("ZoneId")]
    [Required]
    public Guid ZoneId { get; set; }
    [JsonIgnore]
    public virtual Zone? Zone { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string LocalGovtArea { get; set; } = string.Empty;
    public List<Department> Departments { get; set; } = [];
    public List<StudentClass> StudentClasses { get; set; } = [];
    public List<Subject> Subjects { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
  }
}