using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Models;
using WatchDog;

namespace SchoolManagementApi.Services.Admin
{
  public class SubjectService(ApplicationDbContext context, ILoggerManager logger) : ISubjectService
  {
    private readonly ApplicationDbContext _context = context;
    private readonly ILoggerManager _logger = logger;
    private static readonly char[] separator = [' '];

    public async Task<Subject> AddSubject(string subjectName)
    {
      try
      {
        var subject = "";
        if (subjectName.Contains(' '))
          subject = ConvertToLowerCaseWithSpace(subjectName);
        else
          subject = subjectName.ToLower();
        // check if subject already exists
        var checkSubject = await _context.Subjects.AnyAsync(s => s.SubjectName == subject);
        if (!checkSubject)
        {
          var newSubject = new Subject
          {
            SubjectName = subject
          };
          var response = _context.Subjects.Add(newSubject);
          await _context.SaveChangesAsync();
          return response.Entity;
        }
        return null;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error Adding Subject - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error Adding Subject - {ex.Message}");
        throw;
      }
    }

    private static string ConvertToLowerCaseWithSpace(string input)
    {
      var lowerCase = input.ToLower();
      var trimmed = lowerCase.Trim();
      var words = trimmed.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      return string.Join(" ", words);
    }
  }
}