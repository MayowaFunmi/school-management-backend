using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.Interfaces.LessonNotes;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Models.Lessons;
using WatchDog;

namespace SchoolManagementApi.Services.LessonNotes
{
  public class LessonNoteServices(ApplicationDbContext context, ILoggerManager logger) : ILessonNoteServices
  {
    private readonly ApplicationDbContext _context = context;
    private readonly ILoggerManager _logger = logger;

    public async Task<bool> AddLessonNote(LessonNote lessonNote)
    {
      try
      {
        _context.LessonNotes.Add(lessonNote);
        var addedNote = await _context.SaveChangesAsync();
        return addedNote > 0;
      }
      catch (Exception ex)
      {
          _logger.LogError($"Error adding lesson note - {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error adding lesson note - {ex.Message}");
          throw;
      }
    }

    public async Task<LessonNote?> GetLessonNoteById(string lessonNoteId)
    {
      try
      {
        var note = await _context.LessonNotes
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(l => l.Id.ToString() == lessonNoteId);
        return note;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting lesson note by note id - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting lesson note by note id - {ex.Message}");
        throw;
      }
    }

    public async Task<int> TeacherLessonNotesCount(string teacherId)
    {
      try
      {
        return await _context.LessonNotes.AsNoTracking().CountAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting a teacher's lesson notes count - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting a teacher's lesson notes count - {ex.Message}");
        throw;
      }
    }

    public async Task<List<LessonNote>> TeacherLessonNotes(string teacherId, int page, int pageSize)
    {
      try
      {
        return await _context.LessonNotes
                                  .Where(l => l.TeacherId == teacherId)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .OrderBy(s => s.StartDate)
                                  .AsNoTracking()
                                  .ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting a teacher's lesson notes - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting a teacher's lesson notes - {ex.Message}");
        throw;
      }
    }
  }
}