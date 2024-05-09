using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetOrganizationSchoolData
  {
    public record GetOrganizationSchoolDataQuery(string OrganizationUniqueId) : IRequest<GenericResponse>;

    public class GetOrganizationSchoolDataHandler(ISchoolServices schoolServices) : IRequestHandler<GetOrganizationSchoolDataQuery, GenericResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<GenericResponse> Handle(GetOrganizationSchoolDataQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var organizationData = await _schoolServices.OrganizationData(request.OrganizationUniqueId);
          if (organizationData.Count != 0)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Organization scoool data retrieved successfully",
              Data = organizationData
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = $"No school data found for organization with id {request.OrganizationUniqueId}",
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