using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;

namespace SchoolManagementApi.Queries.School
{
  public class GetNonTeachingStaff
  {
    public class GetNonTeachingStaffQuery : IRequest<PageDataResponse>
    {
      public string SchoolId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetNonTeachingStaffHandler(ISchoolServices schoolServices) : IRequestHandler<GetNonTeachingStaffQuery, PageDataResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<PageDataResponse> Handle(GetNonTeachingStaffQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var teachersCount = await _schoolServices.GetNonTeachersInSchoolCount(request.SchoolId);
          var teachers = await _schoolServices.GetNonTeachersInSchool(request.SchoolId, request.Page, request.PageSize);
          var paginationMetaData = new PaginationMetaData(request.Page, request.PageSize, teachersCount);

          if (teachers.Count != 0)
          {
            return new PageDataResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Non teaching staff in school retrieved successfully",
              Data = teachers,
              Pagination = paginationMetaData
            };
          }
          return new PageDataResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "No non teaching staff found in the school",
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