using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.LoggerManager;
using SchoolManagementApi.Intefaces.Profiles;
using WatchDog;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllZonesByUniqueId
  {
    public record GetAllZonesByUniqueIdCommand(string OrganizationUniqueId) : IRequest<GenericResponse>;

    public class GetAllZonesByUniqueIdHandler(ITeachingStaffInterface teachingStaffInterface, ILoggerManager logger) : IRequestHandler<GetAllZonesByUniqueIdCommand, GenericResponse>
    {
      private readonly ITeachingStaffInterface _teachingStaffInterface = teachingStaffInterface;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetAllZonesByUniqueIdCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var zones = await _teachingStaffInterface.AllOrganizationZones(request.OrganizationUniqueId);
          if (zones.Count == 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "No zone found for this organization",
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"{zones.Count} zones retrieved successfully",
            Data = zones
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting organizations zones - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting organization zones - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting organization zones- {ex.Message}",
          };
        }
      }
    }
  }
}