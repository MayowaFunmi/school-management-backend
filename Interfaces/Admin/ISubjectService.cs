using SchoolManagementApi.Models;

namespace SchoolManagementApi.Intefaces.Admin
{
  public interface ISubjectService
  {
    Task<Subject> AddSubject(string subjectName);
  }
}