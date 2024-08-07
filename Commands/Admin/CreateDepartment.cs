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
      public string SchoolId { get; set; } = string.Empty;
      public List<string> DepartmentNames { get; set; } = [];
    }

    public class CreateDepartmentHandler(IDepartmentServices departmentServices) : IRequestHandler<CreateDepartmentCommand, GenericResponse>
    {
      private readonly IDepartmentServices _departmentServices = departmentServices;

      public async Task<GenericResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var existingDepts = new List<string>();
          var newDepts = new List<string>();

          foreach (var dept in request.DepartmentNames)
          {
            if (await _departmentServices.DepartmentExists(dept))
            {
              existingDepts.Add(dept);
            }
            else
            {
              var department = new Department
              {
                SchoolId = Guid.Parse(request.SchoolId!),
                Name = dept,
              };
              var newDept = await _departmentServices.AddDepartment(department);
              if (newDept != null)
                newDepts.Add(dept);
            }
          }

          var joinedNew = newDepts.Count > 0 ? string.Join(", ", newDepts) : null;
          var joinedExisting = existingDepts.Count > 0 ? string.Join(", ", existingDepts) : null;
          string message;

          if (joinedNew == null)
          {
            message = "All departments already exist";
          }
          else if (joinedExisting == null)
          {
            message = "All departments added successfully";
          }
          else
          {
            message = $"{joinedNew} departments created successfully while {joinedExisting} already exist";
          }

          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = message,
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