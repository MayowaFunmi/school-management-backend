namespace SchoolManagementApi.Utilities
{
  public class SimilarityCheck
  {
    public static int LevenshteinDistance(string source, string target)
    {
      if (string.IsNullOrEmpty(source))
        return string.IsNullOrEmpty(target) ? 0 : target.Length;

      if (string.IsNullOrEmpty(target))
        return source.Length;

      var sourceLength = source.Length;
      var targetLength = target.Length;

      var distance = new int[sourceLength + 1, targetLength + 1];

      for (int i = 0; i <= sourceLength; distance[i, 0] = i++) { }
      for (int j = 0; j <= targetLength; distance[0, j] = j++) { }

      for (int i = 1; i <= sourceLength; i++)
      {
        for (int j = 1; j <= targetLength; j++)
        {
          int cost = target[j - 1] == source[i - 1] ? 0 : 1;
          distance[i, j] = Math.Min(
            Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
            distance[i - 1, j - 1] + cost);
        }
      }

      return distance[sourceLength, targetLength];
    }

    public static string FindSimilarRecord(string newName, List<string> existingNames, int threshold)
    {
      newName = newName.ToLower();
      foreach (var existingName in existingNames)
      {
        var distance = LevenshteinDistance(newName, existingName.ToLower());
        if (distance <= threshold)
        {
          return existingName;
        }
      }
      return string.Empty;
    }

    public static void TestThresholds(List<string> existingNames, List<string> testNames, int maxThreshold)
    {
      for (int threshold = 1; threshold <= maxThreshold; threshold++)
      {
        Console.WriteLine($"Testing with threshold = {threshold}");
        foreach (var testName in testNames)
        {
          var similarName = FindSimilarRecord(testName, existingNames, threshold);
          if (similarName != null)
          {
            Console.WriteLine($"'{testName}' is similar to '{similarName}' with threshold {threshold}");
          }
          else
          {
            Console.WriteLine($"'{testName}' has no similar names with threshold {threshold}");
          }
        }
        Console.WriteLine();
      }
    }
  }
}