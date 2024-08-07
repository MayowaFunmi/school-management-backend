using SchoolManagementApi.Models.Lessons;

namespace SchoolManagementApi.Interfaces.LessonNotes
{
  public interface ILessonNoteServices
  {
    Task<bool> AddLessonNote(LessonNote lessonNote);
    Task<LessonNote?> GetLessonNoteById(string lessonNoteId);
    Task<List<LessonNote>> TeacherLessonNotes(string teacherId, int page, int pageSize);
    Task<int> TeacherLessonNotesCount(string teacherId);
  }
}