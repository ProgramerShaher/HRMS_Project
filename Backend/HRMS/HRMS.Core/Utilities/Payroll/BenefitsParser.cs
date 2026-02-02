using System.Text.Json;

namespace HRMS.Core.Utilities.Payroll;

public class BenefitConfigItem
{
    public int ElementId { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Percentage { get; set; }
}

public static class BenefitsParser
{
    public static List<BenefitConfigItem> Parse(string? jsonConfig)
    {
        if (string.IsNullOrWhiteSpace(jsonConfig))
            return new List<BenefitConfigItem>();

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            var items = JsonSerializer.Deserialize<List<BenefitConfigItem>>(jsonConfig, options);
            return items ?? new List<BenefitConfigItem>();
        }
        catch
        {
            // Logging can be added here
            // Fallback to empty list to prevent system crash
            return new List<BenefitConfigItem>();
        }
    }
}
