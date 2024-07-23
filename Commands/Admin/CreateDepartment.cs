using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Models;

namespace SchoolManagementApi.Commands.Admin
{
  public class CreateDepartment
  {
    public class CreateDepartmentCommand : IRequest<GenericResponse>
    {
      public string AdminId { get; set; } = string.Empty;
      public string? SchoolId { get; set; }
      public string? Name { get; set; }
    }

    public class CreateDepartmentHandler(IDepartmentServices departmentServices) : IRequestHandler<CreateDepartmentCommand, GenericResponse>
    {
      private readonly IDepartmentServices _departmentServices = departmentServices;

      public async Task<GenericResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var checkDept = await _departmentServices.DepartmentExists(request.Name!);
          if (!checkDept)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.BadRequest.ToString(),
              Message = $"{request.Name} Department already exists",
            };
          }
          var department = new Department
          {
            SchoolId = Guid.Parse(request.SchoolId!),
            Name = request.Name!,
          };
          var dept = await _departmentServices.AddDepartment(department);
          if (dept != null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Department Created Successfully",
              Data = dept
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to create Department",
          };
        }
        catch (Exception ex)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"An internal server error occurred - {ex.Message}",
          };
        }
      }
    }
  }
}