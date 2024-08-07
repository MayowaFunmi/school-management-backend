using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.LessonNotes;

namespace SchoolManagementApi.Queries.LessonNote
{
  public class GetTeacherLessonNotes
  {
    public class GetTeacherLessonNotesQuery : IRequest<PageDataResponse>
    {
      public string TeacherId { get; set; } = string.Empty;
      public int Page { get; set; } = 0;
      public int PageSize { get; set; } = 0;
    }

    public class GetTeacherLessonNotesHandler(ILessonNoteServices lessonNoteServices) : IRequestHandler<GetTeacherLessonNotesQuery, PageDataResponse>
    {
      private readonly ILessonNoteServices _lessonNoteServices = lessonNoteServices;

      public async Task<PageDataResponse> Handle(GetTeacherLessonNotesQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var notesCount = await _lessonNoteServices.TeacherLessonNotesCount(request.TeacherId);
          var notes = await _lessonNoteServices.TeacherLessonNotes(request.TeacherId, request.Page, request.PageSize);
          var paginationMetaData = new PaginationMetaData(request.Page, request.PageSize, notesCount);

          if (notes.Count != 0)
          {
            return new PageDataResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = $"All {notesCount} teacher's notes retrieved successfully",
              Data = notes,
              Pagination = paginationMetaData
            };
          }
          return new PageDataResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "No teacher's note found",
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