using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllStudentClasses
  {
    public record GetAllStudentClassesQuery(string SchoolId) : IRequest<GenericResponse>;

    public class GetAllStudentClassesHandlers(IStudentClassServices studentClassServices) : IRequestHandler<GetAllStudentClassesQuery, GenericResponse>
    {
      private readonly IStudentClassServices _studentClassServices = studentClassServices;

      public async Task<GenericResponse> Handle(GetAllStudentClassesQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var schoolClasses = await _studentClassServices.GetAllClasses(request.SchoolId);
          if (schoolClasses.Count != 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "classes in school retrieved successfully",
              Data = schoolClasses
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"No class found for school with id {request.SchoolId}",
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