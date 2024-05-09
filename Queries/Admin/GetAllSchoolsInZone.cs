using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.Admin
{
  public class GetAllSchoolsInZone
  {
    public class GetAllSchoolsInZoneQuery : IRequest<GenericResponse>
    {
      public string ZoneId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetAllSchoolsInZoneHandler(ISchoolServices schoolServices) : IRequestHandler<GetAllSchoolsInZoneQuery, GenericResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<GenericResponse> Handle(GetAllSchoolsInZoneQuery request, CancellationToken cancellationToken)
      {
        int totalSchoolCount = await _schoolServices.AllSchoolsInZoneCount(request.ZoneId);
        int totalPages = 1;
        if (request.Page != 0 || request.PageSize != 0)
          totalPages = (int)Math.Ceiling((double)totalSchoolCount / request.PageSize);
        try
        {
          var schools = await _schoolServices.AllZoneScchools(request.ZoneId, request.Page, request.PageSize);
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
              Message = $"{schools.Count} schools retrieved for zone with id {request.ZoneId}",
              Data = response
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"No school found for zone with id {request.ZoneId}",
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