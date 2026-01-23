namespace PronunciationApi;

internal sealed class SpeechClient
{
    private readonly HttpClient _http;
    private readonly SpeechOptions _opt;

    public SpeechClient(HttpClient http, SpeechOptions opt)
    {
        _http = http;
        _opt = opt;
    }

    public async Task<(int StatusCode, string Json, string ContentType)> AnalyzeAsync(
        byte[] audioBytes,
        string? audioContentType,
        string language,
        string expectedText,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_opt.Key))
            throw new InvalidOperationException("SPEECH_KEY is missing");
        if (string.IsNullOrWhiteSpace(_opt.Region))
            throw new InvalidOperationException("SPEECH_REGION is missing");

        var endpoint =
            $"https://{_opt.Region}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1" +
            $"?language={Uri.EscapeDataString(language)}&format=detailed";

        using var req = new HttpRequestMessage(HttpMethod.Post, endpoint);
        req.Headers.Add("Ocp-Apim-Subscription-Key", _opt.Key);
        req.Headers.Add("Pronunciation-Assessment", PronunciationAssessmentHeader.BuildBase64(expectedText));

        var content = new ByteArrayContent(audioBytes);
        content.Headers.TryAddWithoutValidation("Content-Type", NormalizeAudioContentType(audioContentType));
        req.Content = content;

        using var resp = await _http.SendAsync(req, ct);
        var json = await resp.Content.ReadAsStringAsync(ct);

        var ctResp = resp.Content.Headers.ContentType?.ToString() ?? "application/json";
        return ((int)resp.StatusCode, json, ctResp);
    }

    private static string NormalizeAudioContentType(string? contentType)
    {
        // Для старта не ужесточаем. Позже можно валидировать wav pcm 16k / ogg opus.
        return string.IsNullOrWhiteSpace(contentType) ? "audio/wav" : contentType;
    }
}
