using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.LoggerManager;
using SchoolManagementApi.Intefaces.Profiles;
using WatchDog;

namespace SchoolManagementApi.Queries.Profiles
{
  public class GetTeachingStaffById
  {
    public record GetTeachingStaffByIdQuery(string StaffId) : IRequest<GenericResponse>;

    public class GetTeachingStaffByIdHandler(ITeachingStaffInterface teachingStaffInterface, ILoggerManager logger) : IRequestHandler<GetTeachingStaffByIdQuery, GenericResponse>
    {
      private readonly ITeachingStaffInterface _teachingStaffInterface = teachingStaffInterface;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetTeachingStaffByIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var teacher = await _teachingStaffInterface.GetTeacherById(request.StaffId);
          if (teacher != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Teacher retrieved successfully",
              Data = teacher
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"Teacher with Id {request.StaffId} not found",
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting teaching staff by id - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting teaching staff by id - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting teaching staff by id - {ex.Message}",
          };
        }
      }
    }
  }
}