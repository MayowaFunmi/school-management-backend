using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;

namespace SchoolManagementApi.Queries.School
{
  public class GetParents
  {
    public class GetParentsQuery : IRequest<GenericResponse>
    {
      public string SchoolId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetParentsHandler(ISchoolServices schoolServices) : IRequestHandler<GetParentsQuery, GenericResponse>
    {
      private readonly ISchoolServices _schoolServices = schoolServices;

      public async Task<GenericResponse> Handle(GetParentsQuery request, CancellationToken cancellationToken)
      {
        var parentsCount = await _schoolServices.GetParentsInSchoolCount(request.SchoolId);
        int totalPages = 1;
        if (request.Page != 0 || request.PageSize != 0)
          totalPages = (int)Math.Ceiling((double)parentsCount / request.PageSize);
        
        request.Page = Math.Min(Math.Max(request.Page, 1), totalPages);

        try
        {
          var parents = await _schoolServices.GetParentsInSchool(request.SchoolId, request.Page, request.PageSize);
          if (parents.Count != 0)
          {
            var response = new PageRespnses.ParentsPageResponse
            {
              Parents = parents,
              TotalPages = totalPages,
              CurrentPage = request.Page,
              PagesLeft = totalPages - request.Page
            };
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"{parents.Count} parents retrieved succesfully",
              Data = response
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"No parents found in this school",
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