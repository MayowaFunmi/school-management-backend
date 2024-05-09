using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetOrganizationUsersByRole
  {
    public class GetOrganizationUsersByRoleQuery : IRequest<GenericResponse>
    {
      public string OrganizationId { get; set; } = string.Empty;
      public string RoleName { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetOrganizationUsersByRoleHandler(IAdminService adminService) : IRequestHandler<GetOrganizationUsersByRoleQuery, GenericResponse>
    {
      private readonly IAdminService _adminService = adminService;

      public async Task<GenericResponse> Handle(GetOrganizationUsersByRoleQuery request, CancellationToken cancellationToken)
      {
        var roleUsers = await _adminService.OrganizationUsersByRole(request.OrganizationId, request.RoleName, request.Page, request.PageSize);
        int totalUserCount = roleUsers.UserCount;
        int totalPages = 1;
        if (request.Page != 0 || request.PageSize != 0)
          totalPages = (int)Math.Ceiling((double)totalUserCount / request.PageSize);
        
        request.Page = Math.Min(Math.Max(request.Page, 1), totalPages);

        try
        {
          if (roleUsers.Users.Count != 0)
          {
            var response = new UserpageResponse
            {
              Users = roleUsers.Users,
              TotalPages = totalPages,
              CurrentPage = request.Page,
              PagesLeft = totalPages - request.Page,
            };
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"{totalUserCount} user {request.RoleName} retrieved for organization",
              Data = response
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"No user found for this organization",
            Data = null
          };
        }
        catch (Exception ex)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"An internal server error occurred - {ex.Message}"
          };
        }
      }
    }
  }
}