using System.Text;

namespace SchoolManagementApi.Utilities
{
  public class GenerateUserCode
  {
    public static string UniqueId()
    {
      const string numbers = "123456789";
      const string letters = "ABCDEFGHJKLMNPQRTUVWXY";
      var random = new Random();
  
      var code = new StringBuilder();
      for (int i = 0; i < 5; i++)
      {
          code.Append(numbers[random.Next(numbers.Length)]);
      }
      for (int i = 0; i < 2; i++)
      {
          code.Append(letters[random.Next(letters.Length)]);
      }
      
      return code.ToString();
    }

    public static string GenerateUserUniqueId()
    {
      return $"USER-{UniqueId()}";
    }

    public static string GenerateOrgUserUniqueId()
    {
      return $"ORG-USER-{UniqueId()}";
    }

    public static string GenerateOrgUniqueId()
    {
      return $"ORG-{UniqueId()}";
    }
    public static string GenerateSchoolUniqueId()
    {
      return $"SCH-{UniqueId()}";
    }
  }
}