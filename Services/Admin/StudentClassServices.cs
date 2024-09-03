using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Commands.Students;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;
using SchoolManagementApi.Queries.Students;
using WatchDog;

namespace SchoolManagementApi.Services.Admin
{
  public class StudentClassServices(ApplicationDbContext context, ILoggerManager logger) : IStudentClassServices
  {
    private readonly ApplicationDbContext _context = context;
    private readonly ILoggerManager _logger = logger;

    public async Task<StudentClass?> AddStudentClass(StudentClass studentClass, string adminId)
    {
      try
      {
        var checkAdmin = await _context.Schools
            .Where(s => s.SchoolId == studentClass.SchoolId && s.AdminId == adminId)
            .Select(s => s.SchoolId)
            .FirstOrDefaultAsync();

        if (checkAdmin == Guid.Empty)
            return null;

        var stdClassExists = await _context.StudentClasses
            .AsNoTracking()
            .AnyAsync(s => s.SchoolId == studentClass.SchoolId && s.Name == studentClass.Name);

        if (stdClassExists)
            return null;

        var response = _context.StudentClasses.Add(studentClass);
        await _context.SaveChangesAsync();
        return response.Entity;

      }
      catch (Exception ex)
      {
        _logger.LogError($"Error adding student classes - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error adding student classes - {ex.Message}");
        throw;
      }
    }

    public async Task<bool> GenerateClassArms(StudentClass studentClass)
    {
      try
      {
        var classArms = Enumerable.Range(65, studentClass.Arm).Select(i => $"{studentClass.Name}{(char)i}").ToList();
        foreach (var armName in classArms)
        {
          var departmentId = AddDepartment(armName, _context);
          _context.ClassArms.Add(new ClassArms
          {
            SchoolId = studentClass.SchoolId,
            StudentClassId = studentClass.StudentClassId,
            Name = armName,
            DepartmentId = departmentId ?? null
          });
          await _context.SaveChangesAsync();
        }
        var school = await _context.Schools.FirstOrDefaultAsync(s => s.SchoolId == studentClass.SchoolId);
        if (school != null && !school.StudentClasses.Any(s => s.StudentClassId == studentClass.StudentClassId))
        {
          school.StudentClasses.Add(studentClass);
          _context.Schools.Update(school);
          await _context.SaveChangesAsync();
        }
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error generating student class arms - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error generating student class arms - {ex.Message}");
        throw;
      }
    }

    public async Task<List<ClassArms>> GetAllClassArms(string schoolId, string classId)
    {
      try
      {
        var classArms = await _context.ClassArms
          .Where(c => c.SchoolId.ToString() == schoolId && c.StudentClassId.ToString() == classId)
          .Include(c => c.Department)
          .ToListAsync();
        return classArms;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting all school's class arms - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting all school's class arms - {ex.Message}");
        throw;
      }
    }

    public async Task<List<StudentClass>> GetAllClasses(string schoolId)
    {
      try
      {
        var studentClasses = await _context.StudentClasses
          .Where(c => c.SchoolId.ToString() == schoolId)
          .Include(c => c.ClassArms)
          .ToListAsync();
        return studentClasses;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting all school's classes - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting all school's classes - {ex.Message}");
        throw;
      }
    }

    public async Task<int> StudentsByClassArmCount(string studentClassId)
    {
      try
      {
        var studentCount = await _context.Students
          .Where(s => s.StudentClassId.ToString() == studentClassId)
          .CountAsync();
        return studentCount;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting number of students in the same class arm - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting number of students in the same class arm- {ex.Message}");
        throw;
      }
    }

    public async Task<List<Student>> StudentsByClassArm(string studentClassId, int page, int pageSize)
    {
      try
      {
        var students = await _context.Students
          .Where(s => s.StudentClassId.ToString() == studentClassId)
          .Include(s => s.User)
          .Include(s => s.SchoolZone)
          .Include(s => s.CurrentSchool)
          .Include(s => s.Department)
          .Include(s => s.Parent)
          .Include(s => s.Documents)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .OrderBy(s => s.User.LastName)
          .ToListAsync();
        return students;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting students in the same class arm - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting students in the same class arm- {ex.Message}");
        throw;
      }
    }

    public async Task<List<Student>> StudentsByClass(string classId)
    {
      try
      {
        var students = await _context.Students
          .Where(s => s.StudentClassId.ToString() == classId)
          .ToListAsync();
        return students;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting students in the same class - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting - {ex.Message}");
        throw;
      }
    }

    public async Task<StudentsCARecord> AddStudentsCATest(AddStudentsCA.AddStudentsCACommand request)
    {
      try
      {
        var studentsCATest = new StudentsCARecord
        {
          //SchoolId = request.SchoolId,
          ClassId = request.ClassId,
          SubjectId = request.SubjectId,
          SchoolSessionId = request.SessionId,
          Term = request.Term,
        };

        _context.StudentsCARecords.Add(studentsCATest);
        await _context.SaveChangesAsync();

        List<StudentsScores> scoresList = [];
        
        foreach (var score in request.StudentsScores)
        {
          var studentScore = new StudentsScores
          {
            StudentsCARecordId = studentsCATest.TestId.ToString(),
            StudentId = score.StudentId,
            CATest1 = score.CATest1,
            CATest2 = score.CATest2,
            CATest3 = score.CATest3,
            Exam = score.Exam
          };
          scoresList.Add(studentScore);
        }

        await _context.AddRangeAsync(scoresList);
        await _context.SaveChangesAsync();
        return studentsCATest;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error adding students' score - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error adding students' score - {ex.Message}");
        throw;
      }
    }

    public async Task<List<ClassStudentsScores>> GetClassStudentsScores(string sessionId, string classId, string subjectId, string term)
    {
      try
      {
        var records = await _context.StudentsCARecords
          .Where(s => s.ClassId == classId && s.SubjectId == subjectId && s.SchoolSessionId == sessionId && s.Term == term)
          .Include(s => s.SchoolSession)
          .Include(s => s.StudentsScores)
          .ThenInclude(ss => ss.Student)
          .ToListAsync();

        var classstudentsScores = records
          .SelectMany(record => record.StudentsScores.Select(score => new ClassStudentsScores
          {
            StudentId = score.StudentId,
            StudentData = new StudentData
            {
              UniqueId = score.Student!.User!.UniqueId,
              LastName = score.Student.User.LastName,
              FirstName = score.Student.User.FirstName,
              MiddleName = score.Student.MiddleName,
              AdmissionNumber = score.Student.AdmissionNumber,
            },
            CATest1 = score.CATest1,
            CATest2 = score.CATest2,
            CATest3 = score.CATest3,
            Exam = score.Exam
          }))
          .ToList();
        return classstudentsScores;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting class students' score - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting class students' score - {ex.Message}");
        throw;
      }
    }

    public Task<StudentSubjectSCores> GetStudentSubjectSCores(string studentId)
    {
        throw new NotImplementedException();
    }

    public async Task<ClassAttendance> MarkStudentsAttendance(ClassAttendance classAttendance)
    {
      try
      {
        var attendance = _context.ClassAttendances.Add(classAttendance);
        await _context.SaveChangesAsync();
        return attendance.Entity;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error marking class attendance - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error marking class attendance- {ex.Message}");
        throw;
      }
    }

    public async Task<object> GetStudentsAttendance(string classArmId, DateTime date, DateTime endDate, string status)
    {
      try
      {
        List<ClassAttendance> attendanceRecords = [];
        if (status == "daily")
        {
          attendanceRecords = await _context.ClassAttendances
                                    .Include(c => c.Student)
                                      .ThenInclude(s => s.User)
                                    .Where(a => a.ClassArmId == classArmId && a.DayAndTime.Date == date.Date)
                                    .AsNoTracking()
                                    .ToListAsync();

          var presentStudents = attendanceRecords
                                    .Where(a => a.MorningAttendance == "present" && a.AfternoonAttendance == "present")
                                    .Select(a => new GetStudentAttendance.StudentDto
                                    {
                                      StudentId = a.StudentId,
                                      StudentUniqueId = a.Student.User.UniqueId,
                                      LastName = a.Student.User.LastName,
                                      FirstName = a.Student.User.FirstName,
                                      MiddleName = a.Student.MiddleName
                                    }).ToList();

          var absentStudents = attendanceRecords
                                  .Where(a => a.MorningAttendance == "absent" && a.AfternoonAttendance == "absent")
                                  .Select(a => new GetStudentAttendance.StudentDto
                                  {
                                    StudentId = a.StudentId,
                                    StudentUniqueId = a.Student.User.UniqueId,
                                    LastName = a.Student.User.LastName,
                                    FirstName = a.Student.User.FirstName,
                                    MiddleName = a.Student.MiddleName
                                  }).ToList();

          var halfDayStudents = attendanceRecords
                                .Where(a => 
                                  (a.MorningAttendance == "present" && a.AfternoonAttendance == "absent") || 
                                  (a.MorningAttendance == "absent" && a.AfternoonAttendance == "present"))
                                .Select(a => new GetStudentAttendance.StudentDto
                                {
                                  StudentId = a.StudentId,
                                  StudentUniqueId = a.Student.User.UniqueId,
                                  LastName = a.Student.User.LastName,
                                  FirstName = a.Student.User.FirstName,
                                  MiddleName = a.Student.MiddleName
                                }).ToList();

          return new GetStudentAttendance.AttendanceStatus
          {
            PresentStudents = presentStudents,
            PresentStudentsCount = presentStudents.Count,
            AbsentStudents = absentStudents,
            AbsentStudentsCount = absentStudents.Count,
            HalfDayStudents = halfDayStudents,
            HalfDayStudentsCount = halfDayStudents.Count
          };
        }
        else
        {
          attendanceRecords = await _context.ClassAttendances
                                    .Include(c => c.Student)
                                      .ThenInclude(s => s.User)
                                    .Where(ca => ca.ClassArmId == classArmId && ca.DayAndTime.Date >= date.Date && ca.DayAndTime.Date < endDate.Date)
                                    .AsNoTracking()
                                    .ToListAsync();

          var groupedRecords = attendanceRecords
                                    .GroupBy(ca => new 
                                    {
                                        ca.StudentId,
                                        ca.Student.User.UniqueId,
                                        ca.Student.User.FirstName,
                                        ca.Student.User.LastName,
                                        ca.MorningAttendance,
                                        ca.AfternoonAttendance
                                    })
                                    .Select(g => new GetStudentAttendance.AttendanceStatus
                                    {
                                      StudentUniqueId = g.Key.UniqueId,
                                      StudentName = $"{g.Key.LastName} {g.Key.FirstName}",
                                      TimesPresent = g.Count(a => a.MorningAttendance == "present" && a.AfternoonAttendance == "present"),
                                      TimesAbsent = g.Count(a => a.MorningAttendance == "absent" && a.AfternoonAttendance == "absent"),
                                      TimesHalfDay = g.Count(a => a.MorningAttendance == "present" && a.AfternoonAttendance == "absent" || 
                                                                a.MorningAttendance == "absent" && a.AfternoonAttendance == "present")
                                    })
                                    .ToList();
          return groupedRecords;
        }

        // if (attendanceRecords.Count == 0 && attendanceRecords.Any(a => a.Student == null))
        // {
        //   return new GetStudentAttendance.AttendanceStatus
        //   {
        //     PresentStudents = [],
        //     AbsentStudents = [],
        //     HalfDayStudents = []
        //   };
        // }
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting class attendance - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting class attendance- {ex.Message}");
        throw;
      }
    }

    public async Task<int> GetClassDailyAttendanceCount(string classArmId, DateTime date)
    {
      try
      {
        return await _context.ClassAttendances
              .Where(ca => ca.ClassArmId == classArmId && ca.DayAndTime.Date == date.Date)
              .AsNoTracking()
              .CountAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting daily class attendance count - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting daily class attendance count- {ex.Message}");
        throw;
      }
    }

    public async Task<int> GetClassWeeklyAttendanceCount(string classArmId, DateTime startDate)
    {
      try
      {
        var endDate = startDate.AddDays(7);
        return await _context.ClassAttendances
            .Where(ca => ca.ClassArmId == classArmId && ca.DayAndTime.Date >= startDate.Date && ca.DayAndTime.Date < endDate.Date)
            .AsNoTracking()
            .CountAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting weekly class attendance count - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting weekly class attendance count- {ex.Message}");
        throw;
      }
    }

    public async Task<int> GetStudentDailyAttendanceCount(string classArmId, string studentId, DateTime date)
    {
      try
      {
        return await _context.ClassAttendances
            .Where(ca => ca.ClassArmId == classArmId && ca.StudentId == studentId && ca.DayAndTime.Date == date.Date)
            .AsNoTracking()
            .CountAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting student daily class attendance count - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting student daily class attendance count- {ex.Message}");
        throw;
      }
    }

    public async Task<int> GetStudentWeeklyAttendanceCount(string classArmId, string studentId, DateTime startDate)
    {
      try
      {
        var endDate = startDate.AddDays(7);
        return await _context.ClassAttendances
            .Where(ca => ca.ClassArmId == classArmId && ca.StudentId == studentId && ca.DayAndTime.Date >= startDate.Date && ca.DayAndTime.Date < endDate.Date)
            .AsNoTracking()
            .CountAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting student weekly class attendance count - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting student weekly class attendance count- {ex.Message}");
        throw;
      }
    }

    public async Task<List<StudentAttendanceCount>> StudentsClassWeeklyAttendanceCounts(string classArmId, DateTime startDate)
    {
      try
      {
        var endDate = startDate.AddDays(7);
        var attendanceCounts = await _context.ClassAttendances
            .Where(ca => ca.ClassArmId == classArmId && ca.DayAndTime.Date >= startDate.Date && ca.DayAndTime.Date < endDate.Date)
            .Include(ca => ca.Student)
            .ThenInclude(student => student.User)
            .AsNoTracking()
            .GroupBy(ca => new 
            {
                ca.Student.User.UniqueId,
                ca.Student.User.FirstName,
                ca.Student.User.LastName,
                ca.StudentId
            })
            .Select(group => new StudentAttendanceCount
            {
                StudentUniqueId = group.Key.UniqueId,
                FirstName = group.Key.FirstName,
                LastName = group.Key.LastName,
                ClassArmId = classArmId,
                Count = group.Count()
            })
            .ToListAsync();

          return attendanceCounts;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting students weekly class attendance counts - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting students weekly class attendance counts- {ex.Message}");
        throw;
      }
    }

    private static Guid? AddDepartment(string armName, ApplicationDbContext dbContext)
    {
      var department = new Department();
      if (armName[0] == 'S')
      {
        var lastChar = char.ToUpper(armName[^1]);
        var departmentName = lastChar switch
        {
          'A' => "Science",
          'B' => "Arts",
          'C' => "Commercial",
          _ => null
        };
        department = dbContext.Departments.FirstOrDefault(d => d.Name == departmentName);
        return department?.DepartmentId;
      }
      else if (armName[0] == 'J')
      {
          // Retrieve the department from the database based on the name "Junior School"
          department = dbContext.Departments.FirstOrDefault(d => d.Name == "Junior School");
          return department?.DepartmentId;
      }
      
      // Return null if armName doesn't start with 'S' or 'J'
      return null;
    }
  }
}