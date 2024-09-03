using SchoolManagementApi.Commands.Students;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;
using static SchoolManagementApi.Queries.Students.GetStudentAttendance;

namespace SchoolManagementApi.Interfaces.Admin
{
  public interface IStudentClassServices
  {
    Task<StudentClass?> AddStudentClass(StudentClass studentClass, string adminId);
    Task<bool> GenerateClassArms(StudentClass studentClass);
    Task<List<StudentClass>> GetAllClasses(string schoolId);
    Task<List<ClassArms>> GetAllClassArms(string schoolId, string classId);
    Task<List<Student>> StudentsByClassArm(string studentClassId, int page, int pageSize);
    Task<int> StudentsByClassArmCount(string StudentClassId);
    Task<List<Student>> StudentsByClass(string classId);
    Task<StudentsCARecord> AddStudentsCATest(AddStudentsCA.AddStudentsCACommand request);
    Task<List<ClassStudentsScores>> GetClassStudentsScores(string sessionId, string classId, string subjectId, string term);
    Task<StudentSubjectSCores> GetStudentSubjectSCores(string studentId);
    Task<ClassAttendance> MarkStudentsAttendance(ClassAttendance classAttendance);
    Task<object> GetStudentsAttendance(string classArmId, DateTime date, DateTime endDate, string status);
    Task<int> GetClassDailyAttendanceCount(string classArmId, DateTime date);
    Task<int> GetClassWeeklyAttendanceCount(string classArmId, DateTime startDate);
    Task<int> GetStudentDailyAttendanceCount(string classArmId, string studentId, DateTime date);
    Task<int> GetStudentWeeklyAttendanceCount(string classArmId, string studentId, DateTime startDate);
    Task<List<StudentAttendanceCount>> StudentsClassWeeklyAttendanceCounts(string classArmId, DateTime satrtDate);
  }
}