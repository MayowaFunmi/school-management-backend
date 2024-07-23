using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using WatchDog;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllOrganizations
  {
    public record GetAllOrganizationsQuery : IRequest<GenericResponse>;

    public class GetAllOrganizationsHandler(IOrganizationService organizationService, ILoggerManager logger) : IRequestHandler<GetAllOrganizationsQuery, GenericResponse>
    {
      private readonly IOrganizationService _organizationService = organizationService;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetAllOrganizationsQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var organizations = await _organizationService.AllOrganizations();
          if (organizations.Count == 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.NotFound.ToString(),
              Message = "No organizations found",
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"{organizations.Count} Organizations retrieved successfully",
            Data = organizations
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting all organizations - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting all organizations - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting all organizations - {ex.Message}",
          };
        }
      }
    }
  }
}