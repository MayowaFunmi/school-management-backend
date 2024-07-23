namespace SchoolManagementApi.DTOs
{
  public class LoginResponse
  {
    public string Token { get; set; } = string.Empty;
    public string OrganizationUniqueIdId { get; set; } = string.Empty;
    public string UniqueId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int PercentageCompleted { get; set; }
    public bool IsActive { get; set; } = true;
    public int LoginCount { get; set; }
    public DateTime DateJoined { get; set; }
    public IList<string> UserRoles { get; set; } = [];
  }
}