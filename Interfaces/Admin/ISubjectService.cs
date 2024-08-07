using SchoolManagementApi.Models;

namespace SchoolManagementApi.Interfaces.Admin
{
  public interface ISubjectService
  {
    Task<Subject> AddSubject(string subjectName);
  }
}