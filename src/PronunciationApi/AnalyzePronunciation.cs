using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace PronunciationApi;

public sealed class AnalyzePronunciation
{
    [Function(nameof(AnalyzePronunciation))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "pronunciation/analyze")] HttpRequest req)
    {
        if (!await SubscriptionGate.HasActiveSubscriptionAsync())
            return new StatusCodeResult(StatusCodes.Status403Forbidden);

        if (!req.HasFormContentType)
            return new ObjectResult(new { error = "Expected multipart/form-data" })
            { StatusCode = StatusCodes.Status415UnsupportedMediaType };

        var form = await req.ReadFormAsync();

        var expectedText = form["expectedText"].ToString();
        if (string.IsNullOrWhiteSpace(expectedText))
            return new BadRequestObjectResult(new { error = "Missing form field: expectedText" });

        var language = form["language"].ToString();
        if (string.IsNullOrWhiteSpace(language))
            return new BadRequestObjectResult(new { error = "Missing form field: language" });

        var audioFile = form.Files.GetFile("audio") ?? form.Files.FirstOrDefault();
        if (audioFile is null)
            return new BadRequestObjectResult(new { error = "Missing form file: audio" });

        // Basic size guard (optional)
        if (audioFile.Length <= 0)
            return new BadRequestObjectResult(new { error = "Audio file is empty" });

        await using var stream = audioFile.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        var speech = req.HttpContext.RequestServices.GetRequiredService<SpeechClient>();

        try
        {
            var (status, json, ct) = await speech.AnalyzeAsync(
                ms.ToArray(),
                audioFile.ContentType,
                language,
                expectedText,
                req.HttpContext.RequestAborted);

            return new ContentResult
            {
                StatusCode = status,
                ContentType = ct,
                Content = json
            };
        }
        catch (Exception ex)
        {
            // ������ ������� (�� Speech). �� ����� �����.
            return new ObjectResult(new
            {
                error = "Backend error",
                message = ex.Message
            })
            { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}
