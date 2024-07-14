using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllOrganizationSchools
  {
    public class GetAllOrganizationSchoolsQuery : IRequest<PageDataResponse>
    {
      public string OrganizationUniqueId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetAllOrganizationSchoolsHandler(ISchoolServices schoolServices) : IRequestHandler<GetAllOrganizationSchoolsQuery, PageDataResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<PageDataResponse> Handle(GetAllOrganizationSchoolsQuery request, CancellationToken cancellationToken)
      {
        try
        {
          int totalSchoolCount = await _schoolServices.AllOrganizationSchoolsCount(request.OrganizationUniqueId!);
          var schools = await _schoolServices.AllOrganizationScchools(request.OrganizationUniqueId!, request.Page, request.PageSize);
          var paginationMetaData = new PaginationMetaData(request.Page, request.PageSize, totalSchoolCount);

          if (schools.Count != 0)
          {
            return new PageDataResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Schools in organization retrieved successfully",
              Data = schools,
              Pagination = paginationMetaData
            };
          }
          return new PageDataResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "No school found for this organization",
          };
        }
        catch (Exception ex)
        {
          return new PageDataResponse
          {
            Status = HttpStatusCode.InternalServerError.ToString(),
            Message = $"An internal server error occurred - {ex.Message}"
          };
        }
      }
    }
  }
}