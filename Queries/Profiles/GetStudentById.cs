using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Interfaces.Profiles;
using WatchDog;

namespace SchoolManagementApi.Queries.Profiles
{
  public class GetStudentById
  {
    public record GetStudentByIdQuery(string studentId) : IRequest<GenericResponse>;

    public class GetStudentByIdHandler(IStudentService studentService, ILoggerManager logger) : IRequestHandler<GetStudentByIdQuery, GenericResponse>
    {
      private readonly IStudentService _studentService = studentService;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var student = await _studentService.GetStudentById(request.studentId);
          if (student != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "student retrieved successfully",
              Data = student
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"Parent not found",
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting student by id - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting student by id - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting student by id - {ex.Message}",
          };
        }
      }
    }
  }
}