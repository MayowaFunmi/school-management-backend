using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using WatchDog;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetOrganizationZones
  {
    public record GetOrganizationZonesQuery(string OrganizationId) : IRequest<GenericResponse>;

    public class GetOrganizationZonesHandler(IZoneService zoneService, ILoggerManager logger) : IRequestHandler<GetOrganizationZonesQuery, GenericResponse>
    {
      private readonly IZoneService _zoneService = zoneService;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetOrganizationZonesQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var zones = await _zoneService.AllOrganizationZones(request.OrganizationId);  // this uses organization Id. not unique id
          if (zones.Count == 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.NotFound.ToString(),
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
          _logger.LogError($"Error getting organizations for admin id - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting organizations for admin id - {ex.Message}");
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