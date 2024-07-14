using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllSchoolsInZone
  {
    public class GetAllSchoolsInZoneQuery : IRequest<PageDataResponse>
    {
      public string ZoneId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetAllSchoolsInZoneHandler(ISchoolServices schoolServices) : IRequestHandler<GetAllSchoolsInZoneQuery, PageDataResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<PageDataResponse> Handle(GetAllSchoolsInZoneQuery request, CancellationToken cancellationToken)
      {
        try
        {
          int totalSchoolCount = await _schoolServices.AllSchoolsInZoneCount(request.ZoneId);
          var schools = await _schoolServices.AllZoneScchools(request.ZoneId, request.Page, request.PageSize);
          var paginationMetaData = new PaginationMetaData(request.Page, request.PageSize, totalSchoolCount);

          if (schools.Count != 0)
          {
            return new PageDataResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Schools in zones retrieved successfully",
              Data = schools,
              Pagination = paginationMetaData
            };
          }
          return new PageDataResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "No school found in this zone"
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