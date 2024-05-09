using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementApi.Models
{
  public class StudentClass
  {
    [Key]
    public Guid StudentClassId { get; set; }

    [ForeignKey("SchoolId")]
    public Guid SchoolId { get; set; }
    public string Name { get; set; } = string.Empty;  // ss1, ss2, etc
    [Range(1, int.MaxValue)]
    public int Arm { get; set; }
    public List<ClassArms>? ClassArms { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
  }
}