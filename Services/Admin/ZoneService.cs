using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagementApi.Constants;
using SchoolManagementApi.Data;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;
using WatchDog;

namespace SchoolManagementApi.Services.Admin
{
  public class ZoneService(ApplicationDbContext context, ILoggerManager logger, IMemoryCache cache) : IZoneService
  {
    private readonly ApplicationDbContext _context = context;
    private readonly ILoggerManager _logger = logger;
    private readonly IMemoryCache _cache = cache;

    public async Task<List<Zone>> AllOrganizationZones(string organizationId)
    {
        try
        {
          return await _context.Zones
              .Include(x => x.Schools)
              .Where(x => x.OrganizationId.ToString() == organizationId)
              .AsNoTracking()
              .ToListAsync();
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error getting all organization zones- {ex.Message}");
          WatchLogger.LogError(ex.ToString(), $"Error getting all organization zones - {ex.Message}");
          throw;
        }
    }


    public async Task<bool> CreateZone(Zone zone)
    {
      try
      {
        _context.Zones.Add(zone);
        var result = await _context.SaveChangesAsync() > 0;
        return result;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error Creating Zones - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error Creating Zones - {ex.Message}");
        throw;
      }
    }

    public Task<List<TeachingStaff>> GetAllTeachersInZone(string zoneId, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<string> OrganizationExists(string organizationUniqueId, string adminId)
    {
      try
      {
        var orgId = string.Empty;
        var organizationExists = await _context.Organizations
          .FirstOrDefaultAsync(x => x.OrganizationUniqueId == organizationUniqueId);
        if (organizationExists != null && organizationExists.AdminId == adminId)
        {
          orgId = organizationExists.OrganizationId.ToString();
        }
        return orgId;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error checking if organization exists - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error checking if organization exists - {ex.Message}");
        throw;
      }
    }
  }
}