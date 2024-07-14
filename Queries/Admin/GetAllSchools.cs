using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllSchools
  {
    public class GetAllSchoolsQuery : IRequest<PageDataResponse>
    {
      public int Page { get; set; }
      public int PageSize { get; set; }
    };
    
    public class GetAllSchoolsHandler(ISchoolServices schoolServices) : IRequestHandler<GetAllSchoolsQuery, PageDataResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<PageDataResponse> Handle(GetAllSchoolsQuery request, CancellationToken cancellationToken)
      {
        try
        {
          int totalSchoolCount = await _schoolServices.AllSchoolCount();
          var schools = await _schoolServices.AllScchools(request.Page, request.PageSize);
          var paginationMetaData = new PaginationMetaData(request.Page, request.PageSize, totalSchoolCount);

          if (schools.Count != 0)
          {
            return new PageDataResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "All schools retrieved successfully",
              Data = schools,
              Pagination = paginationMetaData
            };
          }
          return new PageDataResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "No school found"
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