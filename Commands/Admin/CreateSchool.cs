using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Models;
using SchoolManagementApi.Utilities;

namespace SchoolManagementApi.Commands.Admin
{
  public class CreateSchool
  {
    public class CreateSchoolCommand : IRequest<GenericResponse>
    {
      public string AdminId { get; set; } = string.Empty;
      public string OrganizationUniqueId { get; set; } = string.Empty;
      public string ZoneId { get; set; } = string.Empty;
      public string Name { get; set; } = string.Empty;
      public string Address { get; set; } = string.Empty;
    }

    public class CreateSchoolHandler(ISchoolServices schoolServices) : IRequestHandler<CreateSchoolCommand, GenericResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<GenericResponse> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
      {
        var organizationExists = await _schoolServices.OrganizationExists(request.OrganizationUniqueId);
        if (!organizationExists)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"Organization with unique id {request.OrganizationUniqueId} does not exist, or you are not an admin",            
          };
        }
        var school = new School
        {
          AdminId = request.AdminId,
          OrganizationUniqueId = request.OrganizationUniqueId!,
          SchoolUniqueId = GenerateUserCode.GenerateSchoolUniqueId(),
          ZoneId = Guid.Parse(request.ZoneId!),
          Name = request.Name!,
          Address = request.Address!,
        };
        var schoolCreated = await _schoolServices.AddSchool(school);
        if (schoolCreated)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "School added sucessfully",
          };
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "Failed to add school",
        };
      }
    }
  }
}