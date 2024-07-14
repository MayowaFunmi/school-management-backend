using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagementApi.Constants;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Intefaces.Admin;
using SchoolManagementApi.Intefaces.LoggerManager;
using SchoolManagementApi.Intefaces.Profiles;
using SchoolManagementApi.Models;
using SchoolManagementApi.Models.UserModels;
using WatchDog;

namespace SchoolManagementApi.Services.Admin
{
    public class AdminService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILoggerManager logger,
        ITeachingStaffInterface teachingStaffInterface,
        INonTeachingStaffInterface nonTeachingStaffInterface,
        IParentService parentService,
        IStudentService studentService,
        IMemoryCache cache
        ) : IAdminService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly ILoggerManager _logger = logger;
        private readonly ITeachingStaffInterface _teachingStaffInterface = teachingStaffInterface;
        private readonly INonTeachingStaffInterface _nonTeachingStaffInterface = nonTeachingStaffInterface;
        private readonly IParentService _parentService = parentService;
        private readonly IStudentService _studentService = studentService;
        private readonly IMemoryCache _cache = cache;
        public async Task<List<UserWithRoles>> GetAllUsersWithRoles()   // for superadmin and owner
        {
            try
            {
                if (!_cache.TryGetValue(CacheConstants.USERS, out List<UserWithRoles>? userWithRoles))
                {
                    userWithRoles = [];
                    var allusers = await _userManager.Users
                                                    .AsNoTracking()
                                                    .ToListAsync();
                    foreach (var user in allusers)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        userWithRoles.Add(new UserWithRoles
                        {
                            User = user,
                            UserRoles = roles
                        });
                    }
                    _cache.Set(CacheConstants.USERS, userWithRoles);
                }
                else
                {
                    userWithRoles ??= [];
                }
                return userWithRoles;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all users with roles - {ex.Message}");
                WatchLogger.LogError(ex.ToString(), $"Error getting all users with roles - {ex.Message}");
                throw;
            }
        }

        public async Task<UserWithRoles> GetUserByUniqueId(string uniqueId, string? userRole)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UniqueId == uniqueId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    //var userRoles = new List<IdentityRole>();
                    TeachingStaff? teacherProfile = null;
                    NonTeachingStaff? nonTeacherProfile = null;
                    Parent? parentProfile = null;
                    Student? studentProfile = null;
                    TeachingStaff? adminTeacher = null;
                    NonTeachingStaff? adminNonTeacher = null;

                    // foreach (var roleName in roles)
                    // {
                    //     var role = await _roleManager.FindByNameAsync(roleName);
                    //     userRoles.Add(role!);
                    // }
                    if (userRole == "TeachingStaff")
                        teacherProfile = await _teachingStaffInterface.GetTeacherById(user.Id);
                    else if (userRole == "NonTeachingStaff")
                        nonTeacherProfile = await _nonTeachingStaffInterface.GetStaffById(user.Id);
                    else if (userRole == "Admin" || userRole == "OrganizationAdmin")
                    {
                        adminTeacher = await _teachingStaffInterface.GetTeacherById(user.Id);
                        if (adminTeacher == null)
                            adminNonTeacher = await _nonTeachingStaffInterface.GetStaffById(user.Id);
                    }
                    else if (userRole == "Parent")
                        parentProfile = await _parentService.GetParentById(user.Id);
                    else if (userRole == "Student")
                        studentProfile = await _studentService.GetStudentById(user.Id);
                    return new UserWithRoles
                    {
                        User = user,
                        UserRoles = roles,
                        TeacherProfile = teacherProfile,
                        NonTeacherProfile = nonTeacherProfile,
                        ParentProfile = parentProfile,
                        StudentProfile = studentProfile,
                        AdminTeacher = adminTeacher,
                        AdminNonTeacher = adminNonTeacher
                    };
                }
                return new UserWithRoles();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user with uniqueId - {ex.Message}");
                WatchLogger.LogError(ex.ToString(), $"Error getting user with uniqueId - {ex.Message}");
                throw;
            }
        }

        public async Task<List<Subject>> GetSubjectsInSchool()
        {
            try
            {
                return await _context.Subjects.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Subjects - {ex.Message}");
                WatchLogger.LogError(ex.ToString(), $"Error getting Subjects - {ex.Message}");
                throw;
            }
        }

        public async Task<OrganizationUserCount> OrganizationUsersByRole(string organizationId, string roleName, int page, int pageSize)
        {
            try
            {
                var usersInOrganization = await _userManager.Users
                    .Where(u => u.OrganizationId == organizationId)
                    .AsNoTracking()
                    .ToListAsync();
                
                // get the role with the specified name
                var role = await _roleManager.FindByNameAsync(roleName) ?? throw new ArgumentException($"Role '{roleName}' not found.");

                // get the ids of users with the specified role
                var userIdsInRole = await _context.UserRoles
                    .Where(ur => ur.RoleId == role.Id)
                    .AsNoTracking()
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                var roleNameUsers = usersInOrganization
                    .Where(u => userIdsInRole.Contains(u.Id))
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(s => s.LastName)
                    .ToList();
                
                return new OrganizationUserCount
                {
                    Users = roleNameUsers,
                    UserCount = roleNameUsers.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting organization admin users - {ex.Message}");
                WatchLogger.LogError(ex.ToString(), $"Error getting organization admin users - {ex.Message}");
                throw;
            }
        }
    }
}