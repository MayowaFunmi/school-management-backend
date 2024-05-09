using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetSchoolDepartments
  {
    public record GetSchoolDepartmentsQuery(string SchoolId) : IRequest<GenericResponse>;

    public class GetSchoolDepartmentsHandler(IDepartmentServices departmentServices) : IRequestHandler<GetSchoolDepartmentsQuery, GenericResponse>
    {
      private readonly IDepartmentServices _departmentServices = departmentServices;

      public async Task<GenericResponse> Handle(GetSchoolDepartmentsQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var departments = await _departmentServices.GetSchoolDepartments(request.SchoolId);
          Console.WriteLine("departments = " + departments.Count);
          if (departments.Count != 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"{departments.Count} registered departments retrieved succesfully",
              Data = departments
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"No department has been registered for this school",
          };
        }
        catch (Exception ex)
        {
          Console.WriteLine("error = " + ex.Message);
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