using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Intefaces.LoggerManager;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;
using WatchDog;

namespace SchoolManagementApi.Services.Admin
{
  public class OrganizationService(ApplicationDbContext context, ILoggerManager logger) : IOrganizationService
  {
    private readonly ApplicationDbContext _context = context;
    private readonly ILoggerManager _logger = logger;

    public async Task<List<Organization>> AllOrganizations()
    {
      try
      {
        var organizations = await _context.Organizations
          .Include(o => o.Admin)
          .Include(o => o.Zones)
          .ToListAsync();

        return organizations;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting all organizations - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting all organizations - {ex.Message}");
        throw;
      }
    }

    public async Task<Organization> CreateOrganization(Organization organization)
    {
      try
      {
        var response = _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();
        return response.Entity;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error Creating Organization - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error Creating Organization - {ex.Message}");
        throw;
      }
    }

    public Task<List<TeachingStaff>> GetAllTeachersInOrganization(string organizationId, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Organization>> RetrieveAdminOrganizations(string adminId)
    {
      try
      {
        var adminOrgs = await _context.Organizations
          .Include(o => o.Admin)
          .Include(o => o.Zones)
          .ThenInclude(z => z.Schools)
          .Where(x => x.AdminId == adminId)
          .ToListAsync();
        return adminOrgs;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting organizations for admin id - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting organizations for admin id - {ex.Message}");
        throw;
      }
    }
  }
}