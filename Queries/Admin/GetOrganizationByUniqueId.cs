using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using WatchDog;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetOrganizationByUniqueId
  {
    public record GetOrganizationByUniqueIdQuery(string OrganizationId) : IRequest<GenericResponse>;

    public class GetOrganizationByUniqueIdHandler(IOrganizationService organizationService, ILoggerManager logger) : IRequestHandler<GetOrganizationByUniqueIdQuery, GenericResponse>
    {
      private readonly IOrganizationService _organizationService = organizationService;
      private readonly ILoggerManager _logger = logger;
      public async Task<GenericResponse> Handle(GetOrganizationByUniqueIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var org = await _organizationService.GetOrganizationByUniqueId(request.OrganizationId);
          if (org == null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.NotFound.ToString(),
              Message = "No organization found",
            };
          }

          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "organization retrieved successfully",
            Data = org
          };
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting organizations by unique id - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting organizations by unique id - {ex.Message}");
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"Error getting organizations by unique id - {ex.Message}",
          };
        }
      }
    }
  }
}