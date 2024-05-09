// namespace SchoolManagementApi.Configurations
// {
//   public class JwtTokenDecoder(IHttpContextAccessor httpContextAccessor)
//   {
//     private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
//     public string GetJwtToken()
//     {
//       HttpContext httpContext = _httpContextAccessor.HttpContext;
//       string authorizationHeader = httpContext.Request.Headers.Authorization;
//       if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
//       {
//         return null;
//       }
//       string jwtToken = authorizationHeader["Bearer ".Length..].Trim();
//       return jwtToken;
//     }

//     public class JwtTokenInfo DecodeJwtToken()
//     {
      
//     }

//     public class JwtTokenInfo
//     {
//       public string Username { get; set; }
//       public string UserId { get; set; }
//       public string Email { get; set; }
//       public List<string> Roles { get; set; }
//     }
//   }
// }