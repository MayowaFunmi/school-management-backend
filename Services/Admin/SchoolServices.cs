using System.Net;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Interfaces.Admin;
using SchoolManagementApi.Interfaces.LoggerManager;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;
using WatchDog;

namespace SchoolManagementApi.Services.Admin
{
  public class SchoolServices(ApplicationDbContext context, ILoggerManager logger) : ISchoolServices
  {
    private readonly ApplicationDbContext _context = context;
    private readonly ILoggerManager _logger = logger;

    public async Task<bool> AddSchool(School school)
    {
      try
      {
        _context.Schools.Add(school);
        return await _context.SaveChangesAsync() > 0;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error Creating School - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error Creating School - {ex.Message}");
        throw;
      }
    }

    public async Task<GenericResponse> AddSchoolSession(SessionDto sessionDto)
    {
      try
      {
        var session = new SchoolSession
        {
          Name = sessionDto.Name,
          SchoolId = sessionDto.SchoolUniqueId,
          SessionStarts = sessionDto.SessionStarts,
          SessionEnds = sessionDto.SessionEnds
        };
        _context.SchoolSessions.Add(session);
        var res = await _context.SaveChangesAsync();
        if (res > 0)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "School session added successfully"
          };
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "Failed to add school session"
        };
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error Creating School session - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error Creating School session - {ex.Message}");
        throw;
      }
    }

    public async Task<GenericResponse> AddSchoolTerms(SchoolTermDto schoolTermDto)
    {
      try
      {
        Console.WriteLine($"dto service = {schoolTermDto.SchoolSessionId}");
        var session = await _context.SchoolSessions.FirstOrDefaultAsync(s => s.SchoolSessionId.ToString() == schoolTermDto.SchoolSessionId);
        if (session == null)
        {
          return new GenericResponse
          {
            Status = HttpStatusCode.NotFound.ToString(),
            Message = "Session not found"
          };
        }
        List<SchoolTerm> termsList = [];

        foreach (var term in schoolTermDto.SchoolTerms)
        {
          var schTerm = AddTerm(schoolTermDto.SchoolSessionId, term, session!);
          if (schTerm != null && schTerm.Status == "BadRequest")
          {
            return new GenericResponse
            {
              Status = HttpStatusCode.BadRequest.ToString(),
              Message = schTerm.Message
            };
          }
          else if (schTerm != null && schTerm.Data != null)
          {
            termsList.Add((SchoolTerm)schTerm.Data);
          }
        }
        await _context.SchoolTerms.AddRangeAsync(termsList);
        var res = await _context.SaveChangesAsync();
        if (res > 0)
        {
          session.SchoolTerms.AddRange(termsList);
          _context.SchoolSessions.Update(session);
          await _context.SaveChangesAsync();
          return new GenericResponse
          {
            Status = HttpStatusCode.OK.ToString(),
            Message = "School terms created successfully"
          };
        }
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "Failed to add school terms"
        };
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error Creating School terms - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error Creating School terms - {ex.Message}");
        throw;
      }
    }

    private static GenericResponse? AddTerm(string sessionId, TermDto termDto, SchoolSession session)
    {
      var checkTerm = CheckTermDates(session, termDto.TermStarts, termDto.TermEnds);
      if (checkTerm.Status == "false")
      {
        return new GenericResponse
        {
          Status = "BadRequest",
          Message = checkTerm.Message
        };
      }
        
      var term = new SchoolTerm
      {
        SchoolSessionId = sessionId,
        SchoolId = session.SchoolId,
        Name = termDto.TermName,
        TermStarts = termDto.TermStarts,
        TermEnds = termDto.TermEnds
      };
      // _context.SchoolTerms.Add(term);
      // await _context.SaveChangesAsync();
      return new GenericResponse
      {
        Status = "Ok",
        Data = term
      };
    }

    private static GenericResponse CheckTermDates(SchoolSession session, DateTime termStart, DateTime termEnd)
    {
      if (session == null)
      {
        return new GenericResponse
        {
          Status = "false",
          Message = "session does not exist"
        };
      }

      if (termStart < session.SessionStarts || termEnd > session.SessionEnds)
      {
        return new GenericResponse
        {
          Status = "false",
          Message = "Either term start is less than session start or term end is greater than session end"
        };
      }
      if (termEnd < session.SessionStarts || termEnd < termStart || termStart > session.SessionEnds)
      {
        return new GenericResponse
        {
          Status = "false",
          Message = "Either term end is less than session start or term end is less than term start or term start is greater than session end"
        };
      }
      return new GenericResponse
      {
        Status = "true",
      };
    }

    public async Task<List<School>> AllOrganizationScchools(string OrganizationUniqueId, int page, int pageSize)
    {
      try
      {
        List<School> schools = [];
        if (page == 0 || pageSize == 0)
        {
          schools = await _context.Schools
          .Where(s => s.OrganizationUniqueId == OrganizationUniqueId)
          .AsNoTracking()
          .ToListAsync();
        }
        else
        {
          schools = await _context.Schools
          .Where(s => s.OrganizationUniqueId == OrganizationUniqueId)
          .AsNoTracking()
          .Include(s => s.Departments)
          .Include(s => s.StudentClasses)
          .Include(s => s.Zone)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .OrderBy(s => s.Name)
          .ToListAsync();
        }
        return schools;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting all organization schools - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting all organization schools - {ex.Message}");
        throw;
      }
    }

    public async Task<int> AllOrganizationSchoolsCount(string OrganizationUniqueId)
    {
      return await _context.Schools.Where(s => s.OrganizationUniqueId == OrganizationUniqueId).AsNoTracking().CountAsync();
    }

    public async Task<List<School>> AllScchools(int page, int pageSize)
    {
      try
      {
        var schools = await _context.Schools
          .Include(s => s.Departments)
          .Include(s => s.StudentClasses)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .OrderBy(s => s.Name)
          .AsNoTracking()
          .ToListAsync();
        return schools;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting all sCHOOLS - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting all Schools - {ex.Message}");
        throw;
      }
    }

    public async Task<int> AllSchoolCount()
    {
      return await _context.Schools.AsNoTracking().AsNoTracking().CountAsync();
    }

    public async Task<int> AllSchoolsInZoneCount(string ZoneId)
    {
      return await _context.Schools.Where(s => s.ZoneId.ToString() == ZoneId).AsNoTracking().CountAsync();
    }

    public async Task<List<School>> AllZoneScchools(string ZoneId, int page, int pageSize)
    {
      try
      {
        List<School> schools = [];
        if (page == 0 || pageSize == 0)
        {
          schools = await _context.Schools
            .Where(s => s.ZoneId.ToString() == ZoneId)
            .AsNoTracking()
            .ToListAsync();
        }
        else
        {
          schools = await _context.Schools
          .Where(s => s.ZoneId.ToString() == ZoneId)
          .Include(s => s.Departments)
          .Include(s => s.StudentClasses)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .OrderBy(s => s.Name)
          .AsNoTracking()
          .ToListAsync();
        }
        return schools;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting all Schools in the zone with id {ZoneId} - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting all Schools in the zone with id {ZoneId} - {ex.Message}");
        throw;
      }
    }

    public async Task<List<TeachingStaff>> GetAllTeachersInSchool(string schoolId, int page, int pageSize)
    {
      try
      {
        var teacher = await _context.TeachingStaffs
          .Where(t => t.CurrentPostingSchoolId.ToString() == schoolId)
          .Include(t => t.User)
          .Include(t => t.CurrentPostingZone)
          .Include(t => t.CurrentPostingSchool)
          .Include(t => t.Documents)
          .Include(t => t.CurrentSubject)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .OrderBy(s => s.User.LastName)
          .ToListAsync();

        return teacher;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting list of all teachers in school - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting list of all teachers in school - {ex.Message}");
        throw;
      }
    }

    public async Task<int> GetAllTeachersInSchoolCount(string schoolId)
    {
      return await _context.TeachingStaffs.Where(t => t.CurrentPostingSchoolId.ToString() == schoolId).CountAsync();
    }

    public async Task<List<Department>> GetDepartmentsBySchoolId(string schoolId)
    {
      try
      {
        return await _context.Departments
          .Where(d => d.SchoolId.ToString() == schoolId)
          .ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting departments in school - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting department in school - {ex.Message}");
        throw;
      }
    }

    public async Task<List<NonTeachingStaff>> GetNonTeachersInSchool(string schoolId, int page, int pageSize)
    {
      try
      {
        var staff = await _context.NonTeachingStaffs
          .Where(t => t.CurrentPostingSchoolId.ToString() == schoolId)
          .Include(t => t.User)
          .Include(t => t.CurrentPostingZone)
          .Include(t => t.CurrentPostingSchool)
          .Include(t => t.Documents)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .OrderBy(s => s.User.LastName)
          .ToListAsync();

        return staff;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting list of all non teachers in school - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting list of all non teachers in school - {ex.Message}");
        throw;
      }
    }

    public async Task<int> GetNonTeachersInSchoolCount(string schoolId)
    {
      return await _context.NonTeachingStaffs.Where(t => t.CurrentPostingSchoolId.ToString() == schoolId).CountAsync();
    }

    public async Task<List<Parent>> GetParentsInSchool(string schoolId, int page, int pageSize)
    {
      try
      {
        var parents = await _context.Parents
          .Where(t => t.StudentSchoolId.ToString() == schoolId)
          .Include(t => t.User)
          .Include(t => t.StudentSchool)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .OrderBy(s => s.User.LastName)
          .AsNoTracking()
          .ToListAsync();

        return parents;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting list of all parents in school - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting list of all parents in school - {ex.Message}");
        throw;
      }
    }

    public async Task<int> GetParentsInSchoolCount(string schoolId)
    {
      return await _context.Parents.Where(t => t.StudentSchoolId.ToString() == schoolId).CountAsync();
    }

    public async Task<School?> GetSchoolById(string schoolId)
    {
      try
      {
        var school = await _context.Schools
          .Include(s => s.Admin)
          .Include(s => s.Zone)
          .Include(s => s.Departments)
          .Include(s => s.StudentClasses)
          .ThenInclude(c => c.ClassArms)
          .Include(s => s.Subjects)
          .Where(s => s.SchoolId.ToString() == schoolId)
          .AsNoTracking()
          .FirstOrDefaultAsync();
        return school;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting school - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting school - {ex.Message}");
        throw;
      }
    }

    public async Task<School?> GetSchoolByAdminId(string adminId)
    {
      try
      {
        var school = await _context.Schools
          .Include(s => s.Admin)
          .Include(s => s.Zone)
          .Include(s => s.Departments)
          .Include(s => s.StudentClasses)
          .ThenInclude(c => c.ClassArms)
          .Include(s => s.Subjects)
          .AsNoTracking()
          .FirstOrDefaultAsync(s => s.AdminId == adminId);
        return school;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting school by admin id - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting school by admin id - {ex.Message}");
        throw;
      }
    }

    public async Task<List<School>> GetSchoolByIdList(List<string> schoolIds)
    {
      try
      {
        return await _context.Schools
          .Where(s => schoolIds.Contains(s.SchoolId.ToString()))
          .OrderBy(s => s.Name)
          .AsNoTracking()
          .ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting list of schools - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting list of schools - {ex.Message}");
        throw;
      }
    }

    public async Task<List<Parent>> GetSchoolParents(string schoolId)
    {
      try
      {
        return await _context.Parents
        .Where(d => d.StudentSchoolId.ToString() == schoolId)
        .ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting parents in schools - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting parents in schools - {ex.Message}");
        throw;
      }
    }

    public async Task<List<SchoolSession>> GetSchoolSessions(string schoolUniqueId)
    {
      try
      {
        return await _context.SchoolSessions
          .Where(s => s.SchoolId == schoolUniqueId)
          .AsNoTracking()
          .ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting sessions - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting sessions - {ex.Message}");
        throw;
      }
    }

    public async Task<List<ClassArms>> GetStudentClassesBySchoolId(string schoolId)
    {
      try
      {
        return await _context.ClassArms
          .Where(d => d.SchoolId.ToString() == schoolId)
          .AsNoTracking()
          .OrderBy(c => c.Name)
          .ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting class arms in school - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting class arms in school - {ex.Message}");
        throw;
      }
    }

    public async Task<List<Student>> GetStudentsInSchool(string schoolId, int page, int pageSize)
    {
      try
      {
        var students = await _context.Students
          .Where(t => t.CurrentSchoolId.ToString() == schoolId)
          .Include(t => t.User)
          .Include(t => t.SchoolZone)
          .Include(t => t.Department)
          .Include(t => t.StudentClass)
          .Include(t => t.Documents)
          .Include(t => t.Parent)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .OrderBy(s => s.User.LastName)
          .ToListAsync();

        return students;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting list of all students in school - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting list of all students in school - {ex.Message}");
        throw;
      }
    }

    public async Task<int> GetStudentsInSchoolCount(string schoolId)
    {
      return await _context.Students.Where(t => t.CurrentSchoolId.ToString() == schoolId).CountAsync();
    }

    public async Task<List<Subject>> GetSubjectsByIdList(List<string> subjectIds)
    {
      try
      {
        return await _context.Subjects
        .Where(s => subjectIds.Contains(s.SubjectId.ToString()))
        .ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting list of subjects - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting list of subjects - {ex.Message}");
        throw;
      }
    }

    public async Task<List<OrganizationData>> OrganizationData(string organizationUniqueId)
    {
      try
      {
        return  await _context.Schools
          .Where(s => s.OrganizationUniqueId == organizationUniqueId)
          .Select(s => new OrganizationData
          {
            SchholName = s.Name,
            SchholAddress = s.Address,
            SchoolUniqueId = s.SchoolUniqueId
          })
          .ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting organization's school data - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting organization's school data - {ex.Message}");
        throw;
      }
    }

    public async Task<bool> OrganizationExists(string organizationUniqueId)
    {
      try
      {
        var organizationExists = await _context.Organizations
          .Include(o => o.Admin)
          .Include(o => o.Zones)
          .FirstOrDefaultAsync(x => x.OrganizationUniqueId == organizationUniqueId);
        if (organizationExists != null)
          return true;
        else
          return false;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error checking if organization exists - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error checking if organization exists - {ex.Message}");
        throw;
      }
    }

    public async Task<List<School>> SearchSchool(string searchString)
    {
      try
      {
        var searchParam = searchString.ToLower();
        return await _context.Schools
          .Include(s => s.Zone)
          .Include(s => s.Departments)
          .Include(s => s.StudentClasses)
          .Where(s => s.Name.ToLower().Contains(searchParam) 
            || s.Address.ToLower().Contains(searchParam) || s.LocalGovtArea.ToLower().Contains(searchParam) || s.State.ToLower().Contains(searchParam))
          .ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error searching for schools - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error searching for schools - {ex.Message}");
        throw;
      }
    }

    public async Task<List<SchoolTerm>> GetSchoolTerms(string schoolId)
    {
      try
      {
        var schoolTerm = await _context.SchoolTerms
          .Where(s => s.SchoolId == schoolId)
          .ToListAsync();
        return schoolTerm;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting school terms - {ex.Message}");
        WatchLogger.LogError(ex.ToString(), $"Error getting school terms - {ex.Message}");
        throw;
      }
    }
  }
}