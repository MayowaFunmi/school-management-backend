using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;

namespace SchoolManagementApi.Queries.School
{
  public class GetParents
  {
    public class GetParentsQuery : IRequest<PageDataResponse>
    {
      public string SchoolUniqueId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetParentsHandler(ISchoolServices schoolServices) : IRequestHandler<GetParentsQuery, PageDataResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<PageDataResponse> Handle(GetParentsQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var parentsCount = await _schoolServices.GetParentsInSchoolCount(request.SchoolUniqueId);
          var parents = await _schoolServices.GetParentsInSchool(request.SchoolUniqueId, request.Page, request.PageSize);
          var paginationMetaData = new PaginationMetaData(request.Page, request.PageSize, parentsCount);

          if (parents.Count != 0)
          {
            return new PageDataResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Parents in school retrieved successfully",
              Data = parents,
              Pagination = paginationMetaData
            };
          }
          return new PageDataResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "No parent found in school",
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