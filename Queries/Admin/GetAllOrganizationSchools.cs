using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllOrganizationSchools
  {
    public class GetAllOrganizationSchoolsQuery : IRequest<GenericResponse>
    {
      public string OrganizationUniqueId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetAllOrganizationSchoolsHandler(ISchoolServices schoolServices) : IRequestHandler<GetAllOrganizationSchoolsQuery, GenericResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<GenericResponse> Handle(GetAllOrganizationSchoolsQuery request, CancellationToken cancellationToken)
      {
        int totalSchoolCount = await _schoolServices.AllOrganizationSchoolsCount(request.OrganizationUniqueId!);
        int totalPages = 1;
        if (request.Page != 0 || request.PageSize != 0)
          totalPages = (int)Math.Ceiling((double)totalSchoolCount / request.PageSize);
        
        request.Page = Math.Min(Math.Max(request.Page, 1), totalPages);

        try
        {
          var schools = await _schoolServices.AllOrganizationScchools(request.OrganizationUniqueId!, request.Page, request.PageSize);
          if (schools.Count != 0)
          {
            var response = new PaginationResponse
            {
              Schools = schools,
              TotalPages = totalPages,
              CurrentPage = request.Page,
              PagesLeft = totalPages - request.Page,
            };
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"{schools.Count} schools retrieved for organization with id {request.OrganizationUniqueId}",
              Data = response
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"No school found for organization with id {request.OrganizationUniqueId}",
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