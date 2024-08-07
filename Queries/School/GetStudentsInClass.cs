using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using WatchDog;

namespace SchoolManagementApi.Queries.School
{
  public class GetStudentsInClass
  {
    public record GetStudentsInClassQuery(string studentClassId) : IRequest<GenericResponse>;

    public class GetStudentsInClassHandler(IStudentClassServices studentClassServices, ILoggerManager logger) : IRequestHandler<GetStudentsInClassQuery, GenericResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetStudentsInClassQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var students = await _studentClassServices.StudentsByClass(request.studentClassId);
          if (students.Count != 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"{students.Count} students in class retrieved successfully",
              Data = students
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "no students found in this class",
            Data = students
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting students in the same class - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting students in the same class - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting students- {ex.Message}",
          };
        }
      }
    }
  }
}