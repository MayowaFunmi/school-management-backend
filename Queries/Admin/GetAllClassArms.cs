using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllClassArms
  {
    public class GetAllClassArmsQuery : IRequest<GenericResponse>
    {
      public string? SchoolId { get; set; }
      public string? ClassId { get; set; }
    }

    public class GGetAllClassArmsQueryHandlers(IStudentClassServices studentClassServices) : IRequestHandler<GetAllClassArmsQuery, GenericResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;

      public async Task<GenericResponse> Handle(GetAllClassArmsQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var classArms = await _studentClassServices.GetAllClassArms(request.SchoolId!, request.ClassId!);
          if (classArms.Count != 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "class arms in school retrieved successfully",
              Data = classArms
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"No class arm found for school with id {request.SchoolId}",
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