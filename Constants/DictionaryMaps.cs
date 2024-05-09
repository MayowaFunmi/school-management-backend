namespace SchoolManagementApi.Constants
{
  public class DictionaryMaps
  {
    public static class TitleMap
    {
      public static readonly Dictionary<string, string> TitleDictionary = new()
      {
        { "Miss", "Miss" },
        { "Mr", "Mr" },
        { "Mrs", "Mrs" },
        { "Dr", "Dr" },
        { "Prof", "Prof" }
      };

      public static readonly Dictionary<string, string> RelationshipDictionary = new()
      {
        { "Father", "Father" },
        { "Mother", "Mother" },
        { "Guardian", "Guardian" },
        { "GrandParent", "GrandParent" },
        { "Brother", "Brother" },
        { "Sister", "Sister" },
        { "Others", "Others" },
      };

      public static readonly Dictionary<string, string> DesignationDictionary = new()
      {
        { "Supervisor", "Supervisor" },
        { "Principal", "Principal" },
        { "VicePrincipal", "VicePrincipal" },
        { "HeadOfDepartment", "HeadOfDepartment" },
        { "ClassTeacher", "ClassTeacher" },
        { "OfficeAssistant", "OfficeAssistant" },
        { "Secretary", "Secretary" },
        { "Librarian", "Librarian" },
        { "Typist", "Typist" },
        { "Others", "Others" },
      };

      public static readonly Dictionary<string, string> QualificationDictionary = new()
      {
        { "SSCE", "SSCE" },
        { "OND", "OND" },
        { "NCE", "NCE" },
        { "HND", "HND" },
        { "BSc", "BSc" },
        { "BEd", "BEd" },
        { "PGDE", "PGDE" },
        { "BArts", "BArts" },
        { "MSc", "MSc" },
        { "PhD", "PhD" },
      };

      public static readonly Dictionary<string, string> GenderDictionary = new()
      {
        { "Male", "Male" },
        { "Female", "Female" }
      };

      public static readonly Dictionary<string, string> ReligionDictionary = new()
      {
        { "Christianity", "Christianity" },
        { "Islam", "Islam" }
      };

      public static readonly Dictionary<string, string> MaritalDictionary = new()
      {
        { "Single", "Single" },
        { "Engaged", "Engaged" },
        { "Married", "Married" },
        { "Divorced", "Divorced" },
        { "Widow", "Widow" }
      };
    }
  }
}