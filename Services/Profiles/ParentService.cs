using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Interfaces.Profiles;
using SchoolManagementApi.Models.UserModels;
using WatchDog;

namespace SchoolManagementApi.Services.Profiles
{
  public class ParentService(ApplicationDbContext context, ILoggerManager logger) : IParentService
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

    public async Task<Parent> AddParentProfile(Parent parent)
    {
      try
      {
        var result = _context.Parents.Add(parent);
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

    public async Task<Parent> GetParentById(string parentId)
    {
      try
      {
        var parent = await _context.Parents
          .Include(p => p.User)
          .AsNoTracking()
          .FirstOrDefaultAsync(p => p.ParentId.ToString() == parentId);
        return parent!;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting parent - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting parent - {ex.Message}");
        throw;
      }
    }

    public Task<Parent> GetParentByUniqueId(string parentUniqueId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Parent>> GetParentInSchool(string schoolId)
    {
        throw new NotImplementedException();
    }

    public Task<Parent> GetStudentParent(string studentId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ParentProfileExists(string userId)
    {
      try
      {
        var parent = await _context.Parents.FirstOrDefaultAsync(p => p.ParentId.ToString() == userId);
        if (parent != null)
          return true;
        return false;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error checking if parent profile exists - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error checking if parent profile exists - {ex.Message}");
        throw;
      }
    }
  }
}