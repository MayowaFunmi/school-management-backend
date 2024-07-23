using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Interfaces.Profiles;
using WatchDog;

namespace SchoolManagementApi.Queries.Profiles
{
  public class GetNonTeachingStaffById
  {
    public record GetNonTeachingStaffByIdQuery(string StaffId) : IRequest<GenericResponse>;
    
    public class GetNonTeachingStaffByIdHandler(INonTeachingStaffInterface nonTeachingStaffInterface, ILoggerManager logger) : IRequestHandler<GetNonTeachingStaffByIdQuery, GenericResponse>
    {
      private readonly INonTeachingStaffInterface _nonTeachingStaffInterface = nonTeachingStaffInterface;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetNonTeachingStaffByIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          if (string.IsNullOrEmpty(request.StaffId))
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.Unauthorized.ToString(),
              Message = "You are not authorized",
            };
          }
          var staff = await _nonTeachingStaffInterface.GetStaffById(request.StaffId);
          if (staff != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Staff retrieved successfully",
              Data = staff
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"Non teaching staff not found",
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting non teaching staff by id - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting non teaching staff by id - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting organizations for admin id - {ex.Message}",
          };
        }
      }
    }
  }
}