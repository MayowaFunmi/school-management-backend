using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using SchoolManagementApi.Configurations;
using SchoolManagementApi.Constants;
using SchoolManagementApi.Data;
using SchoolManagementApi.DTOs;
using SchoolManagementApi.Models.UserModels;
using SchoolManagementApi.Utilities;

namespace SchoolManagementApi.Controllers
{
  public class AuthController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration, IMemoryCache cache) : BaseController
  {
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;
    private readonly IMemoryCache _cache = cache;

    // register users
    [HttpPost]
    [Route("signup")]
    public async Task<GenericResponse> Register([FromBody] RegisterDto registerDto)
    {
      if (!string.IsNullOrEmpty(CurrentUserId))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "User is already logged in"
        };
      }
      var isUsernameExists = await _userManager.Users
                                  .AsNoTracking()
                                  .AnyAsync(u => u.UserName == registerDto.UserName);
      var isEmailExists = await _userManager.Users
                                  .AsNoTracking()
                                  .AnyAsync(u => u.Email == registerDto.Email);
      if (isUsernameExists)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.Conflict.ToString(),
          Message = $"{registerDto.UserName} is already registered. Try to login or click on forgot password"
        };
      }

      if (isEmailExists)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.Conflict.ToString(),
          Message = $"{registerDto.Email} is already registered. Try to login or click on forgot password"
        };
      }

      var confirmOrganization = await _context.Organizations.AsNoTracking().AnyAsync(o => o.OrganizationUniqueId == registerDto.OrganizationUniqueId);
      if (!confirmOrganization)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.NotFound.ToString(),
          Message = $"Organization with unique id {registerDto.OrganizationUniqueId} not found"
        };
      }

      var newUser = new ApplicationUser
      {
        UniqueId = GenerateUserCode.GenerateUserUniqueId(),
        OrganizationId = registerDto.OrganizationUniqueId,
        Email = registerDto.Email,
        UserName = registerDto.UserName,
        FirstName = registerDto.FirstName,
        MiddleName = registerDto.MiddleName,
        LastName = registerDto.LastName,
        PhoneNumber = registerDto.PhoneNumber,
        PercentageCompleted = 30,
        SecurityStamp = Guid.NewGuid().ToString(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow  
      };
      var createdUser = await _userManager.CreateAsync(newUser, registerDto.Password);
      if (!createdUser.Succeeded)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "User Creation Failed"
        };
      }
      await _userManager.AddToRolesAsync(newUser, [StaticUserRoles.Users, registerDto.Role]);
      _cache.Remove(CacheConstants.USERS);
      return new GenericResponse
      {
        Status = HttpStatusCode.OK.ToString(),
        Message = "User Created successfully",
      };
    }

    [HttpPost]
    [Route("organization-admin-signup")]
    public async Task<GenericResponse> OrganizationAdminRegister([FromBody] AdminRegisterDto registerDto)
    {
      if (!string.IsNullOrEmpty(CurrentUserId))
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "User is already logged in"
        };
      }
      var isUsernameExists = await _userManager.Users
                                  .AsNoTracking()
                                  .AnyAsync(u => u.UserName == registerDto.UserName);
      var isEmailExists = await _userManager.Users
                                  .AsNoTracking()
                                  .AnyAsync(u => u.Email == registerDto.Email);
      if (isUsernameExists)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.Conflict.ToString(),
          Message = $"{registerDto.UserName} is already registered. Try to login or click on forgot password"
        };
      }

      if (isEmailExists)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.Conflict.ToString(),
          Message = $"{registerDto.Email} is already registered. Try to login or click on forgot password"
        };
      }

      var newUser = new ApplicationUser
      {
        UniqueId = GenerateUserCode.GenerateOrgUserUniqueId(),
        Email = registerDto.Email,
        UserName = registerDto.UserName,
        FirstName = registerDto.FirstName,
        LastName = registerDto.LastName,
        PhoneNumber = registerDto.PhoneNumber,
        PercentageCompleted = 30,
        SecurityStamp = Guid.NewGuid().ToString(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow  
      };
      var createdUser = await _userManager.CreateAsync(newUser, registerDto.Password);
      if (!createdUser.Succeeded)
      {
        return new GenericResponse
        {
          Status = HttpStatusCode.BadRequest.ToString(),
          Message = "User Creation Failed"
        };
      }
      await _userManager.AddToRolesAsync(newUser, [StaticUserRoles.Users, StaticUserRoles.OrganizationAdmin]);
      _cache.Remove(CacheConstants.USERS);
      return new GenericResponse
      {
        Status = HttpStatusCode.OK.ToString(),
        Message = "Organization Admin User Created successfully",
      };
    }

    // login route
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      if (!string.IsNullOrEmpty(CurrentUserId))
        return Unauthorized("current user is already logged in");

      var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
      //var user = await _userManager.FindByEmailAsync(loginDto.UserName);

      if (user is null)
        return Unauthorized("Username not found!");
      
      var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
      if (!isPasswordCorrect)
        return Unauthorized("Incorrect Password");
      
      var userRoles = await _userManager.GetRolesAsync(user);
      var authClaims = new List<Claim>
      {
        //new(ClaimTypes.Name, user.UserName!),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //new(ClaimTypes.Email, user.Email!),
        new("organizationUniqueId", user.OrganizationId)
      };
      
      foreach (var userRole in userRoles)
      {
        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
      }
      
      var token = GenerateJsonWebToken(authClaims);
      Response.Headers.Authorization = "Bearer " + token;
      user.LoginCount ++;
      user.LastLogin = DateTime.UtcNow;
      await _userManager.UpdateAsync(user);
      // var loginResponse = new LoginResponse
      // {
      //   Token = token,
      //   OrganizationUniqueIdId = user.OrganizationId,
      //   UniqueId = user.UniqueId,
      //   Email = user.Email!,
      //   FirstName = user.FirstName,
      //   MiddleName = user.MiddleName,
      //   LastName = user.LastName,
      //   PhoneNumber = user.PhoneNumber!,
      //   PercentageCompleted = user.PercentageCompleted,
      //   IsActive = user.IsActive,
      //   LoginCount = user.LoginCount,
      //   DateJoined = user.CreatedAt,
      //   UserRoles = userRoles
      // };
      // return Ok(loginResponse);\
      return Ok(token);
    }


    private string GenerateJsonWebToken(List<Claim> claims)
    {
      var jwt = new JwtCredentials
      {
        Secret = _configuration.GetSection("JWT:Secret").Value!,
        Issuer = _configuration.GetSection("JWT:ValidIssuer").Value!,
        Audience = _configuration.GetSection("JWT:ValidAudience").Value!,
        Lifetime = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration.GetSection("JWT:Lifetime").Value))
      };
      var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret));

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Issuer = jwt.Issuer,
        Audience = jwt.Audience,
        Subject = new ClaimsIdentity(claims),
        Expires = jwt.Lifetime,
        SigningCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}