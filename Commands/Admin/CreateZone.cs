using System.Net;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagementApi.Constants;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Commands.Admin
{
  public class CreateZone
  {
    public class CreateZoneCommand : IRequest<GenericResponse>
    {
      public string AdminId { get; set; } = string.Empty;
      public string OrganizationUniqueId { get; set; } = string.Empty;
      public string Name { get; set; } = string.Empty;
      public string State { get; set; } = string.Empty;
      public List<string> LocalGovtAreas { get; set; } = [];
    }

    public class CreateZoneHandler(IZoneService zoneService, IMemoryCache cache) : IRequestHandler<CreateZoneCommand, GenericResponse>
    {
      private readonly IZoneService _zoneService = zoneService;
      private readonly IMemoryCache _cache = cache;
      public async Task<GenericResponse> Handle(CreateZoneCommand request, CancellationToken cancellationToken)
      {
        var organizationId = await _zoneService.OrganizationExists(request.OrganizationUniqueId!, request.AdminId!);
        if (string.IsNullOrEmpty(organizationId))
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"Organization with unique id {request.OrganizationUniqueId} cannot be found",
          };
        }

        var zone = new Models.Zone
        {
          OrganizationId = Guid.Parse(organizationId),
          Name = request.Name,
          AdminId = request.AdminId,
          State = request.State,
          LocalGovtAreas = request.LocalGovtAreas
        };
        var response = await _zoneService.CreateZone(zone);
        if (response)
        {
          _cache.Remove(CacheConstants.ZONES);          
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "Zone added sucessfully",
          };
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "Failed to add zone",
        };
      }
    }
  }
}