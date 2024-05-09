using SchoolManagementApi.Commands.Students;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;

namespace SchoolManagementApi.Intefaces.Admin
{
  public interface IStudentClassServices
  {
    Task<StudentClass> AddStudentClass(StudentClass studentClass);
    Task<bool> GenerateClassArms(StudentClass studentClass);
    Task<List<StudentClass>> GetAllClasses(string schoolId);
    Task<List<ClassArms>> GetAllClassArms(string schoolId, string classId);
    Task<List<Student>> StudentsByClassArm(string studentClassId, int page, int pageSize);
    Task<int> StudentsByClassArmCount(string StudentClassId);
    Task<List<Student>> StudentsByClass(string classId);
    Task<StudentsCARecord> AddStudentsCATest(AddStudentsCA.AddStudentsCACommand request);
    Task<List<ClassStudentsScores>> GetClassStudentsScores(string sessionId, string classId, string subjectId, string term);
    Task<StudentSubjectSCores> GetStudentSubjectSCores(string studentId);
  }
}