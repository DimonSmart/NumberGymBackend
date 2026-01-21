using System.Text;
using System.Text.Json;

namespace PronunciationApi;

internal static class PronunciationAssessmentHeader
{
    public static string BuildBase64(string referenceText)
    {
        var payload = new
        {
            ReferenceText = referenceText,
            GradingSystem = "HundredMark",
            Granularity = "Phoneme",
            Dimension = "Comprehensive",
            EnableMiscue = true
        };

        var json = JsonSerializer.Serialize(payload);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }
}
