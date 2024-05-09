using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.DTOs;

namespace SchoolManagementApi.Models.UserModels
{
  public class TeachingStaff : StaffProfile
  {
    public string? PublishedWork { get; set; }

    [ForeignKey("CurrentSubjectId")]
    public Guid CurrentSubjectId { get; set; }
    public virtual Subject? CurrentSubject { get; set; }
    public List<string>? OtherSubjects { get; set; }
  }
}