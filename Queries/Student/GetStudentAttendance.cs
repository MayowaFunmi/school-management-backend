using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Intefaces.LoggerManager;
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
      public List<StudentDto> AbsentStudents { get; set; } = [];
      public List<StudentDto> HalfDayStudents { get; set; } = [];
    }

    public class GetStudentAttendanceQuery : IRequest<GenericResponse>
    {
      public required string ClassArmId { get; set; }
      public DateTime Date { get; set; }
    }

    public class GetStudentAttendanceHandler(IStudentClassServices studentClassServices, ILoggerManager logger) : IRequestHandler<GetStudentAttendanceQuery, GenericResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;
      private readonly ILoggerManager _logger = logger;
      public async Task<GenericResponse> Handle(GetStudentAttendanceQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var attendance = await _studentClassServices.GetStudentsAttendance(request.ClassArmId, request.Date);
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