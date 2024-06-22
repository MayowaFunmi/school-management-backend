using Microsoft.AspNetCore.Identity;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.DTOs
{
  public class UserWithRoles
  {
    public ApplicationUser User { get; set; } = null!;
    public IList<string> UserRoles { get; set; } = [];
    public TeachingStaff? TeacherProfile { get; set; }
    public NonTeachingStaff? NonTeacherProfile { get; set; }
    public TeachingStaff? AdminTeacher { get; set; }
    public NonTeachingStaff? AdminNonTeacher { get; set; }
    public Parent? ParentProfile { get; set; }
    public Student? StudentProfile { get; set; }
  }
}