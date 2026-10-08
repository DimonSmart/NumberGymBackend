using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace PronunciationApi;

/// <summary>
/// Compatibility endpoint for clients that still call the retired service.
/// Never calls Azure Speech or accepts an audio upload.
/// </summary>
public sealed class AnalyzePronunciation
{
    [Function(nameof(AnalyzePronunciation))]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "pronunciation/analyze")] HttpRequest req)
        => new StatusCodeResult(StatusCodes.Status410Gone);
}
