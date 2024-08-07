using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetOrganizationUsersByRole
  {
    public class GetOrganizationUsersByRoleQuery : IRequest<PageDataResponse>
    {
      public string OrganizationId { get; set; } = string.Empty;
      public string RoleName { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetOrganizationUsersByRoleHandler(IAdminService adminService) : IRequestHandler<GetOrganizationUsersByRoleQuery, PageDataResponse>
    {
      private readonly IAdminService _adminService = adminService;

      public async Task<PageDataResponse> Handle(GetOrganizationUsersByRoleQuery request, CancellationToken cancellationToken)
      {

        try
        {
          var roleUsers = await _adminService.OrganizationUsersByRole(request.OrganizationId, request.RoleName, request.Page, request.PageSize);
          var paginationMetaData = new PaginationMetaData(request.Page, request.PageSize, roleUsers.UserCount);

          if (roleUsers.Users.Count != 0)
          {
            return new PageDataResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"{roleUsers.UserCount} user {request.RoleName} retrieved for organization",
              Data = roleUsers.Users,
              Pagination = paginationMetaData
            };  
          }
          return new PageDataResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"No users found for this organization",
          };
        }
        catch (Exception ex)
        {
          return new PageDataResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"An internal server error occurred - {ex.Message}"
          };
        }
      }
    }
  }
}