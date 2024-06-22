using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.Models.DocumentModels;

namespace SchoolManagementApi.Models.UserModels
{
  public class Student
  {
    [Key]
    public Guid StudentId { get; set; }
    [ForeignKey("UserId")]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
    public string OrganizationUniqueId { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string AdmissionNumber { get; set; } = string.Empty;
    public string AdmissionYear { get; set; } = string.Empty;
    [ForeignKey("SchoolZoneId")]
    public Guid SchoolZoneId { get; set; }
    public virtual Zone SchoolZone { get; set; } = null!;
    [ForeignKey("CurrentSchoolId")]
    public Guid CurrentSchoolId { get; set; }
    public virtual School CurrentSchool { get; set; } = null!;

    [ForeignKey("DepartmentId")]
    public Guid DepartmentId { get; set; }
    public virtual Department Department { get; set; } = null!;

    [ForeignKey("StudentClassId")]
    public Guid StudentClassId { get; set; }
    public virtual ClassArms StudentClass { get; set; } = null!;

    public virtual List<string> PreviousSchoolsIds { get; set; } = [];
    public string ProfilePicture { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Religion { get; set; } = string.Empty;
    public virtual IList<DocumentFile> Documents { get; set;} = [];

    [ForeignKey("ParentId")]
    public Guid ParentId { get; set; }
    public virtual Parent Parent { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
  }
}