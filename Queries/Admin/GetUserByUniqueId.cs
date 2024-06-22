using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetUserByUniqueId
  {
    public class GetUserByUniqueIdQuery : IRequest<GenericResponse>
    {
      public string? UniqueId { get; set; }
      public string? RoleName { get; set; }
    }

    public class GetUserByUniqueIdHandler(IAdminService adminService) : IRequestHandler<GetUserByUniqueIdQuery, GenericResponse>
    {
      private readonly IAdminService _adminService = adminService;

      public async Task<GenericResponse> Handle(GetUserByUniqueIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var userProfile = await _adminService.GetUserByUniqueId(request.UniqueId!, request.RoleName);
          if (userProfile.User != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "user profile found successfully",
              Data = userProfile
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "user profile not found",
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