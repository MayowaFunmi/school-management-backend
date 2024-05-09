using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementApi.Models
{
  public class ClassArms
  {
    [Key]
    public Guid ClassArmId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [ForeignKey("SchoolId")]
    [Required]
    public Guid SchoolId { get; set; }
    
    [ForeignKey("StudentClassId")]
    public Guid StudentClassId { get; set; }

    [ForeignKey("DepartmentId")]
    public Guid? DepartmentId { get; set; }

    public Department? Department { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
  }
}