using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using static SchoolManagementApi.Queries.Admin.GetUserByUniqueId;

namespace SchoolManagementApi.Queries.Profiles
{
  public class GetUserById
  {
    public record GetUserByIdQuery(string UserId) : IRequest<GenericResponse>;

    public class GetUserByIdHandler(IAdminService adminService) : IRequestHandler<GetUserByIdQuery, GenericResponse>
    {
      private readonly IAdminService _adminService = adminService;

      public async Task<GenericResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var user = await _adminService.GetUserById(request.UserId);
          if (user == null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.NotFound.ToString(),
              Message = "user not found",
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "user retrieved successfully",
            Data = user
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