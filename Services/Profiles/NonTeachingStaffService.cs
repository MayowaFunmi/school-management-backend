using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.LoggerManager;
using SchoolManagementApi.Intefaces.Profiles;
using SchoolManagementApi.Models.DocumentModels;
using SchoolManagementApi.Models.UserModels;
using WatchDog;

namespace SchoolManagementApi.Services.Profiles
{
  public class NonTeachingStaffService(ApplicationDbContext context, ILoggerManager logger) : INonTeachingStaffInterface
  {
    private readonly ApplicationDbContext _context = context;
    private readonly ILoggerManager _logger = logger;

    public async Task<NonTeachingStaff> AddNonTeachingStaff(NonTeachingStaff nonTeachingStaff)
    {
      try
      {
        var result = _context.NonTeachingStaffs.Add(nonTeachingStaff);
        await _context.SaveChangesAsync();
        return result.Entity;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error adding non teaching staff - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error adding non teaching staff - {ex.Message}");
        throw;
      }
    }

    public async Task<NonTeachingStaff> GetStaffById(string userId)
    {
      try
      {
        var staff = await _context.NonTeachingStaffs
          .Where(t => t.UserId == userId)
          .Include(t => t.User)
          .Include(t => t.CurrentPostingZone)
          .Include(t => t.CurrentPostingSchool)
          //.Include(t => t.Documents)
          .FirstOrDefaultAsync();

        return staff!;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting staff by id - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting staff by id - {ex.Message}");
        throw;
      }
    }

    public async Task<NonTeachingStaff> GetStaffByUniqueId(string uniqueId)
    {
      try
      {
        var staff = await _context.NonTeachingStaffs
          .Where(t => t.User.UniqueId == uniqueId)
          .Include(t => t.User)
          .Include(t => t.CurrentPostingZone)
          .Include(t => t.CurrentPostingSchool)
          //.Include(t => t.Documents)
          .FirstOrDefaultAsync();

        return staff!;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting staff by unique id - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting staff by unique id - {ex.Message}");
        throw;
      }
    }

    public async Task<NonTeachingStaff> NonTeachingStaffExists(string userId)
    {
      try
      {
        var staff = await _context.NonTeachingStaffs.FirstOrDefaultAsync(t => t.UserId == userId);
        if (staff != null)
          return staff;
        else
          return null;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error checking if staff profile already exists - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error checking if staff profile already exists - {ex.Message}");
        throw;
      }
    }

    public async Task<DocumentFile> UploadDocuments(string userId, List<string> filesUrls)
    {
      try
      {
        var fileExists = await _context.DocumentFiles.FirstOrDefaultAsync(d => d.UserId == userId);
        if (fileExists == null)
          return null!;
        var file = new DocumentFile
        {
          FilesUrls = filesUrls,
          UserId = userId
        };
        _context.DocumentFiles.Add(file);
        await _context.SaveChangesAsync();
        return file;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error uploading documents - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error uploading documents - {ex.Message}");
        throw;
      }
    }

    public async Task<string> OrganizationExists(string organizationUniqueId)
    {
      var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.OrganizationUniqueId == organizationUniqueId);
      if (organization != null)
        return organization.OrganizationId.ToString();
      return string.Empty;
    }
  }
}