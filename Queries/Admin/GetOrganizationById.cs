using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using WatchDog;
using static SchoolManagementApi.Queries.Admin.GetOrganizationsByAdminId;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetOrganizationById
  {
    public record GetOrganizationByIdQuery(string OrganizationId) : IRequest<GenericResponse>;

    public class GetOrganizationByIdHandler(IOrganizationService organizationService, ILoggerManager logger) : IRequestHandler<GetOrganizationByIdQuery, GenericResponse>
    {
      private readonly IOrganizationService _organizationService = organizationService;
      private readonly ILoggerManager _logger = logger;
      public async Task<GenericResponse> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var org = await _organizationService.GetOrganizationById(request.OrganizationId);
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