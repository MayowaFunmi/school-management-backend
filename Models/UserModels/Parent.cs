using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SchoolManagementApi.Constants.RoleCionstants;

namespace SchoolManagementApi.Models.UserModels
{
  public class Parent
  {
    [Key]
    public Guid ParentId { get; set; }
    [ForeignKey("UserId")]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
    [ForeignKey("StudentSchoolId")]
    public Guid StudentSchoolId { get; set; }
    public virtual School StudentSchool { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string RelationshipType { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Religion { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string StateOfOrigin { get; set; } = string.Empty;
    public string LgaOfOrigin { get; set; } = string.Empty;
    public string LgaOfResidence { get; set; } = string.Empty;
    public string Occupation { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
  }
}