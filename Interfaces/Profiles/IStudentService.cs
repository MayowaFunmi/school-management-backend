using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Intefaces.Profiles
{
  public interface IStudentService
  {
    Task<string> OrganizationExists(string organizationUniqueId);
    Task<Student> AddStudentProfile(Student student);
    Task<bool> StudentProfileExists(string studentId);
    Task<Student> GetStudentById(string studentId);
    Task<Student> GetStudentByUbuiqueId(string studentUniqueId);
    Task<List<Student>> GetStudentsInSchool(string schoolId);
  }
}