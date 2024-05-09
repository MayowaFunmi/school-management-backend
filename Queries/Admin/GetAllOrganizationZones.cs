using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllOrganizationZones
  {
    public class GetAllOrganizationZonesQuery : IRequest<GenericResponse>
    {
      public string AdminId { get; set; } = string.Empty;
      public string? OrganizationUniqueId { get; set; }
    }

    public class GetAllOrganizationZonesHandler(IZoneService zoneService) : IRequestHandler<GetAllOrganizationZonesQuery, GenericResponse>
    {
      private readonly IZoneService _zoneService = zoneService;

      public async Task<GenericResponse> Handle(GetAllOrganizationZonesQuery request, CancellationToken cancellationToken)
      {
        var organizationId = await _zoneService.OrganizationExists(request.OrganizationUniqueId!, request.AdminId);
        if (string.IsNullOrEmpty(organizationId))
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"Organization with unique id {request.OrganizationUniqueId} cannot be null",
          };
        }

        var response = await _zoneService.AllOrganizationZones(organizationId);
        if (response.Count == 0)
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
          Message = $"{response.Count} zones found for this organization",
        };
      }
    }
  }
}