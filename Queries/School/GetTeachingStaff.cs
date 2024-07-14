using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.School
{
  public class GetTeachingStaff
  {
    public class GetTeachingStaffQuery : IRequest<PageDataResponse>
    {
      public string SchoolId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetTeachingStaffHandler(ISchoolServices schoolServices) : IRequestHandler<GetTeachingStaffQuery, PageDataResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<PageDataResponse> Handle(GetTeachingStaffQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var teachersCount = await _schoolServices.GetAllTeachersInSchoolCount(request.SchoolId);
          var teachers = await _schoolServices.GetAllTeachersInSchool(request.SchoolId, request.Page, request.PageSize);
          var paginationMetaData = new PaginationMetaData(request.Page, request.PageSize, teachersCount);

          if (teachers.Count != 0)
          {
            return new PageDataResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "Teachers in school retrieved successfully",
            Data = teachers,
            Pagination = paginationMetaData
          };
          }

          return new PageDataResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "No teacher found in school",
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