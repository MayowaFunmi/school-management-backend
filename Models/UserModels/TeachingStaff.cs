using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApi.DTOs;

namespace SchoolManagementApi.Models.UserModels
{
  public class TeachingStaff : StaffProfile
  {
    public string PublishedWork { get; set; } = string.Empty;

    [ForeignKey("CurrentSubjectId")]
    public Guid CurrentSubjectId { get; set; }
    public virtual Subject CurrentSubject { get; set; } = null!;
    public List<string> OtherSubjects { get; set; } = [];
  }
}