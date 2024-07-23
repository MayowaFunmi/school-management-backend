using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using WatchDog;

namespace SchoolManagementApi.Queries.Students
{
  public class GetStudentAttendance
  {
    public class StudentDto
    {
      public required string StudentId { get; set; }
      public required string StudentUniqueId { get; set; }
      public required string LastName { get; set; }
      public required string FirstName { get; set; }
      public required string MiddleName { get; set; }
    }
    public class AttendanceStatus
    {
      public List<StudentDto> PresentStudents { get; set; } = [];
      public int PresentStudentsCount { get; set; }
      public List<StudentDto> AbsentStudents { get; set; } = [];
      public int AbsentStudentsCount { get; set; }
      public List<StudentDto> HalfDayStudents { get; set; } = [];
      public int HalfDayStudentsCount { get; set; }
      public int TotalStudentsCount => PresentStudentsCount + AbsentStudentsCount + HalfDayStudentsCount;
      public string StudentName { get; set; } = string.Empty;
      public string StudentUniqueId { get; set; } = string.Empty;
      public int TimesPresent { get; set; }
      public int TimesAbsent { get; set; }
      public int TimesHalfDay { get; set; }

    }

    public class GetStudentAttendanceQuery : IRequest<GenericResponse>
    {
      public string ClassArmId { get; set; } = string.Empty;
      public DateTime Date { get; set; }
      public DateTime EndDate { get; set; }
      public string Status { get; set; } = "daily";
    }

    public class GetStudentAttendanceHandler(IStudentClassServices studentClassServices, ILoggerManager logger) : IRequestHandler<GetStudentAttendanceQuery, GenericResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;
      private readonly ILoggerManager _logger = logger;
      public async Task<GenericResponse> Handle(GetStudentAttendanceQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var attendance = await _studentClassServices.GetStudentsAttendance(request.ClassArmId, request.Date, request.EndDate, request.Status);
          if (attendance != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"Students' attendance records for {request.Date.Date} retrieved successfully",
              Data = attendance
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"Students' attendance records for {request.Date.Date} not found",
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting student attendance - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting student attendance - {ex.Message}");
          throw;
        }
      }
    }
  }
}