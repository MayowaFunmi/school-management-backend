using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Models;
using SchoolManagementApi.Utilities;

namespace SchoolManagementApi.Commands.Admin
{
  public class CreateSchool
  {
    public class CreateSchoolCommand : IRequest<GenericResponse>
    {
      public string? AdminId { get; set; }
      public string? OrganizationUniqueId { get; set; }
      public string? ZoneId { get; set; }
      public string? Name { get; set; }
      public string? Address { get; set; }
      public string? State { get; set; }
      public string? LocalGovtArea { get; set; }
    }

    public class CreateSchoolHandler(ISchoolServices schoolServices) : IRequestHandler<CreateSchoolCommand, GenericResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<GenericResponse> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
      {
        var organizationExists = await _schoolServices.OrganizationExists(request.OrganizationUniqueId!, request.AdminId!);
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
          OrganizationUniqueId = request.OrganizationUniqueId!,
          SchoolUniqueId = GenerateUserCode.GenerateSchoolUniqueId(),
          ZoneId = Guid.Parse(request.ZoneId!),
          Name = request.Name!,
          Address = request.Address!,
          State = request.State,
          LocalGovtArea = request.LocalGovtArea!
        };
        var schoolCreated = await _schoolServices.AddSchool(school);
        if (schoolCreated != null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "School added sucessfully",
            Data = schoolCreated
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