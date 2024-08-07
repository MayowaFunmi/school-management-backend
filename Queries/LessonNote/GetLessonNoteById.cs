using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.LessonNotes;

namespace SchoolManagementApi.Queries.LessonNote
{
  public class GetLessonNoteById
  {
    public record GetLessonNoteByIdQuery(string LessonNoteId) : IRequest<GenericResponse>;

    public class GetLessonNoteByIdHandler(ILessonNoteServices lessonNoteServices) : IRequestHandler<GetLessonNoteByIdQuery, GenericResponse>
    {
      private readonly ILessonNoteServices _lessonNoteServices = lessonNoteServices;

      public async Task<GenericResponse> Handle(GetLessonNoteByIdQuery request, CancellationToken cancellationToken)
      {
        try
        {
          var note = await _lessonNoteServices.GetLessonNoteById(request.LessonNoteId);
          if (note == null)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.NotFound.ToString(),
              Message = $"Lesson note not found"
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = $"Lesson note retrieved successfully",
            Data = note
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