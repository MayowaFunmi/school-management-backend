using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Interfaces.Profiles;
using SchoolManagementApi.Models.UserModels;
using WatchDog;

namespace SchoolManagementApi.Services.Profiles
{
  public class StudentService(ApplicationDbContext context, ILoggerManager logger) : IStudentService
  {
    private readonly ApplicationDbContext _context = context;
    private readonly ILoggerManager _logger = logger;

    public async Task<string> OrganizationExists(string organizationUniqueId)
    {
      var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.OrganizationUniqueId == organizationUniqueId);
      if (organization != null)
        return organization.OrganizationId.ToString();
      return string.Empty;;
    }

    public async Task<Student> AddStudentProfile(Student student)
    {
      try
      {
        var result = _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return result.Entity;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error adding parent - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error adding parent - {ex.Message}");
        throw;
      }
    }

    public async Task<Student> GetStudentById(string studentId)
    {
      try
      {
        var student = await _context.Students
          .Include(p => p.User)
          .Include(p => p.SchoolZone)
          .Include(p => p.CurrentSchool)
          .Include(p => p.Department)
          .Include(p => p.StudentClass)
          .FirstOrDefaultAsync(p => p.StudentId.ToString() == studentId);
        return student!;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting student - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting student - {ex.Message}");
        throw;
      }
    }

    public Task<Student> GetStudentByUbuiqueId(string studentUniqueId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Student>> GetStudentsInSchool(string schoolId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> StudentProfileExists(string studentId)
    {
      try
      {
        var student = await _context.Students.FirstOrDefaultAsync(p => p.StudentId.ToString() == studentId);
        if (student != null)
          return true;
        return false;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error checking if student profile exists - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error checking if student profile exists - {ex.Message}");
        throw;
      }
    }
  }
}