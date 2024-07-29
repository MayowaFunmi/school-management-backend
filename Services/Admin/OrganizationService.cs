using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;
using SchoolManagementApi.Utilities;
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
          .AsNoTracking()
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

    public async Task<GenericResponse> CheckOrganizationStatus(string organizationName)
    {
      try
      {
        // check if organization exists
        var organizations = await _context.Organizations
                                          .Include(o => o.Admin)
                                          .AsNoTracking()
                                          .ToListAsync();
        List<string> names = organizations.Select(o => o.Name.ToLower()).ToList();
        var similarOrganization = SimilarityCheck.FindSimilarRecord(organizationName, names, 12);

        if (!string.IsNullOrEmpty(similarOrganization))
        {
          var getOrg = organizations.FirstOrDefault(o => o.Name == similarOrganization);
          return new GenericResponse
          {
            Status = HttpStatusCode.Conflict.ToString(),
            Message = "Similar organization already exists",
            Data = getOrg
          };
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.OK.ToString(),
          Message = "No similar organization found",
        };
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting all organizations - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting all organizations - {ex.Message}");
        throw;
      }
    }

    public async Task<Organization?> GetOrganizationById(string organizationId)
    {
      try
      {
        return await _context.Organizations.FirstOrDefaultAsync(o => o.OrganizationId.ToString() == organizationId);
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting organization by Id - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting organization by Id - {ex.Message}");
        throw;
      }
    }

    public async Task<Organization?> CreateOrganization(Organization organization)
    {
      try
      {
        // check if organization already exist
        var orgExists = await _context.Organizations.AsNoTracking().AnyAsync(o => o.Name.ToLower() == organization.Name);
        if (orgExists)
        {
          return null;
        }
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
          .AsNoTracking()
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