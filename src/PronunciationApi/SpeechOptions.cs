namespace PronunciationApi;

internal sealed class SpeechOptions
{
    public string Key { get; init; } = "";
    public string Region { get; init; } = "";
    public string Locale { get; init; } = "en-US";

    public static SpeechOptions FromEnv() => new()
    {
        Key = Environment.GetEnvironmentVariable("SPEECH_KEY") ?? "",
        Region = Environment.GetEnvironmentVariable("SPEECH_REGION") ?? "",
        Locale = Environment.GetEnvironmentVariable("SPEECH_LOCALE") ?? "en-US",
    };
}
