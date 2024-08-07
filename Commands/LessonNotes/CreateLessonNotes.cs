using System.Net;
using MediatR;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.LessonNotes;
using SchoolManagementApi.Models.Lessons;
using static SchoolManagementApi.Commands.Admin.CreateZone;

namespace SchoolManagementApi.Commands.LessonNotes
{
  public class CreateLessonNotes
  {
    public class LessonPeriodDto
    {
      public string LessonNotesId { get; set; } = string.Empty;
      public string SubTopic { get; set; } = string.Empty;
      public string BehaviouralObjective { get; set; } = string.Empty;
      public string PreviousKnowledge { get; set; } = string.Empty;
      public string Presentations { get; set; } = string.Empty; // steps and notes = contents of the lesson
      public string Evaluations { get; set; } = string.Empty;
      public string Conclusion { get; set; } = string.Empty;
      public string Assignment { get; set; } = string.Empty;
      public List<CustomField> CustomFields { get; set; } = [];
    }
    public class CreateLessonNotesCommand : IRequest<GenericResponse>
    {
      public string TeacherId { get; set; } = string.Empty;
      public int WeekNumber { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public string SubjectId { get; set; } = string.Empty;
      public string ClassArmId { get; set; } = string.Empty;
      public string Topic { get; set; } = string.Empty;
      public string SubTopic { get; set; } = string.Empty;
      public string ReferenceBook { get; set; } = string.Empty;
      public string InstructionalAid { get; set; } = string.Empty;
      public bool IsTemplate { get; set; } = false;
      public List<LessonPeriodDto> Periods { get; set; } = [];
      public List<CustomField> CustomFields { get; set; } = [];
    }

    public class CreateLessonNotesHandler(ILessonNoteServices lessonNoteServices) : IRequestHandler<CreateLessonNotesCommand, GenericResponse>
    {
      private readonly ILessonNoteServices _lessonNoteServices = lessonNoteServices;

      public async Task<GenericResponse> Handle(CreateLessonNotesCommand request, CancellationToken cancellationToken)
      {
        try
        {
          var newNote = new LessonNote
          {
            Id = Guid.NewGuid(),
            TeacherId = request.TeacherId,
            WeekNumber = request.WeekNumber,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            SubjectId = request.SubjectId,
            ClassArmId = request.ClassArmId,
            Topic = request.Topic,
            SubTopic = request.SubTopic,
            ReferenceBook = request.ReferenceBook,
            InstructionalAid = request.InstructionalAid,
            IsTemplate = request.IsTemplate,
            CustomFields = request.CustomFields,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
          };

          foreach (var period in request.Periods)
          {
            var newPeriods = new LessonPeriod
            {
              Id = Guid.NewGuid(),
              LessonNotesId = newNote.Id.ToString(),
              SubTopic = period.SubTopic,
              BehaviouralObjective = period.BehaviouralObjective,
              PreviousKnowledge = period.PreviousKnowledge,
              Presentations = period.Presentations,
              Evaluations = period.Evaluations,
              Conclusion = period.Conclusion,
              Assignment = period.Assignment,
              CustomFields = period.CustomFields,
              CreatedAt = DateTime.UtcNow,
              UpdatedAt = DateTime.UtcNow,
            };
            newNote.Periods.Add(newPeriods);
          }

          var addedNote = await _lessonNoteServices.AddLessonNote(newNote);
          if (addedNote)
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.OK.ToString(),
              Message = "Lesson note added successfully",
            };
          }
          return new GenericResponse
          {
            Status = HttpStatusCode.BadRequest.ToString(),
            Message = "Failed to add lesson note",
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