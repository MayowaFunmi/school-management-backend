using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Intefaces.LoggerManager;
using WatchDog;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetOrganizationsByAdminId
  {
    public record GetOrganizationByAdminIdQuery(string AdminId) : IRequest<GenericResponse>;

    public class GetOrganizationsByAdminIdHandler(IOrganizationService organizationService, ILoggerManager logger) : IRequestHandler<GetOrganizationByAdminIdQuery, GenericResponse>
    {
      private readonly IOrganizationService _organizationService = organizationService;
      private readonly ILoggerManager _logger = logger;

      public async Task<GenericResponse> Handle(GetOrganizationByAdminIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var organizations = await _organizationService.RetrieveAdminOrganizations(request.AdminId);
          if (organizations.Count == 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.NotFound.ToString(),
              Message = "No organization found for this user",
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"{organizations.Count} Admin Organizations retrieved successfully",
            Data = organizations
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