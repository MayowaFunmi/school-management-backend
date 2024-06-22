using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.DocumentModels;
using SchoolManagementApi.Models.UserModels;
namespace SchoolManagementApi.DTOs
{
  public class StaffProfile
  {
    [Key]
    public Guid StaffProfileId { get; set; }
    [ForeignKey("UserId")]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
    public string OrganizationUniqueId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string StateOfOrigin { get; set; } = string.Empty;
    public string LgaOfOrigin { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Religion { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string AboutMe { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public int Step { get; set; }
    public DateTime FirstAppointment { get; set; }
    public int YearsInService { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Discipline { get; set; } = string.Empty;

    [ForeignKey("CurrentPostingZoneId")]
    public Guid CurrentPostingZoneId { get; set; }
    public virtual Zone CurrentPostingZone { get; set; } = null!;

    [ForeignKey("CurrentPostingSchoolId")]
    public Guid CurrentPostingSchoolId { get; set; }
    public virtual School CurrentPostingSchool { get; set; } = null!;
    public virtual List<string> PreviousSchoolsIds { get; set; } = [];
    public virtual IList<DocumentFile> Documents { get; set;} = [];
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; }
  }
}